using System;
using System.Collections.Generic;
using ORM.Nhibernate.Model;
using Xunit;

namespace ORM.Nhibernate.Tests {
    public class EmployeeOrderTests : BaseTests {

        public override string ConnectionString {
            get { return "Data Source=.;Initial Catalog=Northwind;Integrated Security=SSPI"; }
        }

        [Fact]
        public void CanGetEmployeeOrders() {
            Employee e = null;

            using (var tx = Session.BeginTransaction()) {
                e = Session.QueryOver<Employee>()
                    .Where(x => x.LastName == "Fuller")
                    .SingleOrDefault();

                tx.Commit();
            }

            Assert.NotNull(e);
            Assert.NotEmpty(e.Orders);
            Assert.Equal(96, e.Orders.Count);
        }

        [Fact]
        public void CanAddAndDeleteOrder() {
            Employee employee = null;
            Order order = null;

            // Adding the Order
            Assert.DoesNotThrow(() => {
                using (var tx = Session.BeginTransaction()) {
                    employee = Session.Get<Employee>(4);
                    order = new Order {
                        CustomerId = "CHOPS",
                        OrderDate = DateTime.Now,
                        RequiredDate = DateTime.Now.AddDays(7),
                        ShipVia = Session.Load<Shipper>(2),
                        ShippedDate = null,
                        ShipName = "",
                        ShipAddress = "",
                        ShipCity = "",
                        ShipRegion = "",
                        ShipPostalCode = "",
                        ShipCountry = ""
                    };

                    employee.AddOrder(order);
                    Session.Update(employee);
                    tx.Commit();
                }
            });

            // Deleting it...
            Assert.DoesNotThrow(() => {
                using (var tx = Session.BeginTransaction()) {
                    employee.RemoveOrder(order);
                    Session.Update(employee);
                    tx.Commit();
                }
            });

        }

        [Fact]
        public void CanUpdateAnEmployeeOrder() {
            Employee e = null;

            Assert.DoesNotThrow(() => {
                using (var tx = Session.BeginTransaction()) {
                    e = Session.QueryOver<Employee>()
                        .Where(x => x.LastName == "Peacock")
                        .JoinQueryOver<Order>(x => x.Orders, NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                        .SingleOrDefault();

                    foreach (var order in e.Orders) {
                        order.ShipRegion = DateTime.Now.ToString("ddMM hhmmss");
                        break;
                    }

                    Session.Update(e);
                    tx.Commit();
                }
            });
        }

        [Fact]
        public void InsertingDuplicateIntoSetThrows() {
            Employee e = null;

            Assert.Throws <NHibernate.NonUniqueObjectException>(() => {
                using (var tx = Session.BeginTransaction()) {

                    e = Session.QueryOver<Employee>()
                        .Where(x => x.LastName == "Peacock")
                        .SingleOrDefault();

                    e.AddOrder(new Order {
                        OrderId = 11029,        // This Id already exists
                        CustomerId = "CHOPS",
                        OrderDate = DateTime.MinValue,
                        RequiredDate = DateTime.MinValue,
                        ShippedDate = DateTime.MinValue,
                        ShipVia = Session.Load<Shipper>(1),
                        Freight = 0m,
                        ShipName = string.Empty,
                        ShipAddress = string.Empty,
                        ShipRegion = string.Empty,
                        ShipPostalCode = string.Empty,
                        ShipCountry = string.Empty,
                    });

                    Session.Update(e);
                    tx.Commit();
                }
            });
        }
    }
}
