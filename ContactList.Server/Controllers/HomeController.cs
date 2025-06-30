using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ContactList.Server.Controllers;
   
        public class HomeController : Controller
        {
            public IActionResult Book()
            {
                return View();
            }

            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Search()
            {
                return View();
            }

            public IActionResult Login()
            {
                return View();
            }

           public IActionResult Register()
            {
              return View();
            }


    public IActionResult Statute()
            {
                return View();
            }

            public IActionResult AddBook()
            {
                return View();
            }

            public IActionResult EditBook()
            {
                return View();
            }

            public IActionResult User()
            {
                return View();
            }

            public IActionResult Forbidden()
            {
                return View();
            }

            //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            //public IActionResult Error()
            //{
            //    //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            //}

        }
