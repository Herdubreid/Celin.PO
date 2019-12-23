using System;
using System.Collections.Generic;

namespace Celin.W4312F
{
    public class Row
    {
        public string z_AITM_42 { get; set; }
        public string z_CRCD_38 { get; set; }
        public decimal z_AOPN_17 { get; set; }
        public DateTime z_TRDJ_90 { set; get; }
        public int z_AN8_14 { get; set; }
        public string  z_DCTO_11 { get; set; }
        public string  z_SFXO_40 { get; set; }
        public int z_SHPN_114 { get; set; }
        public string z_KCOO_12 { get; set; }
        public DateTime z_PDDJ_92 { get; set; }
        public int z_ITM_13 { get; set; }
        public DateTime z_DRQJ_91 { get; set; }
        public string z_ANI_26 { get; set; }
        public string z_BCRC_79 { get; set; }
        public string z_UOM_87 { get; set; }
        public decimal z_LNID_43 { get; set; }
        public decimal z_FAP_39 { get; set; }
        public int z_DOCO_10 { get; set; }
        public string z_MCU_34 { get; set; }
        public int z_SHAN_15 { get; set; }
        public string z_LITM_41 { get; set; }
        public decimal z_UOPN_16 { get; set; }
        public decimal? Adjustment { get; set; }         
        public string z_DSC1_25 { get; set; }
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
                    found = found || z_DOCO_10.ToString().StartsWith(term);
                    found = found || z_AN8_14.ToString().StartsWith(term);
                }
                found = found || z_DCTO_11.ToUpper().Contains(term);
                found = found || z_DSC1_25.ToUpper().Contains(term);
            }
            return found;
        }
        public bool Equals(Row r)
        {
            return z_DOCO_10.Equals(r.z_DOCO_10)
                && z_LNID_43.Equals(r.z_LNID_43)
                && z_DCTO_11.Equals(r.z_DCTO_11)
                && z_KCOO_12.Equals(r.z_KCOO_12);
        }
    }
    public class Form : AIS.FormData<Row>
    {
         public AIS.FormField<string> z_DOCR_97 { get; set; }
         public AIS.FormField<string> z_ANI_23 { get; set; }
         public AIS.FormField<string> z_UITM_20 { get; set; }
         public AIS.FormField<string> z_EV01_64 { get; set; }
         public AIS.FormField<string> z_KCOO_63 { get; set; }
         public AIS.FormField<string> z_MCU_37 { get; set; }
         public AIS.FormField<int> z_DOCO_7 { get; set; }
         public AIS.FormField<string> z_DCTO_9 { get; set; }
         public AIS.FormField<int> z_DOC_85 { get; set; }
    }
    public class Response : AIS.FormResponse
    {
        public AIS.Form<Form> fs_P4312_W4312F { get; set; }
    }
    public class Request : AIS.FormRequest
    {
        public Request(string company, int number, string type) : this()
        {
            findOnEntry = "FALSE";
            query = new AIS.Query
            {
                matchType = AIS.Query.MATCH_ALL,
                autoFind = true,
                condition = new List<AIS.Condition>
                {
                    new AIS.Condition
                    {
                        controlId = "1[10]",
                        @operator = AIS.Condition.EQUAL,
                        value = new AIS.Value[]
                        {
                            new AIS.Value
                            {
                                content = number.ToString(),
                                specialValueId = AIS.Value.LITERAL
                            }
                        }
                    },
                    new AIS.Condition
                    {
                        controlId = "1[11]",
                        @operator = AIS.Condition.EQUAL,
                        value = new AIS.Value[]
                        {
                            new AIS.Value
                            {
                                content = type,
                                specialValueId = AIS.Value.LITERAL
                            }
                        }
                    },
                    new AIS.Condition
                    {
                        controlId = "1[12]",
                        @operator = AIS.Condition.EQUAL,
                        value = new AIS.Value[]
                        {
                            new AIS.Value
                            {
                                content = company,
                                specialValueId = AIS.Value.LITERAL
                            }
                        }
                    }
                }
            };
        }
        public Request()
        {
            outputType = "GRID_DATA";
            formName = "P4312_W4312F";
            version = "ZJDE0004";
            findOnEntry = "TRUE";
        }
    }
}
