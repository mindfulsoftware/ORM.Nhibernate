using System;
using System.Linq; 
using System.Collections.Generic;
using System.Configuration;
using ORM.Nhibernate.Model;
using Xunit;
using NHibernate;
using System.Threading.Tasks;

namespace ORM.Nhibernate.Tests {
    public class EmployeeUpdateTests: BaseTests  {

        public override string ConnectionString {
            get { return ConfigurationManager.ConnectionStrings["cnn"].ConnectionString; }
        }

        [Fact]
        public void SimpleUpdate() {

            string fakeRegion = DateTime.Now.Second.ToString();

            using (var tx = Session.BeginTransaction()) {
                var employee = Session.Load<Employee>(5);
                employee.Region = fakeRegion;   // Forces load from database
                tx.Commit();                    // Automatically flushes changes
            }
        }

        [Fact]
        public void DifferentUsersUpdatingSameVersionedObjectThrows() {
            int employeeId = 5;
            Employee employee = null;
            string fakeRegion = DateTime.Now.Second.ToString();

            var factory = Session.SessionFactory;

            Assert.Throws<NHibernate.StaleObjectStateException>(() => {

                // user1: get employee and display in UI
                var session1 = Session; 
                using (var tx = session1.BeginTransaction()) {
                    employee = Session.Get<Employee>(employeeId);
                    tx.Commit();
                }
                session1.Close();
                session1.Dispose();

                // user2: update employee
                Task.Factory.StartNew((x) => {
                    var sessionT = (x as ISessionFactory).OpenSession();

                    using (var tx = sessionT.BeginTransaction()) {
                        sessionT.Get<Employee>(5).Region = string.Concat(DateTime.Now.Second, "T");
                        tx.Commit();
                    }
                }, factory).Wait();


                // user1: update employee
                employee.Region = fakeRegion;
                var session2 = factory.OpenSession();
                using (var tx = session2.BeginTransaction()) {
                    session2.SaveOrUpdate(employee);
                    tx.Commit();
                }
                session2.Close();
                session2.Dispose();
            });
        }
    }
}
