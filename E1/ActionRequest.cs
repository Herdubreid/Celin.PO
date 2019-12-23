using System.Text.RegularExpressions;

namespace Celin
{
    public class ActionRequest : AIS.StackFormRequest
    {
        static readonly Regex FormPat = new Regex(@"(?:^[A-Z|\d]*)_(W[A-Z|\d]*)(?:_[A-Z|\d]*)?");
        public ActionRequest(AIS.FormResponse form, AIS.ActionRequest actionRq)
            : this(form, execute)
        {
            var m = FormPat.Match(form.currentApp);
            actionRequest = actionRq;
            actionRequest.formOID = m.Success ? m.Groups[1].Value : "";
        }
        public ActionRequest(AIS.FormResponse form, string act)
        {
            stackId = form.stackId;
            stateId = form.stateId;
            rid = form.rid;
            action = act;
        }
    }
}
