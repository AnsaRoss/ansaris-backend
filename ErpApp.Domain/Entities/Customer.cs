using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }  // Nombre o razón social
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Opcional: si querés tener relación inversa
        //public ICollection<SalesOrder> SalesOrders { get; set; }
    }

}
