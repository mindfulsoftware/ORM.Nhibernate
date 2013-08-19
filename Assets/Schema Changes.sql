use Northwind
go

if not exists(select * from sys.columns where object_name(object_id) = 'Employees' and name = 'Version')
	alter table dbo.Employees add Version int not null default(0)