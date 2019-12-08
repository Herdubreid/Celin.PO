using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Celin.PO
{
    public class E1Date
    {
        static readonly Regex PAT = new Regex(@"^(\d{4})(\d{2})(\d{2})$");
        public DateTime Value { get; }
        public static explicit operator E1Date(string date)
        {
            var m = PAT.Match(date);
            if (m.Success)
            {
                try
                {
                     return new E1Date(new DateTime(
                        Convert.ToInt32(m.Groups[1].Value),
                        Convert.ToInt32(m.Groups[2].Value),
                        Convert.ToInt32(m.Groups[3].Value)));
                }
                catch { }
            }
            return new E1Date(new DateTime());
        }
        public E1Date(DateTime date) { Value = date; }
        public E1Date(string date) { }
        public E1Date() { }
    }
}
