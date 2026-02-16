# Migration Folder Contents

This folder contains all scripts and tools for migrating Yoruba Dictionary data from MySQL to MongoDB.

## Start Here

1. **Read:** [QUICKSTART.md](QUICKSTART.md) - 5-minute setup
2. **Edit:** `migration_config.py` - Enter your database credentials
3. **Run:** `python migrate_mysql_to_mongodb.py` - Execute migration
4. **Validate:** `python validate_migration.py` - Verify success

## Files Overview

### 🚀 Quick Reference
- **[QUICKSTART.md](QUICKSTART.md)** - Get started in 5 minutes
- **[README.md](README.md)** - Complete documentation

### ⚙️ Configuration
- **migration_config.py** - Database credentials and settings (MUST EDIT)
- **requirements.txt** - Python dependencies

### 📜 Python Scripts
- **migrate_mysql_to_mongodb.py** - Main migration (400+ lines, 5 classes)
- **validate_migration.py** - Post-migration validation (6 checks)
- **rollback_migration.py** - Cleanup and rollback utility

### 📊 Generated During Migration
- **migration_log.txt** - Detailed operation log (generated)

## Workflow

```
┌─────────────────────────────────────────┐
│ 1. Edit migration_config.py             │
│    (MySQL host, user, password, etc)    │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│ 2. pip install -r requirements.txt      │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│ 3. python migrate_mysql_to_mongodb.py   │
│    (Watch console output)               │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│ 4. python validate_migration.py         │
│    (Run 6 integrity checks)             │
└──────────────────┬──────────────────────┘
                   │
                   ▼
            ✓ MIGRATION COMPLETE
```

## What Gets Migrated

| Source | Destination | Details |
|--------|-------------|---------|
| word_entry | Words | Normalized table → MongoDB document |
| definition | Definitions[] | Separate table → embedded array |
| definition_examples | Definitions[].Examples[] | Deeply nested array |
| 19 SQL columns | MongoDB fields | Mapped + 4 optional |

**Total**: ~402 words with ~627 definitions and ~1000+ examples

## Validation Checks

The validator runs 6 checks:
1. ✓ Document counts match
2. ✓ Definitions nested correctly
3. ✓ Examples nested correctly
4. ✓ Document structure valid
5. ✓ Key fields not null
6. ✓ Data types correct

All must pass for successful migration.

## Troubleshooting

**Connection Error?**
```
→ Check migration_config.py credentials
→ Verify MySQL and MongoDB are running
```

**Missing Documents?**
```powershell
→ Review migration_log.txt for errors
→ Run validate_migration.py to see details
```

**Want to Retry?**
```powershell
→ Use rollback_migration.py to clean MongoDB
→ Fix issues
→ Re-run migrate_mysql_to_mongodb.py
```

## Advanced Usage

### Customize Batch Size
Edit `migration_config.py`:
```python
BATCH_SIZE = 500  # Faster (default 100)
```

### Include/Exclude Optional Fields
Edit `migration_config.py`:
```python
UNMAPPED_COLUMNS_HANDLING = {
    'tags': 'include',           # ✓ Include
    'tonal_mark': 'discard',     # ✗ Don't include
    'famous_people': 'include',
    'in_other_languages': 'include'
}
```

### Change Log File Location
Edit `migration_config.py`:
```python
LOG_FILE = 'my_custom_log.txt'
```

## File Descriptions

### migrate_mysql_to_mongodb.py
**Main Migration Script** (398 lines)

Classes:
- `MigrationStats` - Tracks progress
- `MySQLConnection` - MySQL operations
- `MongoDBConnection` - MongoDB operations
- `DataTransformer` - Converts data
- `DataMigration` - Orchestrates workflow

### validate_migration.py
**Validation Script** (320 lines)

Checks:
- Document count matching
- Definition nesting
- Example nesting
- Document structure
- Key field integrity
- Data type validation

### rollback_migration.py
**Cleanup/Rollback Utility** (280 lines)

Interactive menu options:
- View statistics
- List backups
- Create backups
- Empty collection
- Drop collection
- Restore from backup
- Delete by query

---

## Quick Commands

```powershell
# Install dependencies
pip install -r requirements.txt

# Run migration
python migrate_mysql_to_mongodb.py

# Validate results
python validate_migration.py

# Cleanup (interactive)
python rollback_migration.py

# View logs
type migration_log.txt

# Filter errors from log
type migration_log.txt | Select-String "ERROR"
```

## Support

📖 **Read First:** [QUICKSTART.md](QUICKSTART.md)  
📘 **Full Guide:** [README.md](README.md)  
📋 **Check:** migration_log.txt for detailed error info

---

**Ready to migrate?** Edit `migration_config.py` and run:
```powershell
python migrate_mysql_to_mongodb.py
```
