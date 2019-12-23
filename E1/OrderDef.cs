using System;
using System.Collections.Generic;

namespace Celin.PO
{
    public enum ProcessSteps
    {
        PENDING,
        OPEN,
        RECEIPT,
        CONFIRM,
        FAILED
    }
    public class OrderDef
    {
        public ProcessSteps? Status { get; set; }
        public string Company { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        public AIS.ErrorWarning[] Errors { get; set; }
        public bool Equals(OrderDef o)
        {
            return Company.Equals(o.Company) && Number.Equals(o.Number) && Type.Equals(o.Type);
        }
        public bool Equals(W4312F.Row r)
        {
            return Company.Equals(r.z_KCOO_12) && Number.Equals(r.z_DOCO_10) && Type.Equals(r.z_DCTO_11);
        }
        public bool Contains(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return true;
            }
            bool found = false;
            foreach (var term in search.ToUpper().Split(' '))
            {
                if (int.TryParse(term, out _))
                {
                    found = found || Number.ToString().StartsWith(term);
                }
                found = found || Company.ToUpper().Contains(term);
                found = found || Type.ToUpper().Contains(term);
            }
            return found;
        }
    }
}
