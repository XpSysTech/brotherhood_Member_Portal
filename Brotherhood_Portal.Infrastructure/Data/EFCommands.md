Drop-Database 
Remove-Migration
Add-Migration RebuildSchema
Update-Database
Remove-Migration
Add-Migration RebuildSchema
Update-Database
Set-DefaultProject Brotherhood_Portal.Infrastructure
Add-Migration AddFinanceAggregates
Update-Database

docker exec -it boena-postgres psql -U boena_postgres -d brotherhood_db
SELECT "Email" FROM "AspNetUsers";