using System;
using System.Collections.Generic;
using System.Text.Json;

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
         public string z_DSC1_25 { get; set; }
    }
    public class Response : AIS.Response
    {
        public AIS.Form<AIS.FormData<Row>> fs_P4312_W4312F { get; set; }
    }
    public class Request : AIS.FormRequest
    {
        public Request(bool demo = false)
        {
            outputType = "GRID_DATA";
            formName = "P4312_W4312F";
            version = "ZJDE0001";
            formServiceDemo = demo ? "TRUE" : null;
            findOnEntry = "TRUE";
        }
    }
}
