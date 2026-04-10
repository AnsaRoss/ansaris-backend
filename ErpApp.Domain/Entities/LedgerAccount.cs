using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Entities
{
    public class LedgerAccount
    {
        public Guid Id { get; set; }
        public string Code { get; set; } // 1001, 2001, etc.
        public string Name { get; set; } // Caja, Banco, Ventas, etc.
        public AccountType Type { get; set; } // Activo, Pasivo, Ingreso, Gasto, etc.
    }

}
