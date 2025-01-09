using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("accounts")]
public class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userid")]
    public int UserId { get; set; }

    [Column("accountnumber")]
    public string AccountNumber { get; set; }

    [Column("balance")]
    public decimal Balance { get; set; }
}
