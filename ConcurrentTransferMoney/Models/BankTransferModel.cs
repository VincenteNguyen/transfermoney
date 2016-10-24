namespace ConcurrentTransferMoney.Models
{
    public class BankTransferModel
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}