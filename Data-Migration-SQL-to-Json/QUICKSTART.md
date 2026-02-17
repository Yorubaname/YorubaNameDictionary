# Quick Start Guide: MySQL to MongoDB Migration

## 5-Minute Setup

### 1. Install Python Dependencies
```powershell
cd migration
pip install -r requirements.txt
```

### 2. Configure Your Credentials
Edit `migration_config.py` and update:

```python
MYSQL_CONFIG = {
    'host': 'YOUR_MYSQL_HOST',        # e.g., 'ec2-34-216-203-18.us-west-2.compute.amazonaws.com'
    'user': 'YOUR_USERNAME',
    'password': 'YOUR_PASSWORD',
    'database': 'yoruba_dictionary',
    'port': 3306,
    'charset': 'utf8mb4'
}

MONGODB_CONFIG = {
    'connection_string': 'YOUR_MONGODB_URI',  # e.g., 'mongodb://localhost:27017/'
    'database': 'yoruba_dictionary',
    'collection': 'Words'
}
```

### 3. Run Migration
```powershell
python migrate_mysql_to_mongodb.py
```

That's it! The script will:
- ✓ Connect to MySQL and MongoDB
- ✓ Read word entries from MySQL
- ✓ Fetch related definitions, examples, and metadata
- ✓ Transform to MongoDB document structure
- ✓ Insert into MongoDB in batches
- ✓ Verify counts match
- ✓ Generate detailed log file

## What Happens During Migration

```
MySQL Database                          MongoDB Collection
├── word_entry                 →      Words (documents)
│   ├── id → MySQLId (reference)
│   ├── _id (auto-generated ObjectId)
│   ├── word → Title
│   ├── pronunciation → Pronunciation
│   ├── syllables → Syllables[]
│   ├── ipa_notation → IpaNotation
│   ├── morphology → Morphology[]
│   ├── media → MediaLinks[]
│   └── ...other fields...
│
├── definition                 →      Definitions[] (nested)
│   ├── content → Content
│   ├── english_translation → EnglishTranslation
│   └── submitted_at → CreatedAt
│
└── definition_examples        →      Definitions[].Examples[] (nested)
    ├── content → Content
    ├── english_translation → EnglishTranslation
    └── type → Type
```

## After Migration Completes

### Check the Results
```powershell
# View detailed log
type migration_log.txt

# Validate migration
python validate_migration.py
```

### Verify Sample Document
```powershell
# Connect to MongoDB and check
db.Words_Production.findOne({Title: "ounje"})
```

## Need Help?

### Check the Logs
```powershell
type migration_log.txt | Select-String "ERROR|FAIL|WARNING"
```

### Common Issues

**Error: "Access denied for user"**
- Update MYSQL_CONFIG with correct credentials

**Error: "connection refused" (MongoDB)**
- Ensure MongoDB is running
- Check MONGODB_CONFIG connection string

**Error: "No such file or directory"**
- Ensure you're in the migration folder
- Check migration_config.py exists

## Advanced Usage

Edit `migration_config.py` to customize:
- `BATCH_SIZE` - Adjust for speed vs memory (default: 100)
- `UNMAPPED_COLUMNS_HANDLING` - Include/exclude unmapped SQL fields
- `LOG_FILE` - Change log filename

---

**Ready to migrate?** Just run:
```powershell
python migrate_mysql_to_mongodb.py
```
