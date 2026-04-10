using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Constants
{
    public static class LedgerAccounts
    {
        public static readonly Guid AccountsReceivable = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid SalesRevenue = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Inventory = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid AccountsPayable = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid CashOrBank = Guid.Parse("55555555-5555-5555-5555-555555555555"); 
    }
}
