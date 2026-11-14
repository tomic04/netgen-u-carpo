set nocount on;
set xact_abort on;
begin tran;
print '== spKntMatKnjizenje';
select definition from sys.sql_modules where object_id = object_id('dbo.spKntMatKnjizenje');
print '== procedure koje zove';
select distinct referenced_entity_name from sys.sql_expression_dependencies where referencing_id = object_id('dbo.spKntMatKnjizenje');
rollback;
