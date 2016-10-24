using System.ComponentModel.DataAnnotations;

namespace ConcurrentTransferMoney.Entities
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public decimal Balance { get; set; }
    }
}