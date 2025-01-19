using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolCreditBanking.Models
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column("accountnumber")]
        public string AccountNumber { get; set; } = string.Empty;

        [Column("cardnumber")]
        public string? CardNumber { get; set; }

        [Column("balance")]
        public decimal Balance { get; set; }

        // 1. Kolekcja transakcji, w których to konto jest "Account" (źródło)
        [InverseProperty("Account")]
        public virtual ICollection<Transaction> Transactions { get; set; }

        // 2. Kolekcja transakcji, w których to konto jest "DestinationAccount" (docelowe)
        [InverseProperty("DestinationAccount")]
        public virtual ICollection<Transaction> TransactionsAsDestination { get; set; }

        public Account()
        {
            // Zawsze dobrze zainicjalizować kolekcje:
            Transactions = new HashSet<Transaction>();
            TransactionsAsDestination = new HashSet<Transaction>();
        }
    }
}
