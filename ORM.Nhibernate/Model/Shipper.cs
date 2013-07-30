using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Nhibernate.Model {
    public class Shipper {
        public virtual int ShipperId { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Phone { get; set; }
    }
}
