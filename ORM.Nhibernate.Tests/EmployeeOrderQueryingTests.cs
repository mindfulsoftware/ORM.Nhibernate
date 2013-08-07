using System;
using System.Collections.Generic;
using System.Configuration;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using ORM.Nhibernate.Dto;
using ORM.Nhibernate.Model;
using Xunit;
using mdl = ORM.Nhibernate.Model;



namespace ORM.Nhibernate.Tests {
    public class EmployeeOrderQueryingTests : BaseTests {

        public override string ConnectionString {
            get { return ConfigurationManager.ConnectionStrings["cnn"].ConnectionString; }
        }

        // Lazy Loading (Select N+1)
        [Fact]
        public void LazilyLoadingEmployeeOrders() {
            IList<Employee> employees = null;

            using (var tx = Session.BeginTransaction()) {
                employees = Session.QueryOver<Employee>()
                    .WhereRestrictionOn(x => x.EmployeeId)
                        .IsIn(new int[] { 1, 2, 3, 4 })
                    .List();

                tx.Commit();      
            }

            //using (var tx = Session.BeginTransaction()) {
            //    employees = Session.CreateCriteria<Employee>()
            //        .Add(Expression.In("EmployeeId", new int[] {1, 2, 3, 4}))
            //        .List<Employee>();
            //    tx.Commit();
            //}

            decimal totalFrieght = 0;
            foreach (var employee in employees)
                foreach (var order in employee.Orders)
                    totalFrieght += order.Freight;
        }

        // Eager Loading
        [Fact]
        public void EagerLoadingEmployeeOrders() {
            IList<Employee> employees = null;

            using (var tx = Session.BeginTransaction()) {
                employees = Session.QueryOver<Employee>()
                    .WhereRestrictionOn(x => x.EmployeeId)
                        .IsIn(new int[] { 1, 2, 3, 4 })
                    .Fetch(x => x.Orders).Eager 
                    .List();

                tx.Commit();
            }

            //using (var tx = Session.BeginTransaction()) {
            //    employees = Session.CreateCriteria<Employee>()
            //        .SetFetchMode("Orders", FetchMode.Join)
            //        .Add(Expression.In("EmployeeId", new int[] { 1, 2, 3, 4 }))
            //        .List<Employee>();
            //    tx.Commit();
            //}


            decimal totalFrieght = 0;
            foreach (var employee in employees)
                foreach (var order in employee.Orders)
                    totalFrieght += order.Freight;

        }

        // Filtering Child Collections
        [Fact]
        public void ApplyingCriteriaOnChildCollections() {
            Employee employee;

            using (var tx = Session.BeginTransaction()) {

                employee = Session.QueryOver<Employee>()
                    .Where(x => x.EmployeeId == 2)
                    .JoinQueryOver<mdl.Order>(x => x.Orders, JoinType.InnerJoin)
                    .Where(o => o.ShippedDate == null)
                    .SingleOrDefault();
                    
                //employee = Session.CreateCriteria<Employee>()
                //    .Add(Expression.Eq("EmployeeId", 2))
                //    .CreateCriteria("Orders", JoinType.InnerJoin)                  
                //    .Add(Expression.IsNull("ShippedDate"))
                //    .UniqueResult<Employee>();

                Assert.Equal(96, employee.Orders.Count);
                Session.Evict(employee);


                employee = Session.QueryOver<Employee>()
                    .Where(x => x.EmployeeId == 2)
                    .JoinQueryOver<mdl.Order>(x => x.Orders, JoinType.LeftOuterJoin)
                    .Where(o => o.ShippedDate == null)
                    .SingleOrDefault();

                //employee = Session.CreateCriteria<Employee>()
                //    .Add(Expression.Eq("EmployeeId", 2))
                //    .CreateCriteria("Orders", JoinType.LeftOuterJoin)
                //    .Add(Expression.IsNull("ShippedDate"))
                //    .UniqueResult<Employee>();
            }

            Assert.Equal(3, employee.Orders.Count);
        }

        [Fact]
        public void DuplicateParentEntitiesWhenQueriesChildren() {

            using (var tx = Session.BeginTransaction()) {

                var duplicates = Session.QueryOver<Employee>()
                    .OrderBy(x => x.EmployeeId).Asc
                    .JoinQueryOver<mdl.Order>(x => x.Orders, JoinType.LeftOuterJoin)
                    .Where(x => x.ShippedDate == null)
                    .List();

                //var duplicates = Session.CreateCriteria<Employee>()
                //    .AddOrder(NHibernate.Criterion.Order.Asc("EmployeeId"))
                //    .CreateCriteria("Orders", JoinType.LeftOuterJoin)
                //    .Add(Expression.IsNull("ShippedDate"))
                //    .List();

                Assert.Equal(21, duplicates.Count);

                var unique = Session.QueryOver<Employee>()
                    .OrderBy(x => x.EmployeeId).Asc
                    .JoinQueryOver<mdl.Order>(x => x.Orders, JoinType.LeftOuterJoin)
                    .Where(x => x.ShippedDate == null)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();

                //var unique = Session.CreateCriteria<Employee>()
                //    .AddOrder(NHibernate.Criterion.Order.Asc("EmployeeId"))
                //    .CreateCriteria("Orders", JoinType.LeftOuterJoin)
                //    .Add(Expression.IsNull("ShippedDate"))
                //    .SetResultTransformer(Transformers.DistinctRootEntity)
                //    .List();

                Assert.Equal(7, unique.Count);

            }
        }

        // Grouping
        [Fact]
        public void GroupingAndAggregatingResults() {

            Employee e = null;
            mdl.Order o = null;
            EmployeeOrdersByCustomer r = null;

            using (var tx = Session.BeginTransaction()) {

                var outstanding = Session.QueryOver<Employee>(() => e)
                    .JoinAlias(x => x.Orders, () => o, JoinType.LeftOuterJoin) 
                    .Where(() => o.ShippedDate == null)
                    .SelectList(l => l
                        .SelectGroup(() => e.FirstName).WithAlias(() => r.FirstName)
                        .SelectGroup(() => e.LastName).WithAlias(() => r.LastName)
                        .SelectGroup(() => o.CustomerId).WithAlias(() => r.CustomerId)
                        .SelectCount(() => o.OrderId).WithAlias(() => r.Count)
                    )
                    .TransformUsing(Transformers.AliasToBean<EmployeeOrdersByCustomer>())
                    .List<EmployeeOrdersByCustomer>();

                //var outstanding = Session.CreateCriteria<Employee>("e")
                //    .CreateAlias("Orders", "o", JoinType.LeftOuterJoin) 
                //    .Add(Expression.IsNull("o.ShippedDate"))
                //    .SetProjection(
                //        Projections.ProjectionList()
                //            .Add(Projections.GroupProperty("e.FirstName"), "FirstName")    
                //            .Add(Projections.GroupProperty("e.LastName"), "LastName")
                //            .Add(Projections.GroupProperty("o.CustomerId"), "CustomerId")
                //            .Add(Projections.RowCount(), "Count")
                //    )
                //    .SetResultTransformer(Transformers.AliasToBean(typeof(EmployeeOrdersByCustomer)))
                //    .List<EmployeeOrdersByCustomer>();

                Assert.NotEmpty(outstanding);
                Assert.Equal(20, outstanding.Count);

            }
        }
    }
}
