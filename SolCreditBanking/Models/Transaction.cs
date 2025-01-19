using SolCreditBanking.Models;
public class Transaction
{
    public int Id { get; set; }

    // Konto źródłowe (z którego wychodzą pieniądze)
    public int AccountId { get; set; }
    public virtual Account SourceAccount { get; set; }

    // Konto docelowe (na które trafiają pieniądze)
    public int DestinationAccountId { get; set; }
    public virtual Account DestinationAccount { get; set; }

    // Kwota przelewu
    public decimal Amount { get; set; }

    // Data transakcji
    public DateTime Date { get; set; }
}
