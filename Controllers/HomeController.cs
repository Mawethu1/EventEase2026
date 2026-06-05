using Microsoft.AspNetCore.Mvc;

namespace EventEase2026.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    [Route("Home/Error")]
    public IActionResult Error() => View();
}
