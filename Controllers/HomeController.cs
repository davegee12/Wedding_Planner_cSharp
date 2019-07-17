using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        DateTime Now = DateTime.Today;
        // if bool returns true then it is in the future and valid.
        public static bool CalculateDate(DateTime Date)
        {
            // int Future = DateTime.Now.Year - Date.Year;
            if (DateTime.Now.Year < Date.Year)
            {
                Console.WriteLine("true");
                return true;
            }
            else if (DateTime.Now.Year == Date.Year)
            {
                if (DateTime.Now.DayOfYear < Date.DayOfYear)
                {
                    Console.WriteLine("true");
                    return true;
                }
                else
                {
                    Console.WriteLine("false");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("false");
                return false;
            }
        }

        // Display Index Page
        [HttpGet("")]
        public ViewResult Index()
        {
            return View("Index");
        }

        // Create RegUser POST route
        [HttpPost("create/register")]
        public IActionResult CreateRegUser(RegUser newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<RegUser> Hasher = new PasswordHasher<RegUser>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("LoggedInId", newUser.RegUserId);
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }

        // Login LogUser POST route
        [HttpPost("login")]
        public IActionResult CreateLogUser(LogUser LoggedIn)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == LoggedIn.LogEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(LoggedIn, userInDb.Password, LoggedIn.LogPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedInId", userInDb.RegUserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        // Log Out
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // Dashboard Display Page
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            if (IntVariable == null)
            {
                HttpContext.Session.Clear();
                ModelState.AddModelError("LoggedIn", "Please log in");
                return RedirectToAction("Index");
            }
            else
            {
                var LoggedInUser = dbContext.Users
                .Include(user => user.WeddingsAttended)
                .ThenInclude( r => r.Wedding)
                .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                List<Wedding> AllWeddings = dbContext.Weddings
                .Include(w => w.GuestList)
                .ThenInclude(g => g.Guest)
                .OrderByDescending(i => i.WeddingDate)
                .ToList();

                ViewBag.LoggedInUser = LoggedInUser;
                return View("Dashboard", AllWeddings);
            }
        }

        // Display page to create new Wedding
        [HttpGet("new/wedding")]
        public IActionResult NewWedding()
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            var LoggedInUser = dbContext.Users
            .Include(user => user.WeddingsAttended)
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));
            ViewBag.LoggedInUser = LoggedInUser;
            return View("CreateWedding");
        }

        // Post route for creating new wedding
        [HttpPost("create/wedding")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            if (ModelState.IsValid)
            {
                if (CalculateDate(newWedding.WeddingDate) == false)
                {
                    ModelState.AddModelError("WeddingDate", "Wedding can't be in the future!");
                    return View("CreateWedding");
                }
                else
                {
                    int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
                    var LoggedInUser = dbContext.Users
                    .Include(user => user.WeddingsAttended)
                    .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

                    newWedding.RegUserId = Convert.ToInt32(IntVariable);
                    newWedding.Creator = LoggedInUser;
                    dbContext.Add(newWedding);
                    dbContext.SaveChanges();


                    var AllWeddings = dbContext.RSVPs
                    .Include(w => w.Wedding)
                    .ThenInclude(g => g.GuestList)
                    .ToList();

                    ViewBag.AllWeddings = AllWeddings;

                    ViewBag.LoggedInUser = LoggedInUser;
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return View("CreateWedding");
            }
        }

        // Route to display Show page
        [HttpGet("{id}")]
        public IActionResult Show(int id)
        {
            var ThisWedding = dbContext.Weddings
            .Include(u => u.GuestList)
            .ThenInclude(l => l.Guest)
            .FirstOrDefault(w => w.WeddingId == id);
            ViewBag.ThisWedding = ThisWedding;

            List<RSVP> ThisRSVP = dbContext.RSVPs
            .Where(i => i.WeddingId == id)
            .Include(a => a.Guest)
            .ToList();
            ViewBag.GuestList = ThisRSVP;

            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            var LoggedInUser = dbContext.Users
            .Include(user => user.WeddingsAttended)
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));
            ViewBag.ThisUser = LoggedInUser;

            return View("Show");
        }

        // Delete Wedding
        [HttpGet("{id}/delete")]
        public IActionResult Delete(int id)
        {
            Wedding RetrievedWedding = dbContext.Weddings.FirstOrDefault(wedding => wedding.WeddingId == id);
            dbContext.Weddings.Remove(RetrievedWedding);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // RSVP Button
        [HttpGet("{id}/add/guest")]
        public IActionResult AddGuestToGuestList(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            RegUser LoggedInUser = dbContext.Users
            .Include(user => user.WeddingsAttended)
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            Wedding RsvpWedding = dbContext.Weddings
            .FirstOrDefault(i => i.WeddingId == id);

            RSVP thisRSVP = new RSVP();
            thisRSVP.RegUserId = LoggedInUser.RegUserId;
            thisRSVP.WeddingId = id;
            dbContext.RSVPs.Add(thisRSVP);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        // un-RSVP Button
        [HttpGet("{id}/remove/guest")]
        public IActionResult RemoveGuestFromGuestList(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("LoggedInId");
            RegUser LoggedInUser = dbContext.Users
            .FirstOrDefault(u => u.RegUserId == Convert.ToInt32(IntVariable));

            RSVP deadRSVP = dbContext.RSVPs
            .FirstOrDefault(h => h.WeddingId == id && h.RegUserId == LoggedInUser.RegUserId);

            dbContext.RSVPs.Remove(deadRSVP);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }
    }
}
