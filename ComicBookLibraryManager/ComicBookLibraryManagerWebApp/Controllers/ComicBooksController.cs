using ComicBookShared.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using ComicBookLibraryManagerWebApp.ViewModels;
using System.Net;
using ComicBookShared.Data;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Comic Books" section of the website.
    /// </summary>
    public class ComicBooksController : Controller
    {
        private Context _context = null;

        public ComicBooksController()
        {
            _context = new Context();
        }

        public ActionResult Index()
        {
            // TODO Get the comic books list.
            // Include the "Series" navigation property.
            var comicBooks = _context.ComicBooks
                .Include(cb => cb.Series)
                .OrderBy(cb => cb.Series.Title)
                .ThenBy(cb => cb.IssueNumber)
                .ToList();

            return View(comicBooks);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comicBook = _context.ComicBooks
                .Include(cb => cb.Series)
                .Include(cb => cb.Artists.Select(a => a.Artist))
                .Include(cb => cb.Artists.Select(a => a.Role))
                .Where(cb => cb.Id == id)
                .SingleOrDefault();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            // Sort the artists.
            comicBook.Artists = comicBook.Artists.OrderBy(a => a.Role.Name).ToList();

            return View(comicBook);
        }

        public ActionResult Add()
        {
            var viewModel = new ComicBooksAddViewModel();

            // Pass the Context class to the view model "Init" method.
            viewModel.Init(_context);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(ComicBooksAddViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;
                comicBook.AddArtist(viewModel.ArtistId, viewModel.RoleId);

                // TODO Add the comic book.

                TempData["Message"] = "Your comic book was successfully added!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            // TODO Pass the Context class to the view model "Init" method.
            viewModel.Init(_context);

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get the comic book.
            var comicBook = new ComicBook();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBooksEditViewModel()
            {
                ComicBook = comicBook
            };
            viewModel.Init(_context);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ComicBooksEditViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;

                // TODO Update the comic book.

                TempData["Message"] = "Your comic book was successfully updated!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            viewModel.Init(_context);

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get the comic book.
            // Include the "Series" navigation property.
            var comicBook = new ComicBook();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            return View(comicBook);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO Delete the comic book.

            TempData["Message"] = "Your comic book was successfully deleted!";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Validates a comic book on the server
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="comicBook">The comic book to validate.</param>
        private void ValidateComicBook(ComicBook comicBook)
        {
            //// If there aren't any "SeriesId" and "IssueNumber" field validation errors...
            //if (ModelState.IsValidField("ComicBook.SeriesId") &&
            //    ModelState.IsValidField("ComicBook.IssueNumber"))
            //{
            //    // Then make sure that the provided issue number is unique for the provided series.
            //    // TODO Call method to check if the issue number is available for this comic book.
            //    if (false)
            //    {
            //        ModelState.AddModelError("ComicBook.IssueNumber",
            //            "The provided Issue Number has already been entered for the selected Series.");
            //    }
            //}
        }

        private bool _disposed = false; //in order to guard against the case of if the Dispose method is called more than once, let's define a private field to trqack if the Dispose method has already been called.

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}

/*
 Notes:
 The web.config file in the project root is for configuring the MVC application, but the web.config file in the views folder is specifically for the Razor view engine and shouldn't be touched.

    when we use our Context to persist or retrieve data from the database, EF will open a connection to the database, which is an unmanaged resource (meaning it doesn't auto-delete when it goes out of scope). By calling our context's Dispose() method (which is inherited from teh DBContext base class), we're letting EF know that the database connection can be closed.
    -Previously we placed the connection in a using statement in order to ensure that the context isDispose method was called. Now that we're instantiating the context within a controller's constructor, we can't use that approach. Luckily we can explicitly dispose of the context.

     */