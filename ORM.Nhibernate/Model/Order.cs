using System;

namespace ORM.Nhibernate.Model {
    public class Order {
        public virtual int OrderId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual string CustomerId { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual DateTime RequiredDate { get; set; }
        public virtual DateTime? ShippedDate { get; set; }
        public virtual Shipper ShipVia { get; set; }
        public virtual decimal Freight { get; set; }
        public virtual string ShipName { get; set; }
        public virtual string ShipCity { get; set; }
        public virtual string ShipAddress { get; set; }
        public virtual string ShipRegion { get; set; }
        public virtual string ShipPostalCode { get; set; }
        public virtual string ShipCountry { get; set; }

        public override string ToString() {
            return string.Format("Order (OrderId: {0})", OrderId);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Order o = (Order)obj;
            return
                (Employee == o.Employee) &&
                (CustomerId == o.CustomerId) &&
                (OrderDate == o.OrderDate);
        }

        public override int GetHashCode() {

            // Jon Skeet's recommended implementation
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked 
            {
                int hash = 17;

                hash = hash * 23 + (null == Employee ? 
                    new Employee().GetHashCode() : 
                    Employee.GetHashCode());

                hash = hash * 23 + (string.IsNullOrEmpty(CustomerId) ? 
                    string.Empty.GetHashCode() : 
                    CustomerId.GetHashCode());

                hash = hash * 23 + OrderDate.GetHashCode();

                return hash;
            }
        }
    }
}
