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
                return View(transaction);
            }

            // Znajdź konto źródłowe
            var sourceAccount = _context.Accounts.FirstOrDefault(a => a.Id == transaction.AccountId);
            if (sourceAccount == null)
            {
                ModelState.AddModelError("", "Konto źródłowe nie istnieje.");
                return View(transaction);
            }

            // Znajdź konto docelowe
            var destinationAccount = _context.Accounts.FirstOrDefault(a => a.Id == transaction.DestinationAccountId);
            if (destinationAccount == null)
            {
                ModelState.AddModelError("", "Konto docelowe nie istnieje.");
                return View(transaction);
            }

            // Sprawdzenie środków na koncie źródłowym
            if (sourceAccount.Balance < transaction.Amount)
            {
                ModelState.AddModelError("", "Brak wystarczających środków na koncie źródłowym.");
                return View(transaction);
            }

            // Aktualizacja salda kont
            sourceAccount.Balance -= transaction.Amount;
            destinationAccount.Balance += transaction.Amount;

            // Zapis transakcji
            transaction.Date = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Users"); // Przekierowanie po sukcesie
        }
    }
}
