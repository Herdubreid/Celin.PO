using BlazorState;
using System.Collections.Generic;

namespace Celin.PO
{
    public partial class POState
    {
        public class UpdateAdjustmentAction : IAction
        {
            public W4312F.Row Line { get; set; }
            public decimal? Adjustment { get; set; }
        }
        public class UpdateStatusAction : IAction
        {
            public OrderDef Order { get; set; }
        }
        public class ReceiptOrderAction : IAction
        {
            public IEnumerable<OrderDef> Orders { get; set; }
        }
        public class SaveAction : IAction
        {
            public StorageKeys StorageKey { get; set; }
        }
        public class LoadAction : IAction { }
        public class OpenRequestAction : IAction { }
        public class DemoRequestAction : IAction
        {
            public string FormName { get; set; }
        }
    }
}
