using Microsoft.AspNetCore.Mvc;

namespace RestWithASPNET.Controllers
{
    public class PersonController : Controller
    {
        private readonly ILogger<PersonController> _logger;
        public PersonController(ILogger<PersonController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
