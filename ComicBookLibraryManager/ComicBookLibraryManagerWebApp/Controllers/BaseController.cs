using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComicBookShared.Data;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    // Will never need to be instantiated directly, so we ensure that it's  not possible to instantiate it by adding the abstract keyword
    public abstract class BaseController : Controller
    {
        private bool _disposed; //in order to guard against the case of if the Dispose method is called more than once, let's define a private field to track if the Dispose method has already been called. Boolean default value is false.
        protected Context Context { get; private set; }

        public BaseController()
        {
            Context = new Context();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Context.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}