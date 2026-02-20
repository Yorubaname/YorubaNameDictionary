# Migration Configuration
# Update these values with your actual connection details

MYSQL_CONFIG = {
    'host': '127.0.0.1',  # e.g., 'localhost' or 'ec2-34-216-203-18.us-west-2.compute.amazonaws.com'
    'user': 'root',  # e.g., 'root'
    'password': '',
    'database': '',
    'port': 3306,
    'charset': 'utf8mb4'
}

MONGODB_CONFIG = {
    'connection_string': 'mongodb://localhost:27017/',  # Update with your MongoDB connection string
    'database': '',
    'collection': ''
}

# Migration Settings
BATCH_SIZE = 100  # Number of records to process at once
RETRY_ATTEMPTS = 3
LOG_FILE = 'migration_log.txt'

# Data handling options
UNMAPPED_COLUMNS_HANDLING = {
    'tags': 'include',  # 'include', 'discard', or 'backup'
    'tonal_mark': 'include',
    'famous_people': 'include',
    'in_other_languages': 'include'
}
