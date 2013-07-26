using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using ORM.Nhibernate.Model;
using Xunit;

namespace ORM.Nhibernate.Tests {
    public class EmployeeTests: BaseTests  {

        public override string ConnectionString {
            get { return "Data Source=.;Initial Catalog=Northwind;Integrated Security=SSPI"; }
        }


        [Fact]
        public void CanGetEmployeeById() {
            Employee e = null; 

            using (var tx = Session.BeginTransaction()) {
                e = Session.Get<Employee>(4);
                tx.Commit();
            }

            Assert.NotNull(e);
            Assert.Equal("Peacock", e.LastName);
            Assert.Equal("Margaret", e.FirstName);
        }

        [Fact]
        public void CanGetEmployeeThatDoesNotExist() {
            Employee e = null;

            using (var tx = Session.BeginTransaction()) {
                e = Session.Load<Employee>(20);
                tx.Commit();
            }

            Assert.NotNull(e);
            Assert.Equal(20, e.EmployeeId);
        }

        [Fact]
        public void CanGetEmployeeByLastname() {
            Employee e = null;

            using (var tx = Session.BeginTransaction()) {
                e = Session.QueryOver<Employee>()
                    .Where(x => x.LastName == "Fuller")
                    .SingleOrDefault();

                tx.Commit();
            }

            Assert.NotNull(e);
            Assert.Equal(2, e.EmployeeId);
            Assert.Equal("Fuller", e.LastName);
            Assert.Equal("Andrew", e.FirstName);
        }


        /// <summary>
        /// Returning a List of UK Sales Reps
        /// </summary>
        [Fact]
        public void CanGetSalesRepsFromUk() {
            IList<Employee> employees = null;

            using (var tx = Session.BeginTransaction()) {

                employees = Session.QueryOver<Employee>()
                    .Where(x => x.Title == "Sales Representative")
                    .And(x => x.Country == "UK")
                    .List();

                tx.Commit();
            }

            Assert.NotNull(employees);
            Assert.Equal(3, employees.Count);
        }


        /// <summary>
        /// Using Restrictions (IsBewteen/And, IsEmpty, IsIn, IsNotEmpty, IsLike, IsNotNull, IsNull)
        /// </summary>
        [Fact]
        public void CanGetEmployeesHiredIn1992() {
            IList<Employee> employees = null;

            using (var tx = Session.BeginTransaction()) {
                employees = Session.QueryOver<Employee>()
                    .WhereRestrictionOn(x => x.HireDate)
                        .IsBetween(new DateTime(1992,1,1))
                        .And(new DateTime(1992, 12, 31))
                    .List();

                tx.Commit();
            }

            Assert.NotNull(employees);
            Assert.Equal(3, employees.Count);
        }

        [Fact]
        public void CanGetFemaleEmployees() {
            IList<Employee> employees = null;

            using (var tx = Session.BeginTransaction()) {
                employees = Session.QueryOver<Employee>()
                    .WhereRestrictionOn(x => x.TitleOfCourtesy)
                        .IsIn(new [] { "Ms.", "Mrs." })
                    .List();

                tx.Commit();
            }

            Assert.NotNull(employees);
            Assert.Equal(5, employees.Count);
        }
    }
}
