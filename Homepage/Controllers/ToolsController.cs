using Microsoft.AspNetCore.Mvc;

namespace Homepage.Controllers;

public class ToolsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}