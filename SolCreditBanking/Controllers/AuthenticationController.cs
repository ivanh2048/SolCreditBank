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

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);


                _context.Users.Add(user);
                _context.SaveChanges();

                var newAccount = new Account
                {
                    UserId = user.Id,
                    AccountNumber = GenerateRandomAccountNumber(16),
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
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                    if (isPasswordValid)
                    {
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        return RedirectToAction("Dashboard", "Users");
                    }
                }
                ModelState.AddModelError("", "Nieprawidłowy email lub hasło.");
            }
            return View(model);
        }

        private string GenerateRandomAccountNumber(int length)
        {
            var random = new Random();
            var digits = new char[length];
            for (int i = 0; i < length; i++)
            {
                digits[i] = (char)('0' + random.Next(10));
            }
            return new string(digits);
        }
    }
}
