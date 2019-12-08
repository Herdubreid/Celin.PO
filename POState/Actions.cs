using BlazorState;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celin.PO
{
    public partial class POState
    {
        public class LoadAction : IAction
        {
            public W4312F.Response Open { get; set; }
        }
        public class OpenRequestAction : IAction { }
        public class DemoRequestAction : IAction { }
    }
}
