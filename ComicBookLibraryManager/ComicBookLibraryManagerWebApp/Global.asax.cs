using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ComicBookLibraryManagerWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}

/*

    Global.asax.cs gives us the Application_Start() method which is configured every time the application is run.
    We can customize hwo the app is deployed on diff envirionements here.

    ASP.NET MVC FRAMEWORK takes requests from the server and uses pattern matching to determine what to do with them. It expects the request to look like {controller/action} -> {ComicBooks/Index}. So it would map to a controller named ComicBooks and an action named Index. this is called url routing.
    -once the controller and action method have been identified, MVC instantiates an instance of the controller class and calls the action method on that instance.
    -The action method returns an action result which is in turn rendered and sent back to the user's browser as an HTTP response.
    -This process is repeated for each request. So given that, each request is associated with its own instance of the appropriate controller class.
    -Aligning the lifetime of our context with the controller's lifetime gives us a convenient way of managing the lifetime of the context.

*/