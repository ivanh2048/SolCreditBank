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
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Id == userId.Value);
            if (account == null)
            {
                ViewBag.ErrorMessage = "Konto użytkownika nie zostało znalezione.";
                return View();
            }

            var transactions = _context.Transactions
                .Where(t => t.AccountId == account.Id || t.DestinationCardNumber == account.CardNumber)
                .OrderByDescending(t => t.Date)
                .ToList();

            ViewBag.Account = account;
            ViewBag.Transactions = transactions;

            var email = HttpContext.Session.GetString("UserEmail") ?? "Nieznany użytkownik";
            ViewBag.UserEmail = email;

            return View();
        }

        [HttpPost]
        public IActionResult BlockMyAccount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var user = _context.Users.Find(userId.Value);
            if (user != null && !user.IsBlocked)
            {
                user.IsBlocked = true;
                _context.SaveChanges();
            }

            // Po zablokowaniu, user nie będzie mógł się logować
            // ewentualnie od razu wylogowujemy
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Authentication");
        }
    }
}
