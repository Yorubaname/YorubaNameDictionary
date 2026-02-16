# MySQL to MongoDB Migration - Yoruba Dictionary

Complete Python-based migration toolkit to move data from MySQL 5.7 to MongoDB, transforming from normalized relational schema to document-based data model.

## Quick Start

1. **Install dependencies:**
   ```powershell
   pip install -r requirements.txt
   ```

2. **Configure credentials in `migration_config.py`:**
   - MySQL host, user, password
   - MongoDB connection string
   - Batch size and logging options

3. **Run migration:**
   ```powershell
   python migrate_mysql_to_mongodb.py
   ```

4. **Validate results:**
   ```powershell
   python validate_migration.py
   ```

See `QUICKSTART.md` for detailed 5-minute setup guide.

## Files in This Directory

### Core Migration Scripts

| File | Purpose |
|------|---------|
| **migrate_mysql_to_mongodb.py** | Main migration script - orchestrates entire ETL process |
| **migration_config.py** | Configuration file - **EDIT THIS FIRST** with your credentials |
| **validate_migration.py** | Post-migration validation - runs 6 integrity checks |
| **rollback_migration.py** | Cleanup/rollback utility - interactive menu for MongoDB management |

### Dependencies & Configuration

| File | Purpose |
|------|---------|
| **requirements.txt** | Python package dependencies |

### Documentation

| File | Purpose |
|------|---------|
| **QUICKSTART.md** | 5-minute setup and common issues |
| **README.md** | This file - overview and file structure |

## Migration Process

```
1. Configure credentials → migration_config.py
2. Run migration → migrate_mysql_to_mongodb.py
3. Validate data → validate_migration.py
4. Cleanup (if needed) → rollback_migration.py
```

## Configuration

Edit `migration_config.py`:

```python
MYSQL_CONFIG = {
    'host': '127.0.0.1',
    'user': 'root',
    'password': 'your_password',
    'database': 'yoruba_dictionary',
    'port': 3306,
    'charset': 'utf8mb4'
}

MONGODB_CONFIG = {
    'connection_string': 'mongodb://localhost:27020/',
    'database': 'yoruba_dictionary',
    'collection': 'Words_Production'
}

BATCH_SIZE = 100  # Adjust for performance
```

## Validation Checks

The validation script runs 6 checks:

1. **Document Counts** - MySQL vs MongoDB row counts match
2. **Definition Nesting** - All definitions properly embedded
3. **Examples Nesting** - All examples properly nested in definitions
4. **Document Structure** - Required fields present and correct
5. **Key Field Integrity** - No unexpected nulls/empty values
6. **Data Types** - Fields have correct data types

## Rollback & Cleanup

Interactive menu for:
- View collection statistics
- List backup collections
- Create backups before cleanup
- Empty/drop collections
- Restore from backups
- Delete specific documents

Run with: `python rollback_migration.py`

## Data Transformation Summary

| MySQL | MongoDB |  |
|-------|---------|---|
| word_entry (normalized) | Words (document) | ✓ |
| 19 columns | Fields + arrays | ✓ |
| definition (separate table) | Definitions[] (embedded) | ✓ |
| definition_examples (separate) | Definitions[].Examples[] (nested) | ✓ |

**Unmapped SQL columns (optional):**
- tags
- tonal_mark
- famous_people
- in_other_languages

These are included by default but can be configured in `migration_config.py`.

## Prerequisites

- **Python 3.7+**
- **MySQL 5.7** with Yoruba Dictionary database
- **MongoDB 4.0+** instance
- Network access to both databases

## Installation

```powershell
pip install -r requirements.txt
```

Installs:
- `mysql-connector-python==8.2.0`
- `pymongo==4.6.0`

## Troubleshooting

### MySQL Connection Error
```
Error: Access denied for user
```
→ Check MYSQL_CONFIG credentials in migration_config.py

### MongoDB Connection Error
```
Error: Connection refused
```
→ Ensure MongoDB is running and check connection string

### Migration Logs
Check `migration_log.txt` (generated after migration) for:
- Connection status
- Batch progress
- Errors and warnings
- Final statistics

Error lines can be filtered:
```powershell
type migration_log.txt | Select-String "ERROR|FAIL"
```

## Performance Tuning

Adjust in `migration_config.py`:

| Setting | Impact |
|---------|--------|
| BATCH_SIZE = 50 | Slower but safer (less memory) |
| BATCH_SIZE = 500 | Faster but uses more memory |
| BATCH_SIZE = 100 | Default balanced approach |

## Post-Migration

### Create MongoDB Indexes
```powershell
mongo
db.Words_Production.createIndex({Title: 1})
db.Words_Production.createIndex({State: 1})
```

### Verify Sample Document
```powershell
mongo
db.Words_Production.findOne({Title: "ounje"})
```

### Run Application Tests
Test your application against MongoDB to ensure all queries work correctly.

## Architecture

### MySQLConnection Class
- Manages MySQL connection pool
- Executes queries
- Returns dictionary rows

### MongoDBConnection Class
- Manages MongoDB connection
- Bulk inserts documents
- Counts collections

### DataTransformer Class
- Converts MySQL rows to MongoDB documents
- Handles nested structures
- Parses delimited strings to arrays

### DataMigration Class
- Orchestrates the migration workflow
- Manages batch processing
- Verifies results
- Reports statistics

## Notes

- Migration uses **batch processing** to handle large datasets efficiently
- **No joins used** - separate queries fetch definitions and examples (can be optimized with SQL JOINs)
- **Error handling** includes try-catch with detailed logging
- **All insertions** logged with row counts and error details
- **Validation** runs post-migration to verify data integrity

## Generated Files

After running migration:

- **migration_log.txt** - Detailed operation log with timestamps and statistics

## Testing

Before production migration:

1. Test with smaller BATCH_SIZE
2. Review migration_log.txt for errors
3. Run validate_migration.py
4. Spot-check sample documents in MongoDB
5. Test application queries against MongoDB

---

**Version:** 2.0 | **Created:** February 13, 2026 | **Database:** Yoruba Dictionary
