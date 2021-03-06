use Northwind 
go

-- Outstanding Orders for Andrew Fuller
select	o.* 
from	dbo.Employees e 
join	dbo.Orders o on e.EmployeeId = o.EmployeeId
where	e.EmployeeId = 2 
and		o.ShippedDate is null


-- Outstanding Orders by Employee
select	e.FirstName, e.LastName, CustomerID, count(*)
from	dbo.Employees e 
join	dbo.Orders o on e.EmployeeId = o.EmployeeId
where	o.ShippedDate is null
group by e.FirstName, e.LastName, CustomerID