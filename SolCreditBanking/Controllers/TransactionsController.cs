using Microsoft.AspNetCore.Mvc;
using SolCreditBanking.Data;
using SolCreditBanking.Models;
using System;
using System.Linq;

namespace SolCreditBanking.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return View(transaction); // Powrót do formularza
            }

            // Konto źródłowe (zalogowany użytkownik)
            var sourceAccount = _context.Accounts.FirstOrDefault(a => a.Id == transaction.AccountId);
            if (sourceAccount == null)
            {
                ModelState.AddModelError("", "Konto źródłowe nie istnieje.");
                return View(transaction);
            }

            // Konto docelowe (na podstawie numeru karty)
            var targetAccount = _context.Accounts.FirstOrDefault(a => a.CardNumber == transaction.DestinationCardNumber);
            if (targetAccount == null)
            {
                ModelState.AddModelError("", "Nie znaleziono konta docelowego.");
                return View(transaction);
            }

            // Sprawdzenie środków
            if (sourceAccount.Balance < transaction.Amount)
            {
                ModelState.AddModelError("", "Niewystarczające środki na koncie.");
                return View(transaction);
            }

            // Aktualizacja salda
            sourceAccount.Balance -= transaction.Amount;
            targetAccount.Balance += transaction.Amount;

            // Zapis transakcji
            transaction.TransactionType = "Transfer";
            transaction.Date = DateTime.UtcNow;

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            // Przekierowanie do dashboardu
            return RedirectToAction("Dashboard", "Users");
        }

    }
}
