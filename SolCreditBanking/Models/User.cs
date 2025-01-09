﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolCreditBanking.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")] 
        public int Id { get; set; }

        [Column("firstname")]
        public string FirstName { get; set; } = string.Empty;

        [Column("lastname")]
        public string LastName { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        public string Password { get; set; } = string.Empty;

        [Column("passwordhash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
