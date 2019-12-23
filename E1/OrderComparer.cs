using System;
using System.Collections.Generic;

namespace Celin.PO
{
    public class OrderComparer : IEqualityComparer<OrderDef>
    {
        public bool Equals(OrderDef x, OrderDef y)
        {
            return x.Company.Equals(y.Company) && x.Number.Equals(y.Number) && x.Type.Equals(y.Type);
        }

        public int GetHashCode(OrderDef obj)
        {
            return HashCode.Combine(obj.Company, obj.Number, obj.Type);
        }
    }
}
