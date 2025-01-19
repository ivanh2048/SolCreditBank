using System.ComponentModel.DataAnnotations;

namespace SolCreditBanking.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CardNumber { get; set; }

        public decimal Balance { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
