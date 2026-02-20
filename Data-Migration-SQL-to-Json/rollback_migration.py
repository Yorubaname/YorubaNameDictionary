#!/usr/bin/env python3
"""
Rollback/Cleanup Script for Migration
Provides safe ways to clean up MongoDB after migration if needed
"""

from pymongo import MongoClient
import logging
from datetime import datetime
from migration_config import MONGODB_CONFIG

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)


class MigrationRollback:
    """Handle rollback and cleanup operations"""
    
    def __init__(self):
        self.client = None
        self.db = None
        self.collection = None

    def connect(self) -> bool:
        """Connect to MongoDB"""
        try:
            self.client = MongoClient(MONGODB_CONFIG['connection_string'])
            self.client.admin.command('ping')
            self.db = self.client[MONGODB_CONFIG['database']]
            self.collection = self.db[MONGODB_CONFIG['collection']]
            logger.info("✓ Connected to MongoDB")
            return True
        except Exception as e:
            logger.error(f"Connection failed: {e}")
            return False

    def get_collection_stats(self) -> dict:
        """Get current collection statistics"""
        count = self.collection.count_documents({})
        
        # Get storage size
        stats = self.db.command("collStats", MONGODB_CONFIG['collection'])
        
        return {
            'document_count': count,
            'avg_document_size': stats.get('avgObjSize', 0),
            'total_size': stats.get('size', 0),
            'storage_size': stats.get('storageSize', 0)
        }

    def backup_collection_names(self):
        """Create a backup naming convention"""
        timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
        return f"{MONGODB_CONFIG['collection']}_backup_{timestamp}"

    def empty_collection(self) -> bool:
        """Remove all documents from collection"""
        try:
            if self.collection.count_documents({}) == 0:
                logger.info("Collection is already empty")
                return True
            
            logger.warning(f"About to delete ALL documents from {MONGODB_CONFIG['collection']}")
            confirm = input("Type 'YES' to confirm deletion: ").strip()
            
            if confirm != 'YES':
                logger.info("Deletion cancelled")
                return False
            
            result = self.collection.delete_many({})
            logger.info(f"✓ Deleted {result.deleted_count} documents")
            return True
            
        except Exception as e:
            logger.error(f"Failed to empty collection: {e}")
            return False

    def drop_collection(self) -> bool:
        """Drop entire collection"""
        try:
            logger.warning(f"About to DROP collection {MONGODB_CONFIG['collection']}")
            confirm = input("Type 'YES' to confirm: ").strip()
            
            if confirm != 'YES':
                logger.info("Drop collection cancelled")
                return False
            
            self.db.drop_collection(MONGODB_CONFIG['collection'])
            logger.info(f"✓ Collection {MONGODB_CONFIG['collection']} dropped")
            return True
            
        except Exception as e:
            logger.error(f"Failed to drop collection: {e}")
            return False

    def clone_before_drop(self, backup_name: str = None) -> bool:
        """Create backup copy before dropping"""
        try:
            if not backup_name:
                backup_name = self.backup_collection_names()
            
            logger.info(f"Creating backup as {backup_name}...")
            
            # Copy all documents to backup
            documents = list(self.collection.find({}))
            if documents:
                self.db[backup_name].insert_many(documents)
                count = self.db[backup_name].count_documents({})
                logger.info(f"✓ Backup created with {count} documents")
                return True
            else:
                logger.info("No documents to backup")
                return True
                
        except Exception as e:
            logger.error(f"Backup failed: {e}")
            return False

    def delete_documents_by_query(self, query: dict) -> bool:
        """Delete documents matching a query"""
        try:
            count_before = self.collection.count_documents({})
            result = self.collection.delete_many(query)
            count_after = self.collection.count_documents({})
            
            logger.info(f"Deleted {result.deleted_count} documents")
            logger.info(f"Collection count: {count_before} → {count_after}")
            return True
            
        except Exception as e:
            logger.error(f"Delete failed: {e}")
            return False

    def restore_from_backup(self, backup_name: str) -> bool:
        """Restore collection from backup"""
        try:
            if backup_name not in self.db.list_collection_names():
                logger.error(f"Backup collection {backup_name} not found")
                return False
            
            logger.warning(f"About to restore from {backup_name}")
            logger.info(f"Current collection will be DELETED")
            confirm = input("Type 'YES' to confirm restore: ").strip()
            
            if confirm != 'YES':
                logger.info("Restore cancelled")
                return False
            
            # Delete current collection
            self.db.drop_collection(MONGODB_CONFIG['collection'])
            
            # Copy backup to current collection
            backup_collection = self.db[backup_name]
            documents = list(backup_collection.find({}))
            
            if documents:
                self.collection.insert_many(documents)
                count = self.collection.count_documents({})
                logger.info(f"✓ Restored {count} documents from {backup_name}")
                return True
            else:
                logger.error("Backup collection is empty")
                return False
                
        except Exception as e:
            logger.error(f"Restore failed: {e}")
            return False

    def list_backups(self) -> list:
        """List available backup collections"""
        all_collections = self.db.list_collection_names()
        backup_prefix = f"{MONGODB_CONFIG['collection']}_backup_"
        backups = [c for c in all_collections if c.startswith(backup_prefix)]
        
        if not backups:
            logger.info("No backup collections found")
        else:
            logger.info("Available backups:")
            for backup in backups:
                count = self.db[backup].count_documents({})
                logger.info(f"  • {backup} ({count} documents)")
        
        return backups

    def show_menu(self):
        """Interactive menu"""
        while True:
            logger.info("\n" + "="*60)
            logger.info("MIGRATION ROLLBACK & CLEANUP MENU")
            logger.info("="*60)
            
            stats = self.get_collection_stats()
            logger.info(f"\nCurrent Status:")
            logger.info(f"  Documents: {stats['document_count']}")
            logger.info(f"  Avg Size: {stats['avg_document_size']} bytes")
            logger.info(f"  Total Size: {stats['total_size']:,} bytes")
            
            logger.info("\nOptions:")
            logger.info("  1. View collection statistics")
            logger.info("  2. List backup collections")
            logger.info("  3. Create backup before cleanup")
            logger.info("  4. Empty collection (keep structure)")
            logger.info("  5. Drop collection (delete structure)")
            logger.info("  6. Restore from backup")
            logger.info("  7. Delete specific documents")
            logger.info("  8. Exit")
            logger.info("="*60)
            
            choice = input("Select option (1-8): ").strip()
            
            if choice == '1':
                logger.info(f"\nCollection: {MONGODB_CONFIG['collection']}")
                logger.info(f"Database: {MONGODB_CONFIG['database']}")
                for key, value in stats.items():
                    logger.info(f"  {key}: {value}")
            
            elif choice == '2':
                self.list_backups()
            
            elif choice == '3':
                backup_name = self.backup_collection_names()
                self.clone_before_drop(backup_name)
            
            elif choice == '4':
                if self.empty_collection():
                    logger.info("✓ Collection emptied")
            
            elif choice == '5':
                if self.drop_collection():
                    logger.info("✓ Collection dropped")
                    break
            
            elif choice == '6':
                backups = self.list_backups()
                if backups:
                    backup_name = input("Enter backup name to restore: ").strip()
                    if self.restore_from_backup(backup_name):
                        logger.info("✓ Restore complete")
            
            elif choice == '7':
                field = input("Field name (e.g., 'Title'): ").strip()
                value = input("Value to match: ").strip()
                query = {field: value}
                count = self.collection.count_documents(query)
                logger.info(f"Found {count} documents matching query")
                confirm = input("Delete these? (type 'YES'): ").strip()
                if confirm == 'YES':
                    self.delete_documents_by_query(query)
            
            elif choice == '8':
                logger.info("Exiting...")
                break
            
            else:
                logger.warning("Invalid option")

    def disconnect(self):
        """Close MongoDB connection"""
        if self.client:
            self.client.close()
            logger.info("Disconnected from MongoDB")


def main():
    """Main entry point"""
    rollback = MigrationRollback()
    
    if not rollback.connect():
        logger.error("Cannot continue without MongoDB connection")
        return
    
    try:
        rollback.show_menu()
    finally:
        rollback.disconnect()


if __name__ == '__main__':
    main()
