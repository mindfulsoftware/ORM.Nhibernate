using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Nhibernate.Dto {
    public class EmployeeOrdersByCustomer {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerId { get; set; }
        public int Count { get; set; }

        public override string ToString() {
            return string.Format("EmployeeOrdersByCustomer (FirstName: {0}, LastName: {1} , CustomerId: {2}, Count: {3})", 
                FirstName, LastName, CustomerId, Count);
        }
    }
}
