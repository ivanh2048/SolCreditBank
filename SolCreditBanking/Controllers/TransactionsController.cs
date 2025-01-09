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
        public IActionResult Create(int accountId)
        {

            var model = new Transaction
            {
                AccountId = accountId
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return View(transaction);
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Id == transaction.AccountId);
            if (account == null)
            {
                ModelState.AddModelError("", "Konto nie istnieje.");
                return View(transaction);
            }

            transaction.Date = DateTime.UtcNow;

            if (transaction.TransactionType == "Deposit")
            {
                account.Balance += transaction.Amount;
            }
            else if (transaction.TransactionType == "Withdrawal")
            {
                if (account.Balance >= transaction.Amount)
                {
                    account.Balance -= transaction.Amount;
                }
                else
                {
                    ModelState.AddModelError("", "Niewystarczające środki na koncie.");
                    return View(transaction);
                }
            }
            else
            {
                ModelState.AddModelError("", "Nieznany typ transakcji.");
                return View(transaction);
            }

            _context.Transactions.Add(transaction);
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Users");
        }
    }
}
