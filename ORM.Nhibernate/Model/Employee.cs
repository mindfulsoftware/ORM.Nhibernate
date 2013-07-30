using System;
using System.Collections.Generic;

namespace ORM.Nhibernate.Model {
    public class Employee {
        public virtual int EmployeeId {get; set; }
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
        public virtual string Photo {get; set;} 
        public virtual string Notes {get; set;} 
        public virtual string ReportsTo {get; set;}
        public virtual string PhotoPath { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public Employee() {
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
    }
}
