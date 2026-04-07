using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace oop_s2_1_mvc_83356.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        _logger.LogInformation("Error page accessed with status code: {StatusCode}", statusCode);

        return statusCode switch
        {
            404 => View("Error404", new ErrorViewModel { StatusCode = 404 }),
            403 => View("Error403", new ErrorViewModel { StatusCode = 403 }),
            _ => View("Error500", new ErrorViewModel { StatusCode = statusCode ?? 500 })
        };
    }
}

public class ErrorViewModel
{
    public int StatusCode { get; set; }
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
