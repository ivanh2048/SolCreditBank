using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolCreditBanking.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; } // Konto źródłowe

        [ForeignKey("AccountId")] // Klucz obcy do konta źródłowego
        public virtual Account? Account { get; set; }

        [Required]
        public string? DestinationCardNumber { get; set; } // Numer karty konta docelowego

        [Required]
        public decimal Amount { get; set; }

        public string TransactionType { get; set; } = "Transfer";

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
