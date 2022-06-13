sleep 90s

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P MyNotesDb! -d master -i create-database.sql