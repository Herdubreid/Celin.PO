using System.Collections.Generic;

namespace Celin.W43214A
{
    public class Row
    {
        public decimal z_LNID_36 { get; set; }
        public decimal z_UOPN_41 { get; set; }
        public decimal z_AOPN_42 { get; set; }
        public string z_UOM_239 { get; set; }
        public decimal z_UREC_269 { get; set; }
        public string z_LITM_38 { get; set; }
        public string z_MCU_134 { get; set; }
        public string z_KCO_163 { get; set; }
        public int z_DOCO_32 { get; set; }
        public string z_ANI_40 { get; set; }
        public string z_KCOO_108 { get; set; }
        public int z_AN8_146 { get; set; }
        public decimal z_AREC_270 { get; set; }
        public string z_DCT_162 { get; set; }
        public string z_DCTO_33 { get; set; }
        public decimal z_SQOR_282 { get; set; }
        public int z_ITM_37 { get; set; }
        public int z_DOC_161 { get; set; }
    }
    public class Response : AIS.FormResponse
    {
        public AIS.Form<AIS.FormData<Row>> fs_P43214_W43214A { get; set; }
    }
    public class Request : AIS.FormRequest
    {
        public Request(string doc)
        {
            outputType = "GRID_DATA";
            formName = "P43214_W43214A";
            version = "ZJDE0001";
            returnControlIDs = "1";
            query = new AIS.Query
            {
                matchType = AIS.Query.MATCH_ALL,
                autoFind = true,
                condition = new List<AIS.Condition>
                {
                    new AIS.Condition
                    {
                        controlId = "1[161]",
                        @operator = AIS.Condition.EQUAL,
                        value = new AIS.Value[]
                        {
                            new AIS.Value
                            {
                                content = doc,
                                specialValueId = AIS.Value.LITERAL
                            }
                        }
                    }
                }
            };
        }
    }
}
