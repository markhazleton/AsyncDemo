using Westwind.AspNetCore.Markdown;

namespace AsyncDemo.Web.Controllers;

/// <summary>
/// Home Controller
/// </summary>
public class HomeController : BaseController
{
    /// <summary>
    /// Error
    /// </summary>
    /// <returns></returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    { return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); }

    /// <summary>
    /// Home Page
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        var myContent = Markdown.ParseHtmlStringFromFile("~/README.md");
        return View(myContent);
    }

    /// <summary>
    /// Bootstrap 5 Theme Demo - Kitchen Sink
    /// </summary>
    /// <returns>View displaying all Bootstrap 5 components and styles</returns>
    public IActionResult ThemeDemo()
    {
        return View();
    }
}

