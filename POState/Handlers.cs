using BlazorState;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Celin.PO
{
    public partial class POState
    {
        public class LoadHandler : ActionHandler<LoadAction>
        {
            POState State => Store.GetState<POState>();
            public override Task<Unit> Handle(LoadAction aAction, CancellationToken aCancellationToken)
            {
                State.OpenRequest = aAction.Open;

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public LoadHandler(IStore store) : base(store) { }
        }
        public class OpenRequestHandler : ActionHandler<OpenRequestAction>
        {
            AIS.Server E1 { get; }
            POState State => Store.GetState<POState>();
            public override async Task<Unit> Handle(OpenRequestAction aAction, CancellationToken aCancellationToken)
            {
                State.OpenRequest = await E1.RequestAsync<W4312F.Response>(new W4312F.Request());

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public OpenRequestHandler(IStore store, AIS.Server e1) : base(store)
            {
                E1 = e1;
            }
        }
        public class DemoRequestHandler : ActionHandler<DemoRequestAction>
        {
            AIS.Server E1 { get; }
            POState State => Store.GetState<POState>();
            public override async Task<Unit> Handle(DemoRequestAction aAction, CancellationToken aCancellationToken)
            {
                State.DemoRequest = await E1.RequestAsync<W4312F.Response>(new W4312F.Request(true));

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public DemoRequestHandler(IStore store, AIS.Server e1) : base(store)
            {
                E1 = e1;
            }
        }
    }
}
