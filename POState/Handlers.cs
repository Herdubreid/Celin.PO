using Blazor.Extensions.Storage.Interfaces;
using BlazorState;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Celin.PO
{
    public partial class POState
    {
        public class UpdateAdjustmentHandler : ActionHandler<UpdateAdjustmentAction>
        {
            ILocalStorage Local { get; }
            POState State => Store.GetState<POState>();
            public override Task<Unit> Handle(UpdateAdjustmentAction aAction, CancellationToken aCancellationToken)
            {
                var row = State.OpenLines.Find(l => l.Equals(aAction.Line));
                row.Adjustment = aAction.Adjustment;

                Local.SetItem(StorageKeys.OPEN_LINES.ToString(), State.OpenLines);

                return Unit.Task;
            }
            public UpdateAdjustmentHandler(IStore store, ILocalStorage local) : base(store)
            {
                Local = local;
            }
        }
        public class UpdateStatusHandler : ActionHandler<UpdateStatusAction>
        {
            ILocalStorage Local { get; }
            POState State => Store.GetState<POState>();
            public override Task<Unit> Handle(UpdateStatusAction aAction, CancellationToken aCancellationToken)
            {
                var order = State.Orders.Find(o => o.Equals(aAction.Order));
                order.Status = aAction.Order.Status;

                Local.SetItem(StorageKeys.ORDERS.ToString(), State.Orders);

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Task;
            }
            public UpdateStatusHandler(IStore store, ILocalStorage local) : base(store)
            {
                Local = local;
            }
        }
        public class ReceiptOrderHandler : ActionHandler<ReceiptOrderAction>
        {
            ILocalStorage Local { get; }
            AIS.Server E1 { get; }
            POState State => Store.GetState<POState>();
            public override async Task<Unit> Handle(ReceiptOrderAction aAction, CancellationToken aCancellationToken)
            {
                EventHandler handler = State.Changed;
                State.ErrorMessage = null;
                try
                {
                    var last = await E1.RequestAsync<AIS.FormResponse>(
                        new AIS.StackFormRequest
                        {
                            formRequest = new W4312F.Request
                            {
                                findOnEntry = "FALSE"
                            },
                            action = AIS.StackFormRequest.open
                        });
                    foreach (var order in aAction.Orders)
                    {
                        AIS.ErrorWarning[] errors = null;
                        var lines = State.OpenLines.Where(l => order.Equals(l));
                        var rp = new ReceiptProcess
                        {
                            Open = await E1.RequestAsync<W4312F.Response>(
                                new ActionRequest(last,
                                    new AIS.ActionRequest
                                    {
                                        formActions = new List<AIS.Action>
                                        {
                                        new AIS.FormAction
                                        {
                                            controlID = "63",
                                            command = AIS.FormAction.SetControlValue,
                                            value = order.Company
                                        },
                                        new AIS.FormAction
                                        {
                                            controlID = "7",
                                            command = AIS.FormAction.SetControlValue,
                                            value = order.Number.ToString()
                                        },
                                        new AIS.FormAction
                                        {
                                            controlID = "9",
                                            command = AIS.FormAction.SetControlValue,
                                            value = order.Type
                                        },
                                        new AIS.FormAction
                                        {
                                            controlID = "21",
                                            command = AIS.FormAction.DoAction
                                        }
                                        }
                                    }))
                        };

                        State.Orders.Find(o => o.Equals(order)).Status = ProcessSteps.OPEN;
                        handler?.Invoke(State, null);

                        if (rp.Open.fs_P4312_W4312F.data.gridData.summary.records > 0)
                        {
                            rp.Receipt = await E1.RequestAsync<W4312A.Response>(
                                new W4312A.ActionRequest(rp.Open,
                                    new AIS.ActionRequest
                                    {
                                        formActions = new List<AIS.Action>
                                        {
                                        new AIS.FormAction
                                        {
                                            controlID = "4",
                                            command = AIS.FormAction.DoAction
                                        }
                                        }
                                    }));

                            State.Orders.Find(o => o.Equals(order)).Status = ProcessSteps.RECEIPT;
                            handler?.Invoke(State, null);

                            rp.Confirm = await E1.RequestAsync<W4312A.Response>(
                                new W4312A.ActionRequest(rp.Receipt,
                                    new AIS.ActionRequest
                                    {
                                        stopOnWarning = "FALSE",
                                        formActions = new List<AIS.Action>
                                        {
                                    new AIS.GridAction
                                    {
                                        gridAction = new AIS.GridUpdate
                                        {
                                            gridID = "1",
                                            gridRowUpdateEvents = rp.Receipt
                                            .fs_P4312_W4312A
                                            .data
                                            .gridData
                                            .rowset
                                            .Select((r, i) =>
                                            {
                                                var row = lines.SingleOrDefault(l => l.z_LNID_43 == r.z_LNID_44);
                                                return new AIS.RowEvent
                                                {
                                                    rowNumber = i,
                                                    gridColumnEvents = row != null
                                                    ? new List<AIS.ColumnEvent>
                                                    {
                                                        new AIS.ColumnEvent
                                                        {
                                                            columnID = "116",
                                                            command = AIS.GridAction.SetGridCellValue,
                                                            value = (row.Adjustment ?? row.z_UOPN_16).ToString()
                                                        },
                                                        new AIS.ColumnEvent
                                                        {
                                                            columnID = "117",
                                                            command = AIS.GridAction.SetGridCellValue,
                                                            value = "0"
                                                        },
                                                        new AIS.ColumnEvent
                                                        {
                                                            columnID = "382",
                                                            command = AIS.GridAction.SetGridCellValue,
                                                            value = "1"
                                                        }
                                                    }
                                                    : new List<AIS.ColumnEvent>
                                                    {
                                                        new AIS.ColumnEvent
                                                        {
                                                            columnID = "382",
                                                            command = AIS.GridAction.SetGridCellValue,
                                                            value = "1"
                                                        }
                                                    }
                                                };
                                            })
                                            .Where(r => r.gridColumnEvents != null)
                                            .ToList()
                                        }
                                    },
                                    new AIS.FormAction
                                    {
                                        controlID = "4",
                                        command = AIS.FormAction.DoAction
                                    }
                                        }
                                    }));

                            if (rp.Confirm.fs_P43291_W43291A != null || rp.Confirm.fs_P4312_W4312F != null)
                            {
                                State.Orders.Find(o => o.Equals(order)).Status = ProcessSteps.CONFIRM;
                                handler?.Invoke(State, null);

                                AIS.FormResponse success = rp.Confirm;
                                if (rp.Confirm.fs_P43291_W43291A != null)
                                {
                                    success = await E1.RequestAsync<AIS.FormResponse>(
                                        new ActionRequest(rp.Confirm,
                                            new AIS.ActionRequest
                                            {
                                                formActions = new List<AIS.Action>
                                                {
                                                new AIS.FormAction
                                                {
                                                    controlID = "4",
                                                    command = AIS.FormAction.DoAction
                                                }
                                                }
                                            }));
                                }
                                rp.Close = await E1.RequestAsync<W4312F.Response>(
                                    new ActionRequest(success,
                                        new AIS.ActionRequest
                                        {
                                            formActions = new List<AIS.Action>
                                            {
                                            new AIS.FormAction
                                            {
                                                controlID = "21",
                                                command = AIS.FormAction.DoAction
                                            }
                                            }
                                        }));

                                last = rp.Close;

                                var open = rp.Close.fs_P4312_W4312F.data.gridData.rowset;
                                State.OpenLines = State
                                    .OpenLines
                                    .Select(l =>
                                        order
                                        .Equals(l)
                                            ? open.SingleOrDefault(r => r.z_LNID_43.Equals(l.z_LNID_43))
                                            : l)
                                    .Where(l => l != null)
                                    .ToList();

                                if (open.Length > 0)
                                {
                                    State.Orders.Find(o => o.Equals(order)).Status = null;
                                }
                                else
                                {
                                    State.Orders.RemoveAt(State.Orders.FindIndex(o => o.Equals(order)));
                                }

                                var receipts = await E1.RequestAsync<W43214A.Response>(
                                    new W43214A.Request(rp.Close
                                    .fs_P4312_W4312F
                                    .data
                                    .z_DOCR_97
                                    .value));
                                State.Receipts.AddRange(receipts
                                    .fs_P43214_W43214A
                                    .data
                                    .gridData
                                    .rowset
                                    .Where(r => r.z_LNID_36 > 0));

                            }
                            else
                            {
                                errors = rp.Confirm.fs_P4312_W4312A.errors;
                                last = await E1.RequestAsync<AIS.FormResponse>(
                                    new ActionRequest(rp.Confirm,
                                        new AIS.ActionRequest
                                        {
                                            formActions = new List<AIS.Action>
                                            {
                                                new AIS.FormAction
                                                {
                                                    controlID = "5",
                                                    command = AIS.FormAction.DoAction
                                                }
                                            }
                                        }));
                            }
                        }
                        else
                        {
                            errors = new[]
                            {
                            new AIS.ErrorWarning
                            {
                                TITLE = "Zero Lines",
                                MOBILE = "No Lines to receipt!"
                            }
                        };
                            last = rp.Open;
                        }

                        var done = State.Orders.Find(o => order.Equals(o));
                        if (done != null)
                        {
                            done.Errors = errors;
                            if (errors != null)
                            {
                                done.Status = ProcessSteps.FAILED;
                            }
                        }
                        State.ReceiptProcess = rp;
                    }
                    await E1.RequestAsync<AIS.FormResponse>(
                        new ActionRequest(last, AIS.StackFormRequest.close));
                }
                catch (AIS.HttpWebException e)
                {
                    State.ErrorMessage = e.ErrorResponse.message;
                }
                catch (Exception e)
                {
                    State.ErrorMessage = e.Message;
                }

                handler?.Invoke(State, null);

                await Local.SetItem(StorageKeys.ORDERS.ToString(), State.Orders);
                await Local.SetItem(StorageKeys.OPEN_LINES.ToString(), State.OpenLines);
                await Local.SetItem(StorageKeys.RECEIPTS.ToString(), State.Receipts);

                return Unit.Value;
            }
            public ReceiptOrderHandler(IStore store, ILocalStorage local, AIS.Server e1) : base(store)
            {
                E1 = e1;
                Local = local;
            }
        }
        public class SaveHandler : ActionHandler<SaveAction>
        {
            ILocalStorage Local { get; }
            POState State => Store.GetState<POState>();
            public override Task<Unit> Handle(SaveAction aAction, CancellationToken aCancellationToken)
            {
                switch (aAction.StorageKey)
                {
                    case StorageKeys.ORDERS:
                        Local.SetItem(StorageKeys.ORDERS.ToString(), State.Orders);
                        break;
                    case StorageKeys.OPEN_LINES:
                        Local.SetItem(StorageKeys.OPEN_LINES.ToString(), State.OpenLines);
                        break;
                    case StorageKeys.RECEIPTS:
                        Local.SetItem(StorageKeys.RECEIPTS.ToString(), State.Receipts);
                        break;
                }

                return Unit.Task;
            }
            public SaveHandler(IStore store, ILocalStorage local) : base(store)
            {
                Local = local;
            }
        }
        public class LoadHandler : ActionHandler<LoadAction>
        {
            ILocalStorage Local { get; }
            POState State => Store.GetState<POState>();
            public override async Task<Unit> Handle(LoadAction aAction, CancellationToken aCancellationToken)
            {
                State.Orders.Clear();
                var orders = await Local.GetItem<IEnumerable<OrderDef>>(StorageKeys.ORDERS.ToString())
                    ?? Enumerable.Empty<OrderDef>();
                State.Orders.AddRange(orders);
                State.OpenLines.Clear();
                var openLines = await Local.GetItem<IEnumerable<W4312F.Row>>(StorageKeys.OPEN_LINES.ToString())
                    ?? Enumerable.Empty<W4312F.Row>();
                State.OpenLines.AddRange(openLines);
                State.Receipts.Clear();
                var receipts = await Local.GetItem<IEnumerable<W43214A.Row>>(StorageKeys.RECEIPTS.ToString())
                    ?? Enumerable.Empty<W43214A.Row>();
                State.Receipts.AddRange(receipts);

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public LoadHandler(IStore store, ILocalStorage local) : base(store)
            {
                Local = local;
            }
        }
        public class OpenRequestHandler : ActionHandler<OpenRequestAction>
        {
            AIS.Server E1 { get; }
            ILocalStorage Local { get; }
            POState State => Store.GetState<POState>();
            public override async Task<Unit> Handle(OpenRequestAction aAction, CancellationToken aCancellationToken)
            {
                State.ErrorMessage = null;
                try
                {
                    var open = await E1.RequestAsync<W4312F.Response>(new W4312F.Request());

                    State.OpenLines.Clear();
                    State.OpenLines.AddRange(open
                        .fs_P4312_W4312F
                        .data
                        .gridData
                        .rowset);

                    State.Orders.Clear();
                    State.Orders.AddRange(open
                        .fs_P4312_W4312F
                        .data
                        .gridData
                        .rowset
                        .Select(r => new OrderDef { Company = r.z_KCOO_12, Number = r.z_DOCO_10, Type = r.z_DCTO_11 })
                        .Distinct(new OrderComparer()));

                    await Local.SetItem(StorageKeys.OPEN_LINES.ToString(), State.OpenLines);
                    await Local.SetItem(StorageKeys.ORDERS.ToString(), State.Orders);
                }
                catch (AIS.HttpWebException e)
                {
                    State.ErrorMessage = e.ErrorResponse.message;
                }
                catch (Exception e)
                {
                    State.ErrorMessage = e.Message;
                }

                EventHandler handler = State.Changed;
                handler?.Invoke(State, null);

                return Unit.Value;
            }
            public OpenRequestHandler(IStore store, ILocalStorage local, AIS.Server e1) : base(store)
            {
                E1 = e1;
                Local = local;
            }
        }
        public class DemoRequestHandler : ActionHandler<DemoRequestAction>
        {
            AIS.Server E1 { get; }
            POState State => Store.GetState<POState>();
            public override async Task<Unit> Handle(DemoRequestAction aAction, CancellationToken aCancellationToken)
            {
                State.ErrorMessage = null;
                try
                {
                    State.DemoResponse = await E1.RequestAsync<JsonElement>(
                        new AIS.FormRequest
                        {
                            formName = aAction.FormName,
                            formServiceDemo = "TRUE"
                        });
                }
                catch (AIS.HttpWebException e)
                {
                    State.ErrorMessage = e.ErrorResponse.message;
                }
                catch (Exception e)
                {
                    State.ErrorMessage = e.Message;
                }

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
