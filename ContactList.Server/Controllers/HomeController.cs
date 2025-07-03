using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ContactList.Server.Controllers;
   
        public class HomeController : Controller
        {

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


            public IActionResult User()
            {
                return View();
            }

          

          

            public IActionResult Forbidden()
            {
                return View();
            }

            

        }
