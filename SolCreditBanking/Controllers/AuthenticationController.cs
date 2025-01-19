using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SolCreditBanking.Data;
using SolCreditBanking.Models;
using System;
using System.Linq;

namespace SolCreditBanking.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Hashowanie hasła
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                // Dodanie użytkownika do bazy
                _context.Users.Add(user);
                _context.SaveChanges();

                // Tworzenie konta z domyślnym saldem
                var newAccount = new Account
                {
                    Id = user.Id,
                    CardNumber = GenerateRandomCardNumber(16),
                    Balance = 100m,
                    
                };

                _context.Accounts.Add(newAccount);
                _context.SaveChanges();

                return RedirectToAction("Login", "Authentication");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Nieprawidłowy email lub hasło.");
                return View(model);
            }

            // Sprawdź czy user jest zablokowany
            if (user.IsBlocked)
            {
                ModelState.AddModelError("", "Twoje konto jest zablokowane. Skontaktuj się z administratorem.");
                return View(model);
            }

            // Walidacja hasła
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Nieprawidłowy email lub hasło.");
                return View(model);
            }

            // Logowanie OK
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email); // Zapis email w sesji
            HttpContext.Session.SetString("UserRole", user.Role); // Zapis roli w sesji

            if (user.Role == "Admin")
            {
                return RedirectToAction("UserManagement", "Admin");
            }

            return RedirectToAction("Dashboard", "Users");
        }


        [HttpGet]
        public IActionResult Logout()
        {
            // Wyczyszczenie całej sesji
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Authentication");
        }

        private string GenerateRandomCardNumber(int number)
        {
            Random random = new Random();
            var digits = new char[16];
            for (int i = 0; i < 16; i++)
            {
                // Losujemy cyfrę (0-9) i zamieniamy na znak
                digits[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(digits);
        }
    }
}
