namespace ConcurrentTransferMoney.Models
{
    public class BankTransferModel
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public int Amount { get; set; }
    }
}