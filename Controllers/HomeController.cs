﻿using ASP_201.Data;
using ASP_201.Models;
using ASP_201.Services;
using ASP_201.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_201.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DateService  _dateService;
        private readonly TimeService  _timeService;
        private readonly StampService _stampService;
        private readonly IHashService _hashService;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
                              DateService dateService,
                              TimeService timeService,
                              StampService stampService,
                              IHashService hashService,
                              DataContext dataContext,
                              IConfiguration configuration)
        {
            _logger = logger;
            _dateService = dateService;
            _timeService = timeService;
            _stampService = stampService;
            _hashService = hashService;
            _dataContext = dataContext;
            _configuration = configuration;
        }
        public ViewResult Services()
        {
            ViewData["date_service"]  = _dateService.GetMoment();
            ViewData["date_hashcode"] = _dateService.GetHashCode();

            ViewData["time_service"]  = _timeService.GetMoment();
            ViewData["time_hashcode"] = _timeService.GetHashCode();

            ViewData["stamp_service"]  = _stampService.GetMoment();
            ViewData["stamp_hashcode"] = _stampService.GetHashCode();

            ViewData["hash_service"] = _hashService.Hash("123");

            return View();
        }
        public ViewResult ContextD()
        {
            ViewData["UsersCount"] = _dataContext.Users.Count();
            return View();
        }
        public ViewResult EmailConfirmation()
        {
            ViewData["config"] = _configuration["Smpt:Gmail:Host"];
            return View();
        }
        public ViewResult Sessions([FromQuery(Name = "session-attr")]String? sessionAttr)
        {
            if(sessionAttr != null)
            {
                HttpContext.Session.SetString("session-attribute", sessionAttr);
            }
            return View();
        }

        public IActionResult Index()
        {
            return View();
        } 
        
        public IActionResult Middleware()
        {
            
            return View();
        }
        public IActionResult Intro()
        {
            return View();
        }
        public IActionResult Scheme() 
        {
            ViewBag.bagdata = "Data in ViewBag";     // Способи передачі даних
            ViewData["data"] = "Data in ViewData";   // до представлення

            return View();
        }
        public IActionResult Razor()
        {
            return View();
        }
        public IActionResult PassData()
        {
            Models.Home.PassDataModel model = new()
            {
                Header = "Моделі",
                Title = "Моделі передачі даних",
                Products = new()
                {
                    new() { Name = "Зарядний кабель",       Price = 210    },
                    new() { Name = "Маніпулятор 'миша'",    Price = 399.50 },
                    new() { Name = "Наліпка 'Smiley'",      Price = 2.95   },
                    new() { Name = "Серветки для монітору", Price = 100    },
                    new() { Name = "USB ліхтарик",          Price = 49.50  },
                    new() { Name = "Аккумулятор ААА",       Price = 280    },
                    new() { Name = "ОС Windows Home",       Price = 1250   },
                }
            };

            return View(model);
        }
        public IActionResult DisplayTemplates()
        {
            Models.Home.PassDataModel model = new()
            {
                Header = "Шаблони",
                Title = "Шаблони відображення даних",
                Products = new()
                {
                    new() { Name = "Зарядний кабель",       Price = 210   , Image = "puc1.png" },
                    new() { Name = "Маніпулятор 'миша'",    Price = 399.50, Image = "puc2.png" },
                    new() { Name = "Наліпка 'Smiley'",      Price = 2.95  , Image = "puc1.png" },
                    new() { Name = "Серветки для монітору", Price = 100   , Image = "puc2.png" },
                    new() { Name = "USB ліхтарик",          Price = 49.50 , Image = "puc1.png" },
                    new() { Name = "Аккумулятор ААА",       Price = 280    },
                    new() { Name = "ОС Windows Home",       Price = 1250   },
                }
            };

            return View(model);
        }

        public ViewResult WebApi()
        {
            return View();
        }

        public IActionResult TagHelpers()
        {
            return View();
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