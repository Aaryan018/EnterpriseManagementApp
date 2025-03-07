using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class HomeController : Controller
{
    // Action method for the Index page
    public IActionResult Index()
    {
        // Check the role of the current user and render the appropriate view
        if (User.IsInRole("Manager"))
        {
            // Return the default Home/Index.cshtml view for the Manager
            return View();  // This will return Views/Home/Index.cshtml
        }
        else if (User.IsInRole("Client"))
        {
            // Return the default Home/Index.cshtml view for the Client
            return View();  // This will return Views/Home/Index.cshtml
        }

        // Default redirect if no role assigned (show generic dashboard)
        return View();  // This will return Views/Home/Index.cshtml
    }
}
