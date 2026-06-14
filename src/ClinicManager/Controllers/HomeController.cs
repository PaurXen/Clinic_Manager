using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
