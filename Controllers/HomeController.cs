using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Examprep.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Examprep.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private int? uid
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
        }
        private bool isLoggedIn
        {
            get { return uid != null; }
        }

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            // Check initial ModelState => if there are no errors
            if (ModelState.IsValid)
            {
                // If a User exists with provided email
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                // if we reach here it confirms this is new user
                // add them to db... after we hash the password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();
                // save their id to session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                // redirect to Dashboard
                return RedirectToAction("display");
            }
            return View("Index");
        }
        // other code
        [HttpGet("")]
        public IActionResult Index()
        {
            if (!isLoggedIn)
            {
                return View();
            }
            return RedirectToAction("display");
        }
        [HttpGet("display")]
        public IActionResult Display()
        {
           if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            // query all movies and user info and likes
            List<Movie> allMovies = _context
                .Movies   // gets all movies and properties
                .Include(m => m.PostedBy) // grab PostedBy nav property
                .Include(m => m.Fans)  // grab Fans nav property
                .ToList();
            // call user info and put in viewBag
            User u = _context.Users.FirstOrDefault(u => u.UserId == (int)uid);
            ViewBag.User = u;
            return View(allMovies);
            
        }
        [HttpGet("reset")]
        public IActionResult Reset()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpPost("Login")]

        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("loginpage");// handle failure (this should be similar to how "existing email" is handled)
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("display");
            }
            return View("Index");
        }
     
       
        [HttpGet("AddMovie")]
        public IActionResult AddMovie()
        {
            return View();
        }
         [HttpPost("NewMovie")]
        public IActionResult NewMovie(Movie movie)
        {
             if (movie.ReleaseDate > DateTime.Now)
            {
                ModelState.AddModelError("ReleaseDate", "Release Date must be in the past");
            }
            // run validation
            if (ModelState.IsValid)
            {
                // store movie in db
                movie.UserId = (int)uid;
                _context.Movies.Add(movie);
                _context.SaveChanges();
                return Redirect($"/Info/{movie.MovieId}");
                // this below one does the same thing
                // return RedirectToAction("Movies", new { movieId = movie.MovieId });
            }
            User u = _context.Users.FirstOrDefault(u => u.UserId == (int)uid);
            ViewBag.User = u;
            return View("AddMovie");
        }
         [HttpGet("Info/{movieId}")]
        public IActionResult Info(int movieId)
        { 
           Movie thisMovie = _context
            .Movies
            .Include(m => m.PostedBy)
            .Include(m => m.Fans)
            .ThenInclude(f => f.Fan)
            .FirstOrDefault(m => m.MovieId == movieId);
            // call user info and put in viewBag
            User u = _context.Users.FirstOrDefault(u => u.UserId == (int)uid);
            ViewBag.User = u;
            return View(thisMovie);
        }
        [HttpGet("Delete/{movieId}")]
          public IActionResult Delete(int movieId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            // query movies db by id
            Movie delMovie = _context.Movies.FirstOrDefault(m => m.MovieId == movieId);
            // remove from db
            _context.Movies.Remove(delMovie);
            // save changes
            _context.SaveChanges();
            return RedirectToAction("display");
        }
        [HttpGet("like/{movieId}")]
        public IActionResult Like(int movieId)
        {
            // create new Like instance
            Like like = new Like();
            // reassign UserId and MovieId
            like.UserId = (int)uid;
            like.MovieId = movieId;
            // Add to Likes table in db
            _context.Likes.Add(like);
            // save changes
            _context.SaveChanges();
            // redirect dashboard
            return RedirectToAction("display");
        }
        [HttpGet("unlike/{movieId}")]
        public IActionResult Unlike(int movieId)
        {
            // query Like from db
            // must match the movieId and userId in the 1 Like relationship
            Like unlike = _context.Likes.FirstOrDefault(l => l.FanOf.MovieId == movieId && l.Fan.UserId == (int)uid);
            // Add to Likes table in db
            _context.Likes.Remove(unlike);
            // save changes
            _context.SaveChanges();
            // redirect dashboard
            return RedirectToAction("display");
        }


        [HttpGet("Exit")]
        public IActionResult Exit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
