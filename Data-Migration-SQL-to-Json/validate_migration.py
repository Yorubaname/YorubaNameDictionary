#!/usr/bin/env python3
"""
Post-Migration Validation Script
Validates that MySQL → MongoDB migration was successful
"""

import mysql.connector
from pymongo import MongoClient
import logging
from migration_config import MYSQL_CONFIG, MONGODB_CONFIG

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)


class MigrationValidator:
    """Validate migration results"""
    
    def __init__(self):
        self.mysql_conn = None
        self.mongo_client = None
        self.mongo_db = None
        self.mongo_collection = None
        self.validation_errors = []

    def connect(self) -> bool:
        """Connect to both databases"""
        try:
            # MySQL connection
            self.mysql_conn = mysql.connector.connect(**MYSQL_CONFIG)
            logger.info("✓ Connected to MySQL")
            
            # MongoDB connection
            self.mongo_client = MongoClient(MONGODB_CONFIG['connection_string'])
            self.mongo_client.admin.command('ping')
            self.mongo_db = self.mongo_client[MONGODB_CONFIG['database']]
            self.mongo_collection = self.mongo_db[MONGODB_CONFIG['collection']]
            logger.info("✓ Connected to MongoDB")
            
            return True
        except Exception as e:
            logger.error(f"Connection failed: {e}")
            return False

    def validate_document_counts(self) -> bool:
        """Check if document counts match"""
        logger.info("\n" + "="*60)
        logger.info("VALIDATION 1: Document Counts")
        logger.info("="*60)
        
        cursor = self.mysql_conn.cursor(dictionary=True)
        cursor.execute("SELECT COUNT(*) as count FROM word_entry")
        mysql_count = cursor.fetchone()['count']
        cursor.close()
        
        mongo_count = self.mongo_collection.count_documents({})
        
        logger.info(f"MySQL word_entry rows: {mysql_count}")
        logger.info(f"MongoDB Words documents: {mongo_count}")
        
        if mysql_count == mongo_count:
            logger.info("✓ PASS: Counts match")
            return True
        else:
            error = f"COUNT MISMATCH: MySQL={mysql_count}, MongoDB={mongo_count}"
            logger.error(f"✗ FAIL: {error}")
            self.validation_errors.append(error)
            return False

    def validate_definitions(self) -> bool:
        """Check if definitions were nested correctly"""
        logger.info("\n" + "="*60)
        logger.info("VALIDATION 2: Definition Nesting")
        logger.info("="*60)
        
        cursor = self.mysql_conn.cursor(dictionary=True)
        cursor.execute("SELECT COUNT(*) as count FROM definition")
        mysql_def_count = cursor.fetchone()['count']
        cursor.close()
        
        # Count embedded definitions in MongoDB
        mongo_docs = self.mongo_collection.aggregate([
            {"$group": {"_id": None, "total_defs": {"$sum": {"$size": "$Definitions"}}}}
        ])
        
        mongo_def_count = 0
        for doc in mongo_docs:
            mongo_def_count = doc.get('total_defs', 0)
        
        logger.info(f"MySQL definition rows: {mysql_def_count}")
        logger.info(f"MongoDB embedded definitions: {mongo_def_count}")
        
        if mysql_def_count == mongo_def_count:
            logger.info("✓ PASS: All definitions nested correctly")
            return True
        else:
            error = f"DEFINITION COUNT MISMATCH: MySQL={mysql_def_count}, MongoDB={mongo_def_count}"
            logger.error(f"✗ FAIL: {error}")
            self.validation_errors.append(error)
            return False

    def validate_examples(self) -> bool:
        """Check if examples were nested correctly"""
        logger.info("\n" + "="*60)
        logger.info("VALIDATION 3: Examples Nesting")
        logger.info("="*60)
        
        cursor = self.mysql_conn.cursor(dictionary=True)
        cursor.execute("SELECT COUNT(*) as count FROM definition_examples")
        mysql_ex_count = cursor.fetchone()['count']
        cursor.close()
        
        # Count embedded examples in MongoDB
        pipeline = [
            {"$unwind": "$Definitions"},
            {"$unwind": "$Definitions.Examples"},
            {"$count": "total"}
        ]
        
        try:
            result = list(self.mongo_collection.aggregate(pipeline))
            mongo_ex_count = result[0]['total'] if result else 0
        except:
            mongo_ex_count = 0
        
        logger.info(f"MySQL definition_examples rows: {mysql_ex_count}")
        logger.info(f"MongoDB embedded examples: {mongo_ex_count}")
        
        if mysql_ex_count == mongo_ex_count:
            logger.info("✓ PASS: All examples nested correctly")
            return True
        else:
            error = f"EXAMPLES COUNT MISMATCH: MySQL={mysql_ex_count}, MongoDB={mongo_ex_count}"
            logger.error(f"✗ FAIL: {error}")
            self.validation_errors.append(error)
            return False

    def validate_sample_document(self) -> bool:
        """Validate structure of a sample document"""
        logger.info("\n" + "="*60)
        logger.info("VALIDATION 4: Document Structure")
        logger.info("="*60)
        
        sample = self.mongo_collection.find_one()
        if not sample:
            error = "No documents found in MongoDB"
            logger.error(f"✗ FAIL: {error}")
            self.validation_errors.append(error)
            return False
        
        required_fields = [
            '_id', 'Title', 'CreatedAt', 'CreatedBy', 'UpdatedAt', 'UpdatedBy',
            'Pronunciation', 'Syllables', 'IpaNotation', 'Morphology',
            'Definitions', 'GeoLocation', 'State', 'PartOfSpeech',
            'Style', 'GrammaticalFeature'
        ]
        
        missing_fields = [f for f in required_fields if f not in sample]
        
        logger.info(f"Sample document ID: {sample.get('_id')}")
        logger.info(f"Sample document Title: {sample.get('Title')}")
        
        if missing_fields:
            error = f"Missing fields in sample document: {missing_fields}"
            logger.error(f"✗ FAIL: {error}")
            self.validation_errors.append(error)
            return False
        
        # Validate nested structure
        if not isinstance(sample.get('Definitions'), list):
            error = "Definitions is not an array"
            logger.error(f"✗ FAIL: {error}")
            self.validation_errors.append(error)
            return False
        
        if sample['Definitions']:
            definition = sample['Definitions'][0]
            if not isinstance(definition.get('Examples'), list):
                error = "Examples not properly nested in Definition"
                logger.error(f"✗ FAIL: {error}")
                self.validation_errors.append(error)
                return False
        
        logger.info("✓ PASS: Document structure is valid")
        return True

    def validate_no_nulls_key_fields(self) -> bool:
        """Check that key fields don't have unexpected nulls"""
        logger.info("\n" + "="*60)
        logger.info("VALIDATION 5: Key Field Integrity")
        logger.info("="*60)
        
        # Check for null Titles
        null_titles = self.mongo_collection.count_documents({"Title": None})
        null_titles += self.mongo_collection.count_documents({"Title": ""})
        
        # Check for null Definitions arrays
        empty_definitions = self.mongo_collection.count_documents({
            "$expr": {"$eq": [{"$size": "$Definitions"}, 0]}
        })
        
        logger.info(f"Documents with null/empty Title: {null_titles}")
        logger.info(f"Documents with empty Definitions array: {empty_definitions}")
        
        if null_titles > 0:
            error = f"Found {null_titles} documents with null/empty Title"
            logger.warning(error)
            self.validation_errors.append(error)
        
        if empty_definitions > 0:
            logger.info(f"Note: {empty_definitions} words have no definitions (expected)")
        
        logger.info("✓ PASS: Key field integrity check complete")
        return True

    def validate_data_types(self) -> bool:
        """Validate that data types are correct"""
        logger.info("\n" + "="*60)
        logger.info("VALIDATION 6: Data Type Checks")
        logger.info("="*60)
        
        sample = self.mongo_collection.find_one()
        if not sample:
            return True
        
        type_checks = {
            'Syllables': (list, "array"),
            'Morphology': (list, "array"),
            'Definitions': (list, "array"),
            'GeoLocation': (list, "array"),
            'PartOfSpeech': (int, "integer"),
            'Style': (int, "integer"),
            'GrammaticalFeature': (int, "integer"),
        }
        
        all_valid = True
        for field, (expected_type, type_name) in type_checks.items():
            actual_value = sample.get(field)
            if actual_value is not None and not isinstance(actual_value, expected_type):
                error = f"Field '{field}' has wrong type: expected {type_name}, got {type(actual_value).__name__}"
                logger.error(f"✗ FAIL: {error}")
                self.validation_errors.append(error)
                all_valid = False
        
        if all_valid:
            logger.info("✓ PASS: All data types are correct")
        
        return all_valid

    def run(self):
        """Run all validations"""
        if not self.connect():
            return False
        
        logger.info("\n" + "╔" + "="*58 + "╗")
        logger.info("║" + " MIGRATION VALIDATION REPORT ".center(58) + "║")
        logger.info("╚" + "="*58 + "╝")
        
        results = {
            "Document Counts": self.validate_document_counts(),
            "Definitions Nesting": self.validate_definitions(),
            "Examples Nesting": self.validate_examples(),
            "Document Structure": self.validate_sample_document(),
            "Key Field Integrity": self.validate_no_nulls_key_fields(),
            "Data Types": self.validate_data_types(),
        }
        
        # Summary
        logger.info("\n" + "="*60)
        logger.info("SUMMARY")
        logger.info("="*60)
        
        passed = sum(1 for v in results.values() if v)
        total = len(results)
        
        for test_name, result in results.items():
            status = "✓ PASS" if result else "✗ FAIL"
            logger.info(f"{status}: {test_name}")
        
        logger.info("="*60)
        logger.info(f"Result: {passed}/{total} validations passed")
        logger.info("="*60)
        
        if self.validation_errors:
            logger.info("\nERRORS FOUND:")
            for error in self.validation_errors:
                logger.info(f"  • {error}")
        
        # Cleanup
        if self.mysql_conn:
            self.mysql_conn.close()
        if self.mongo_client:
            self.mongo_client.close()
        
        return passed == total


if __name__ == '__main__':
    validator = MigrationValidator()
    success = validator.run()
    exit(0 if success else 1)
