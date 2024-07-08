@echo off
REM Set PostgreSQL database details
SET PGUSER=postgres
SET PGPASSWORD=123456
SET PGDATABASE=comicapp
SET PGHOST=localhost
SET PGPORT=5432
REM Set backup file path
SET BACKUPFILE="ComicAPI/SqlDB/lastest.backup"

REM Function to terminate all connections to the database
psql -U %PGUSER% -h %PGHOST% -p %PGPORT% -d postgres -c "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '%PGDATABASE%' AND pid <> pg_backend_pid();"

REM Drop the database
psql -U %PGUSER% -h %PGHOST% -p %PGPORT% -d postgres -c "DROP DATABASE IF EXISTS %PGDATABASE%;"

REM Create a new database
psql -U %PGUSER% -h %PGHOST% -p %PGPORT% -d postgres -c "CREATE DATABASE %PGDATABASE%;"

REM Restore the backup
pg_restore -U %PGUSER% -h %PGHOST% -p %PGPORT% -d %PGDATABASE% -v %BACKUPFILE%

REM Check if the restore was successful
IF %ERRORLEVEL% EQU 0 (
    echo Restore completed successfully.
) ELSE (
    echo Restore failed.
)
pause