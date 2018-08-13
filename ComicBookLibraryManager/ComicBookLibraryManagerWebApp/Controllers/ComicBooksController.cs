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
        private readonly Context _context; //default is null

        public ComicBooksController()
        {
            _context = new Context();
        }

        public ActionResult Index()
        {
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

                _context.ComicBooks.Add(comicBook);
                _context.SaveChanges();

                TempData["Message"] = "Your comic book was successfully added!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            viewModel.Init(_context);

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comicBook = _context.ComicBooks
                .Where(cb => cb.Id == id)
                .SingleOrDefault();

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

                _context.Entry(comicBook).State = EntityState.Modified;
                _context.SaveChanges();

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

            // Include the "Series" navigation property.
            var comicBook = _context.ComicBooks
                .Include(cb => cb.Series)
                .Where(cb => cb.Id == id)
                .SingleOrDefault();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            return View(comicBook);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var comicBook = new ComicBook() { Id = id };
            _context.Entry(comicBook).State = EntityState.Deleted;
            _context.SaveChanges();

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
            // If there aren't any "SeriesId" and "IssueNumber" field validation errors...
            if (ModelState.IsValidField("ComicBook.SeriesId") &&
                ModelState.IsValidField("ComicBook.IssueNumber"))
            {
                // Then make sure that the provided issue number is unique for the provided series.
                if (_context.ComicBooks
                    .Any(cb => cb.Id != comicBook.Id &&
                               cb.SeriesId == comicBook.SeriesId &&
                               cb.IssueNumber == comicBook.IssueNumber))
                {
                    ModelState.AddModelError("ComicBook.IssueNumber",
                        "The provided Issue Number has already been entered for the selected Series.");
                }
            }
        }

        private bool _disposed; //in order to guard against the case of if the Dispose method is called more than once, let's define a private field to track if the Dispose method has already been called. Boolean default value is false.

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

/*
 * PROJECT REVIEW
 * Added the Database connection string in order to configure the server and database that EF should use
 * Configured the database initializer using our app's config file, which prevented us from making an unnecessary number of calls to the database set initializer method
 * Managed the database context's lifetime by aligning it with the controller's lifetime
 * Implemented the necessary Reads, Creates, Updates, and Deletes (CRUD) for comic books.
 *
 */