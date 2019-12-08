using BlazorState;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celin.PO
{
    public partial class POState : State<POState>
    {
        public event EventHandler Changed;
        public W4312F.Response DemoRequest { get; set; }
        public W4312F.Response OpenRequest { get; set; }
        public override void Initialize() { }
    }
}
