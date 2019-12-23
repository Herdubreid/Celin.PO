namespace Celin
{
    public class ReceiptProcess
    {
        public W4312F.Response Open { get; set; }
        public W4312A.Response Receipt { get; set; }
        public W4312A.Response Confirm { get; set; }
        public W4312F.Response Close { get; set; }
    }
}
