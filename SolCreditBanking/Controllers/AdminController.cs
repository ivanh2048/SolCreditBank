using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public IActionResult UserManagement()
        {
            var email = HttpContext.Session.GetString("UserEmail") ?? "Nieznany użytkownik";
            var role = HttpContext.Session.GetString("UserRole") ?? "Brak roli";

            ViewBag.UserEmail = email;
            ViewBag.UserRole = role;

            if (role != "Admin")
            {
                return Forbid();
            }

            // Pobieranie użytkowników
            var allUsers = _context.Users.ToList();
            Console.WriteLine($"Liczba użytkowników w bazie: {allUsers.Count}");

            return View(allUsers);
        }

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
                user.Role = newRole;
                _context.SaveChanges();
            }
            return RedirectToAction("UserManagement");
        }

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
                return RedirectToAction("UserManagement");
            }

            string newTempPass = GenerateRandomPassword(8);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newTempPass);

            _context.SaveChanges();

            TempData["ResetMessage"] = $"Nowe hasło użytkownika {user.Email} to: {newTempPass}";

            return RedirectToAction("UserManagement");
        }

        private string GenerateRandomPassword(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
