using Microsoft.AspNetCore.Mvc;
using SolCreditBanking.Data;
using SolCreditBanking.Models;
using System;
using System.Linq;

namespace SolCreditBanking.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista wszystkich userów w systemie.
        /// Tylko admin ma do niej dostęp.
        /// </summary>
        [HttpGet]
        public IActionResult UserManagement()
        {
            // Sprawdzamy, czy zalogowany jest admin
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Forbid();
                // lub return RedirectToAction("Dashboard", "Users");
            }

            // Pobieramy wszystkich userów z bazy
            var allUsers = _context.Users.ToList();
            return View(allUsers);
            // => Views/Admin/UserManagement.cshtml
        }

        /// <summary>
        /// Zmienia IsBlocked na true (blokada usera).
        /// </summary>
        [HttpPost]
        public IActionResult BlockUser(int userId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Forbid();
            }

            var user = _context.Users.Find(userId);
            if (user != null && !user.IsBlocked)
            {
                user.IsBlocked = true;
                _context.SaveChanges();
            }
            return RedirectToAction("UserManagement");
        }

        /// <summary>
        /// Odblokowuje usera (IsBlocked = false).
        /// </summary>
        [HttpPost]
        public IActionResult UnblockUser(int userId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Forbid();
            }

            var user = _context.Users.Find(userId);
            if (user != null && user.IsBlocked)
            {
                user.IsBlocked = false;
                _context.SaveChanges();
            }
            return RedirectToAction("UserManagement");
        }

        /// <summary>
        /// Zmiana roli: User ↔ Admin
        /// </summary>
        [HttpPost]
        public IActionResult ChangeRole(int userId, string newRole)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Forbid();
            }

            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.Role = newRole; // np. "Admin" lub "User"
                _context.SaveChanges();
            }
            return RedirectToAction("UserManagement");
        }

        /// <summary>
        /// Reset hasła - generuje nowe hasło i nadpisuje PasswordHash w bazie.
        /// Następnie pokazuje je w widoku.
        /// </summary>
        [HttpPost]
        public IActionResult ResetPassword(int userId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Forbid();
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                // ewentualnie zwróć błąd lub nic
                return RedirectToAction("UserManagement");
            }

            // Generujemy nowe, tymczasowe hasło
            string newTempPass = GenerateRandomPassword(8);
            // Hashujemy
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newTempPass);

            // Zapis w bazie
            _context.SaveChanges();

            // Przekazujemy do widoku np. przez TempData lub ViewBag
            TempData["ResetMessage"] = $"Nowe hasło użytkownika {user.Email} to: {newTempPass}";

            return RedirectToAction("UserManagement");
        }

        /// <summary>
        /// Metoda pomocnicza do generowania losowego hasła.
        /// </summary>
        private string GenerateRandomPassword(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var pass = new char[length];
            for (int i = 0; i < length; i++)
            {
                pass[i] = chars[random.Next(chars.Length)];
            }
            return new string(pass);
        }
    }
}
