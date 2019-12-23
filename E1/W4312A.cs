using System;

namespace Celin.W4312A
{
    public class Row
    {
        public int z_UOPNR_116 { get; set; }
        public string z_MCU_36 { get; set; }
        public string z_LITM_103 { get; set; }
        public string z_DSC1_40 { get; set; }
        public string z_DSC2_41 { get; set; }
        public string z_UOM_54 { get; set; }
        public decimal z_LNID_44 { get; set; }
        public decimal z_UCSTR_119 { get; set; }
        public decimal z_AOPNR_117 { get; set; }
        public string z_RECOPT_382 { get; set; }
    }
    public class Form : AIS.FormData<Row>
    {
        public AIS.FormField<string> z_KCOO_11 { get; set; }
        public AIS.FormField<int> z_AN8_15 { get; set; }
        public AIS.FormField<DateTime> z_RCDJ_19 { get; set; }
        public AIS.FormField<DateTime> z_DGL_25 { get; set; }
        public AIS.FormField<int> z_ICU_27 { get; set; }
        public AIS.FormField<int> z_DOC_157 { get; set; }
        public AIS.FormField<DateTime> z_DGL_550 { get; set; }
    }
    public class Response : AIS.FormResponse
    {
        public AIS.Form<Form> fs_P4312_W4312A { get; set; }
        public AIS.Form<AIS.FormData<AIS.Row>> fs_P43291_W43291A { get; set; }
        public AIS.Form<Form> fs_P4312_W4312F { get; set; }
    }
    public class ActionRequest : Celin.ActionRequest
    {
        public ActionRequest(AIS.FormResponse form, AIS.ActionRequest action)
            : base(form, action)
        {
            actionRequest.returnControlIDs = "11|15|19|25|27|157|550|1[382,103,116,54,119,117,40,41,36,44]";
        }
    }
}
