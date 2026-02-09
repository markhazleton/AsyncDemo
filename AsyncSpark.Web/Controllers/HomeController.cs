using Westwind.AspNetCore.Markdown;

namespace AsyncSpark.Web.Controllers;

/// <summary>
/// Home Controller
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Constructor for HomeController
    /// </summary>
 /// <param name="logger">Logger instance</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <returns></returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
  { 
        _logger.LogError("Error page accessed with RequestId: {RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); 
    }

    /// <summary>
    /// Home Page
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        try
        {
          var myContent = Markdown.ParseHtmlStringFromFile("~/README.md");
  return View(myContent);
        }
    catch (Exception ex)
     {
    _logger.LogError(ex, "Error parsing README.md file");
            return View(new Microsoft.AspNetCore.Html.HtmlString("<h1>Welcome to AsyncSpark</h1><p>Unable to load README content.</p>"));
        }
    }

    /// <summary>
    /// Bootstrap 5 Theme Demo - Kitchen Sink
    /// </summary>
    /// <returns>View displaying all Bootstrap 5 components and styles</returns>
    public IActionResult ThemeDemo()
    {
      return View();
    }

    /// <summary>
    /// Learn Async/Await - Educational landing page
    /// </summary>
    /// <returns>View with structured learning modules and paths</returns>
    public IActionResult Learn()
    {
      return View();
    }

    /// <summary>
    /// WebSpark NuGet Packages - Information and promotion page
    /// </summary>
    /// <returns>View showcasing WebSpark.Bootswatch and WebSpark.HttpClientUtility</returns>
    public IActionResult WebSpark()
    {
      return View();
    }
}

