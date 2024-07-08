@echo off
REM Set PostgreSQL database details
SET PGUSER=postgres
SET PGPASSWORD=123456
SET PGDATABASE=comicapp
SET PGHOST=localhost
SET PGPORT=5432
REM Set backup file path
SET BACKUPFILE= "ComicAPI/SqlDB/lastest.backup"

REM Perform the backup
pg_dump -U %PGUSER% -h %PGHOST% -p %PGPORT% -F c -b -v -f %BACKUPFILE% %PGDATABASE%

REM Check if the backup was successful
IF %ERRORLEVEL% EQU 0 (
    echo Backup completed successfully.
) ELSE (
    echo Backup failed.
)

pause