using System;
using System.Collections.Generic;

namespace ORM.Nhibernate.Model {
    public class Employee {
        public virtual int EmployeeId {get; set; }
        public virtual int Version { get; set; }
        public virtual string LastName {get; set;}
        public virtual string FirstName {get; set;} 
        public virtual string Title {get; set;} 
        public virtual string TitleOfCourtesy {get; set;} 
        public virtual DateTime BirthDate {get; set;} 
        public virtual DateTime HireDate {get; set;} 
        public virtual string Address {get; set;} 
        public virtual string City {get; set;} 
        public virtual string Region {get; set;} 
        public virtual string PostalCode {get; set;} 
        public virtual string Country {get; set;} 
        public virtual string HomePhone {get; set;} 
        public virtual string Extension {get; set;} 
        public virtual byte[] Photo {get; set;} 
        public virtual string Notes {get; set;} 
        public virtual string ReportsTo {get; set;}
        public virtual string PhotoPath { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public Employee() {
            LastName = FirstName = Title = TitleOfCourtesy = string.Empty;
            Address = City = Region = PostalCode = Country = PostalCode = string.Empty;
            HomePhone = Extension = Notes = ReportsTo = PhotoPath;
            BirthDate = HireDate = DateTime.MinValue;

            Orders = new List<Order>();
        }

        public virtual void AddOrder(Order order) {
            order.Employee = this;
            Orders.Add(order);
        }

        public virtual void RemoveOrder(Order order) {
            order.Employee = null;
            Orders.Remove(order);
        }

        public override string ToString() {
            return string.Format("Employee (EmployeeId: {0}, LastName: {1}, FirstName: {2})", EmployeeId, LastName, FirstName);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Employee e = (Employee)obj;

            return 
                string.Compare(FirstName, e.FirstName, true) == 0 &&
                string.Compare(LastName, e.LastName, true) == 0 &&
                DateTime.Equals(BirthDate, e.BirthDate);
        }

        public override int GetHashCode() {

            // Jon Skeet's recommended implementation
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + (string.IsNullOrEmpty(FirstName) ? 
                    string.Empty.GetHashCode() : 
                    FirstName.GetHashCode());

                hash = hash * 23 + (string.IsNullOrEmpty(LastName) ? 
                    string.Empty.GetHashCode() : 
                    LastName.GetHashCode()); 

                hash = hash * 23 + BirthDate.GetHashCode();

                return hash;
            }
        }
    }
}
