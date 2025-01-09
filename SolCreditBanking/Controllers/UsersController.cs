using Microsoft.AspNetCore.Mvc;
using SolCreditBanking.Data;
using SolCreditBanking.Models;
using System.Linq;

namespace SolCreditBanking.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [HttpGet]
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var account = _context.Accounts.FirstOrDefault(a => a.UserId == userId.Value);
            if (account == null)
            {
                ViewBag.ErrorMessage = "Konto użytkownika nie zostało znalezione.";
                return View();
            }

            var transactions = _context.Transactions
                .Where(t => t.AccountId == account.Id)
                .OrderByDescending(t => t.Date)
                .ToList();

            ViewBag.Account = account;
            ViewBag.Transactions = transactions;

            return View();
        }

    }

}
