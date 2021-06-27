using DataLibrary.DataAccess;
using DataLibrary.Models;
using EasyEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitApp.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;

            if (_context.exercises.Count() == 0)
            {
                string file = System.IO.File.ReadAllText("data.json");
                var exercises = JsonSerializer.Deserialize<List<Exercise>>(file);
                _context.AddRangeAsync(exercises);
                _context.SaveChangesAsync();
            }
        }

        [HttpGet]
        [Route("/users")]
        public async Task<ActionResult<IEnumerable<User>>> Index()
        {
            return await _context.users.ToListAsync();
        }

        [HttpPost]
        [Route("/register")]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            if (user.email == null || user.password == null || user.email == "" || user.password == "")
            {
                return BadRequest();
            }

            var users = await _context.users.ToListAsync();

            if (users.Any(u => user.email == u.email))
            {
                return BadRequest();
            }

            user.password = HashPassword(user.password);

            _context.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<ActionResult> Login([FromBody] LoginUser user)
        {
            if(user.email==null || user.password == null || user.email=="" || user.password == "")
            {
                return BadRequest();
            }

            var userFromDb = _context.users.FirstOrDefault(u => u.email == user.email);

            if(userFromDb == null)
            {
                return BadRequest();
            }

            if (PasswordsMatch(user.password, userFromDb.password))
            {
                var tempUser = new UserToReturn();

                tempUser.firstName = userFromDb.firstName;
                tempUser.secondName = userFromDb.secondName;
                tempUser.id = userFromDb.id;
                return Ok(tempUser);
            }

            return Forbid();
        }

        private static string HashPassword(string input)
        {
            return SHA.ComputeSHA256Hash(input);
        }

        private static bool PasswordsMatch(string userInput, string savedTextFilePassword)
        {
            string hashedInput = HashPassword(userInput);
            bool doPasswordsMatch = string.Equals(hashedInput, savedTextFilePassword);
            return doPasswordsMatch;
        }   
    }
    public class LoginUser
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class UserToReturn
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }
    }
}