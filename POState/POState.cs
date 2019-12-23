using BlazorState;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Celin.PO
{
    public enum StorageKeys
    {
        ORDERS,
        OPEN_LINES,
        RECEIPTS
    }
    public partial class POState : State<POState>
    {
        public event EventHandler Changed;
        public string ErrorMessage { get; set; }
        public ReceiptProcess ReceiptProcess { get; set; }
        public List<OrderDef> Orders { get; set; }
        public List<W4312F.Row> OpenLines { get; set; }
        public List<W43214A.Row> Receipts { get; set; }
        public JsonElement? DemoResponse { get; set; }
        public override void Initialize()
        {
            OpenLines = new List<W4312F.Row>();
            Orders = new List<OrderDef>();
            Receipts = new List<W43214A.Row>();
        }
    }
}
