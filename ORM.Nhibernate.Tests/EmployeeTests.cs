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
    }
}
