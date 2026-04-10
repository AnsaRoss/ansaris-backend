using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain
{
    public enum InvoiceStatus
    {
        Draft = 0,
        Pending = 1,
        Paid = 2,
        Cancelled = 3
    }
    public enum InvoiceType
    {
        Sale = 1,
        Purchase = 2
    }
    public enum AccountType
    {
        Asset = 1,       // Activo
        Liability = 2,   // Pasivo
        Income = 3,      // Ingreso
        Expense = 4      // Gasto
    }

    public enum InventoryMovementType
    {
        Inbound = 1,
        Outbound = 2,
        Adjustment = 3,
        Reservation = 4,
        Release = 5
    }

    public enum InventoryTransferStatus
    {
        Pending = 1,
        Completed = 2,
        Cancelled = 3
    }
}
