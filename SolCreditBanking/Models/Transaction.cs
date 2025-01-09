using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SolCreditBanking.Models
{
    [Table("transactions")]
    public class Transaction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("accountid")]
        public int AccountId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("transactiontype")]
        public string TransactionType { get; set; } = string.Empty; 

        [Column("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}