#!/usr/bin/env python3
"""
MySQL to MongoDB Migration Script for Yoruba Dictionary
Migrates data from MySQL 5.7 normalized schema to MongoDB document-based schema
"""

import mysql.connector
from pymongo import MongoClient, errors
import json
from datetime import datetime
from typing import List, Dict, Any, Optional
import logging
import sys
from migration_config import MYSQL_CONFIG, MONGODB_CONFIG, BATCH_SIZE, RETRY_ATTEMPTS, LOG_FILE, UNMAPPED_COLUMNS_HANDLING

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler(LOG_FILE, encoding='utf-8'),
        logging.StreamHandler()
    ]
)
logger = logging.getLogger(__name__)


# Enum mappings (based on C# enums)
STATE_ENUM = {
    'NEW': 0,
    'UNPUBLISHED': 1,
    'PUBLISHED': 2,
    'MODIFIED': 3,
    'SUGGESTED': 4
}

PART_OF_SPEECH_ENUM = {
    'noun': 1,
    'pronoun': 2,
    'verb': 3,
    'adjective': 4,
    'adverb': 5,
    'preposition': 6,
    'conjunction': 7,
    'interjection': 8,
    'determiner': 9,
    'article': 10,
    'numeral': 11,
    'auxiliary verb': 12,
    'auxiliary_verb': 12,
    'modal verb': 13,
    'modal_verb': 13,
    'particle': 14
}

STYLE_ENUM = {
    'colloquial': 1
}

GRAMMATICAL_FEATURE_ENUM = {
    'intransitive': 1
}

MEDIA_TYPE_ENUM = {
    'UNKNOWN': 0,
    'IMAGE': 1,
    'AUDIO': 2,
    'VIDEO': 3,
    'ARTICLE': 4
}

EXAMPLE_TYPE_ENUM = {
    'PROVERB': 1,
    'SONG': 2,
    'OTHER': 0
}

# Source DB example type values (0=PROVERB, 1=SONG, 2=OTHER) to Destination (0=Other, 1=Proverb, 2=Song)
EXAMPLE_TYPE_MAP = {
    0: 1,  # PROVERB -> Proverb
    1: 2,  # SONG -> Song
    2: 0   # OTHER -> Other
}


class MigrationStats:
    """Track migration statistics"""
    def __init__(self):
        self.total_words = 0
        self.migrated_words = 0
        self.failed_words = 0
        self.total_definitions = 0
        self.total_examples = 0
        self.total_locations = 0
        self.errors = []

    def report(self):
        """Print migration report"""
        logger.info("=" * 60)
        logger.info("MIGRATION REPORT")
        logger.info("=" * 60)
        logger.info(f"Total words processed: {self.total_words}")
        logger.info(f"Successfully migrated: {self.migrated_words}")
        logger.info(f"Failed: {self.failed_words}")
        logger.info(f"Total definitions migrated: {self.total_definitions}")
        logger.info(f"Total examples migrated: {self.total_examples}")
        logger.info(f"Total geo-locations migrated: {self.total_locations}")
        logger.info("=" * 60)
        if self.errors:
            logger.error(f"\n{len(self.errors)} Errors encountered:")
            for error in self.errors[:10]:  # Show first 10 errors
                logger.error(f"  - {error}")
            if len(self.errors) > 10:
                logger.error(f"  ... and {len(self.errors) - 10} more")


class MySQLConnection:
    """Handle MySQL connections"""
    def __init__(self, config):
        self.config = config
        self.connection = None

    def connect(self):
        """Establish MySQL connection"""
        try:
            config = self.config.copy()
            config['use_unicode'] = True
            config['charset'] = 'utf8mb4'
            self.connection = mysql.connector.connect(**config)
            logger.info("✓ Connected to MySQL successfully")
            return True
        except mysql.connector.Error as e:
            logger.error(f"✗ Failed to connect to MySQL: {e}")
            return False

    def disconnect(self):
        """Close MySQL connection"""
        if self.connection:
            self.connection.close()
            logger.info("MySQL connection closed")

    def execute_query(self, query: str, params: tuple = ()) -> List[Dict]:
        """Execute query and return results"""
        try:
            cursor = self.connection.cursor(dictionary=True)
            cursor.execute(query, params)
            results = cursor.fetchall()
            cursor.close()
            return results
        except mysql.connector.Error as e:
            logger.error(f"Query failed: {e}")
            return []

    def get_word_count(self) -> int:
        """Get total word count"""
        results = self.execute_query("SELECT COUNT(*) as count FROM word_entry")
        return results[0]['count'] if results else 0

    def get_words_batch(self, offset: int, limit: int) -> List[Dict]:
        """Get batch of words"""
        query = "SELECT * FROM word_entry ORDER BY word ASC LIMIT %s OFFSET %s"
        return self.execute_query(query, (limit, offset))

    def get_definitions(self, word_id: int) -> List[Dict]:
        """Get all definitions for a word"""
        query = "SELECT * FROM definition WHERE word_id = %s"
        return self.execute_query(query, (word_id,))

    def get_examples(self, definition_id: int) -> List[Dict]:
        """Get all examples for a definition"""
        query = "SELECT * FROM definition_examples WHERE definition_id = %s"
        return self.execute_query(query, (definition_id,))

    def get_etymologies(self, word_id: int) -> List[Dict]:
        """Get all etymologies for a word"""
        query = "SELECT * FROM word_entry_etymology WHERE word_entry_id = %s"
        return self.execute_query(query, (word_id,))

    def get_feedbacks(self, word: str) -> List[Dict]:
        """Get all feedback for a word by matching word name"""
        query = "SELECT * FROM word_entry_feedback WHERE word = %s"
        return self.execute_query(query, (word,))

    def get_variants(self, word_id: int) -> List[Dict]:
        """Get all variants for a word"""
        query = "SELECT * FROM word_entry_variants WHERE word_entry_id = %s"
        return self.execute_query(query, (word_id,))

    def get_media_links(self, word_id: int) -> List[Dict]:
        """Get all media links for a word"""
        query = "SELECT * FROM word_entry_media_links WHERE word_entry_id = %s"
        return self.execute_query(query, (word_id,))


class MongoDBConnection:
    """Handle MongoDB connections"""
    def __init__(self, config):
        self.config = config
        self.client = None
        self.db = None
        self.collection = None

    def connect(self):
        """Establish MongoDB connection"""
        try:
            self.client = MongoClient(self.config['connection_string'])
            # Test connection
            self.client.admin.command('ping')
            self.db = self.client[self.config['database']]
            self.collection = self.db[self.config['collection']]
            logger.info("✓ Connected to MongoDB successfully")
            return True
        except Exception as e:
            logger.error(f"✗ Failed to connect to MongoDB: {e}")
            return False

    def disconnect(self):
        """Close MongoDB connection"""
        if self.client:
            self.client.close()
            logger.info("MongoDB connection closed")

    def insert_document(self, document: Dict) -> bool:
        """Insert single document"""
        try:
            result = self.collection.insert_one(document)
            return result.inserted_id is not None
        except errors.PyMongoError as e:
            logger.error(f"Failed to insert document: {e}")
            return False

    def insert_many_documents(self, documents: List[Dict]) -> tuple:
        """Insert multiple documents, return (success_count, failed_count)"""
        if not documents:
            return 0, 0
        
        try:
            result = self.collection.insert_many(documents, ordered=False)
            return len(result.inserted_ids), 0
        except errors.BulkWriteError as e:
            success_count = e.details['nInserted']
            failed_count = len(documents) - success_count
            logger.warning(f"Bulk insert partially succeeded: {success_count} inserted, {failed_count} failed")
            return success_count, failed_count
        except Exception as e:
            logger.error(f"Insert many failed: {e}")
            return 0, len(documents)

    def get_document_count(self) -> int:
        """Get total document count in collection"""
        return self.collection.count_documents({})


class DataTransformer:
    """Transform MySQL data to MongoDB document structure"""
    
    @staticmethod
    def convert_enum_value(value: str, enum_map: Dict[str, int], default: Optional[int] = None) -> Optional[int]:
        """Convert enum string value to integer"""
        if not value:
            return default
        
        value_stripped = value.strip()
        
        # Exact match only
        if value_stripped in enum_map:
            return enum_map[value_stripped]
        
        # Log for debugging if value not found
        logger.warning(f"Enum value not found: '{value}' in map {list(enum_map.keys())}")
        return default
    
    @staticmethod
    def transform_word(mysql_word: Dict, definitions: List[Dict], geo_locations: List[Dict], etymologies: List[Dict], feedbacks: List[Dict], variants: List[Dict], media_links: List[Dict]) -> Dict:
        """Transform a complete word record to MongoDB document"""
        
        # Transform definitions with nested examples
        transformed_definitions = []
        for definition in definitions:
            examples = definition.pop('examples', [])  # Remove from source
            
            # Transform examples
            transformed_examples = []
            for example in examples:
                example_type_value = example.get('type', 2)  # Default to OTHER (2)
                # Map the source type to destination type
                mapped_type = EXAMPLE_TYPE_MAP.get(example_type_value, 0)
                
                transformed_examples.append({
                    "Content": example.get('content', ''),
                    "EnglishTranslation": example.get('english_translation', ''),
                    "Type": mapped_type
                })
            
            transformed_definitions.append({
                "CreatedAt": definition.get('submitted_at'),
                "CreatedBy": None,
                "UpdatedAt": definition.get('submitted_at'),
                "UpdatedBy": None,
                "Content": definition.get('content', ''),
                "EnglishTranslation": definition.get('english_translation', ''),
                "Examples": transformed_examples
            })
        
        # Transform geo locations
        transformed_locations = []
        for location in geo_locations:
            transformed_locations.append({
                "CreatedAt": None,
                "CreatedBy": None,
                "UpdatedAt": None,
                "UpdatedBy": None,
                "Place": location.get('place', ''),
                "Region": location.get('region', '')
            })
        
        # Transform etymologies
        transformed_etymologies = []
        for etymology in etymologies:
            if etymology.get('part'):  # Only add if has content
                transformed_etymologies.append({
                    "Meaning": etymology.get('meaning', ''),
                    "Part": etymology.get('part', '')
                })
        
        # Transform feedbacks
        transformed_feedbacks = []
        for feedback in feedbacks:
            if feedback.get('feedback'):  # Only add if has content
                transformed_feedbacks.append({
                    "Content": feedback.get('feedback', '')
                })
        
        # Transform variants
        transformed_variants = []
        for variant in variants:
            if variant.get('word'):  # Only add if has content
                variant_geo_place = variant.get('geo_location_id', '')
                # Find the matching geo location
                variant_geo_location = None
                if variant_geo_place:
                    for location in geo_locations:
                        if location.get('place') == variant_geo_place:
                            variant_geo_location = {
                                "Place": location.get('place', ''),
                                "Region": location.get('region', '')
                            }
                            break
                
                transformed_variants.append({
                    "Title": variant.get('word', ''),
                    "GeoLocation": variant_geo_location
                })
        
        # Transform media links (all are videos)
        transformed_media_links = []
        for media in media_links:
            if media.get('link'):  # Only add if has content
                transformed_media_links.append({
                    "Url": media.get('link', ''),
                    "Description": media.get('caption'),
                    "Type": MEDIA_TYPE_ENUM['VIDEO']
                })
        
        # Parse arrays if stored as delimited strings
        syllables = mysql_word.get('syllables', '')
        if isinstance(syllables, str):
            syllables = [s.strip() for s in syllables.split(',') if s.strip()]
        
        morphology = mysql_word.get('morphology', '')
        if isinstance(morphology, str):
            morphology = [m.strip() for m in morphology.split(',') if m.strip()]
        
        tags = mysql_word.get('tags', '')
        if isinstance(tags, str):
            tags = [t.strip() for t in tags.split(',') if t.strip()]
        
        in_other_languages = mysql_word.get('in_other_languages', '')
        if isinstance(in_other_languages, str):
            in_other_languages = [l.strip() for l in in_other_languages.split(',') if l.strip()]
        
        # Build MongoDB document
        today = datetime.now().isoformat()
        mongo_doc = {
            "CreatedAt": today,
            "CreatedBy": mysql_word.get('submitted_by'),
            "UpdatedAt": today,
            "UpdatedBy": mysql_word.get('submitted_by'),
            "Title": mysql_word.get('word', ''),
            "Pronunciation": mysql_word.get('pronunciation', '') or None,
            "Syllables": syllables,
            "IpaNotation": mysql_word.get('ipa_notation', '') or None,
            "Morphology": morphology,
            "Etymology": transformed_etymologies,
            "MediaLinks": transformed_media_links,
            "State": DataTransformer.convert_enum_value(mysql_word.get('state', ''), STATE_ENUM, default=STATE_ENUM['PUBLISHED']),
            "Videos": [],  # Not in SQL schema, initializing empty
            "GeoLocation": transformed_locations,
            "VariantsV2": transformed_variants,
            "Modified": None,
            "Duplicates": [],  # Not in SQL schema, initializing empty
            "Feedbacks": transformed_feedbacks,
            "PartOfSpeech": DataTransformer.convert_enum_value(mysql_word.get('part_of_speech', ''), PART_OF_SPEECH_ENUM),
            "Style": DataTransformer.convert_enum_value(mysql_word.get('style', ''), STYLE_ENUM),
            "GrammaticalFeature": DataTransformer.convert_enum_value(mysql_word.get('grammatical_feature', ''), GRAMMATICAL_FEATURE_ENUM),
            "Definitions": transformed_definitions
        }
        
        # Handle unmapped columns based on configuration
        if UNMAPPED_COLUMNS_HANDLING['tags'] == 'include':
            mongo_doc['Tags'] = tags
        if UNMAPPED_COLUMNS_HANDLING['tonal_mark'] == 'include':
            mongo_doc['TonalMark'] = mysql_word.get('tonal_mark', '')
        if UNMAPPED_COLUMNS_HANDLING['famous_people'] == 'include':
            mongo_doc['FamousPeople'] = mysql_word.get('famous_people', '') or None
        if UNMAPPED_COLUMNS_HANDLING['in_other_languages'] == 'include':
            mongo_doc['InOtherLanguages'] = in_other_languages
        
        return mongo_doc


class DataMigration:
    """Orchestrate the migration process"""
    
    def __init__(self):
        self.mysql = MySQLConnection(MYSQL_CONFIG)
        self.mongo = MongoDBConnection(MONGODB_CONFIG)
        self.stats = MigrationStats()

    def validate_connections(self) -> bool:
        """Validate both database connections"""
        logger.info("Validating database connections...")
        
        mysql_ok = self.mysql.connect()
        mongo_ok = self.mongo.connect()
        
        if not (mysql_ok and mongo_ok):
            logger.error("Failed to establish database connections")
            return False
        
        return True

    def get_geo_locations(self, mysql: MySQLConnection, word_id: int) -> List[Dict]:
        """Get geo locations for a word via junction table"""
        query = """
            SELECT g.* 
            FROM geo_location g
            JOIN word_entry_geo_location wgl ON g.place = wgl.geo_location_place
            WHERE wgl.word_entry_id = %s
        """
        return mysql.execute_query(query, (word_id,))

    def migrate_batch(self, words: List[Dict]) -> tuple:
        """Migrate a batch of words"""
        documents_to_insert = []
        
        for word in words:
            try:
                self.stats.total_words += 1
                word_id = word.get('id')
                
                # Fetch related data
                definitions = self.mysql.get_definitions(word_id)
                
                # Fetch examples for each definition
                for definition in definitions:
                    definition['examples'] = self.mysql.get_examples(definition.get('id'))
                    self.stats.total_examples += len(definition['examples'])
                
                self.stats.total_definitions += len(definitions)
                
                # Get geo locations (via word_entry_geo_location junction table)
                geo_locations = self.get_geo_locations(self.mysql, word_id)
                self.stats.total_locations += len(geo_locations)
                
                # Get etymologies
                etymologies = self.mysql.get_etymologies(word_id)
                
                # Get feedbacks (match by word name)
                feedbacks = self.mysql.get_feedbacks(word.get('word', ''))
                
                # Get variants
                variants = self.mysql.get_variants(word_id)
                
                # Get media links
                media_links = self.mysql.get_media_links(word_id)
                
                # Build examples per definition breakdown
                examples_per_def = [len(d.get('examples', [])) for d in definitions]
                examples_str = ', '.join(str(e) for e in examples_per_def) if examples_per_def else '0'
                
                # Log word processing details
                logger.info(f"Processing: '{word.get('word')}' | Definitions: {len(definitions)} | Examples per definition: [{examples_str}] | Locations: {len(geo_locations)} | Etymologies: {len(etymologies)} | Feedbacks: {len(feedbacks)} | Variants: {len(variants)} | Media: {len(media_links)}")
                
                # Transform to MongoDB document
                mongo_doc = DataTransformer.transform_word(word, definitions, geo_locations, etymologies, feedbacks, variants, media_links)
                documents_to_insert.append(mongo_doc)
                
            except Exception as e:
                error_msg = f"Error processing word {word.get('word')}: {str(e)}"
                logger.error(error_msg)
                self.stats.errors.append(error_msg)
                self.stats.failed_words += 1
        
        # Insert all documents in this batch
        if documents_to_insert:
            success, failed = self.mongo.insert_many_documents(documents_to_insert)
            self.stats.migrated_words += success
            self.stats.failed_words += failed
            logger.info(f"Batch completed: {success} inserted, {failed} failed")
            return success, failed
        
        return 0, 0

    def run(self) -> bool:
        """Execute the migration"""
        try:
            if not self.validate_connections():
                return False
            
            logger.info("\n" + "=" * 60)
            logger.info("STARTING MIGRATION: MySQL → MongoDB")
            logger.info("=" * 60)
            
            # Get total word count
            total_count = self.mysql.get_word_count()
            logger.info(f"Total words to migrate: {total_count}")
            
            # Migrate in batches
            for offset in range(0, total_count, BATCH_SIZE):
                logger.info(f"\nProcessing batch: {offset} to {offset + BATCH_SIZE}")
                
                words = self.mysql.get_words_batch(offset, BATCH_SIZE)
                if not words:
                    logger.warning(f"No words found at offset {offset}")
                    break
                
                self.migrate_batch(words)
            
            # Verify migration
            logger.info("\n" + "=" * 60)
            logger.info("VERIFYING MIGRATION")
            logger.info("=" * 60)
            mongo_count = self.mongo.get_document_count()
            logger.info(f"MySQL word count: {total_count}")
            logger.info(f"MongoDB document count: {mongo_count}")
            
            if total_count == mongo_count:
                logger.info("✓ Document counts match!")
            else:
                logger.warning(f"✗ Count mismatch: {total_count} vs {mongo_count}")
            
            # Print final report
            self.stats.report()
            
            return True
            
        except Exception as e:
            logger.error(f"Migration failed: {e}", exc_info=True)
            return False
        
        finally:
            self.mysql.disconnect()
            self.mongo.disconnect()


def main():
    """Main entry point"""
    migration = DataMigration()
    success = migration.run()
    
    sys.exit(0 if success else 1)


if __name__ == '__main__':
    main()
