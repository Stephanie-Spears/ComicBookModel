using ComicBookShared.Models;
using System.Linq;
using System.Web.Mvc;
using ComicBookLibraryManagerWebApp.ViewModels;
using System.Net;
using ComicBookShared.Data;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Comic Books" section of the website.
    /// </summary>
    public class ComicBooksController : BaseController
    {
        private readonly ComicBooksRepository _comicBooksRepository;
        private readonly SeriesRepository _seriesRepository;
        private readonly ArtistsRepository _artistsRepository;

        public ComicBooksController()
        {
            _comicBooksRepository = new ComicBooksRepository(Context);
            _seriesRepository = new SeriesRepository(Context);
            _artistsRepository = new ArtistsRepository(Context);
        }

        public ActionResult Index()
        {
            var comicBooks = _comicBooksRepository.GetList();

            return View(comicBooks);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comicBook = _comicBooksRepository.Get((int)id);

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
            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

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

                _comicBooksRepository.Add(comicBook);

                TempData["Message"] = "Your comic book was successfully added!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comicBook = _comicBooksRepository.Get((int)id,
                includeRelatedEntities: false);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBooksEditViewModel()
            {
                ComicBook = comicBook
            };

            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ComicBooksEditViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;

                _comicBooksRepository.Update(comicBook);

                TempData["Message"] = "Your comic book was successfully updated!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            viewModel.Init(Repository, _seriesRepository, _artistsRepository);

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Include the "Series" navigation property.
            var comicBook = _comicBooksRepository.Get((int)id);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            return View(comicBook);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _comicBooksRepository.Delete(id);

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
                if (_comicBooksRepository.ComicBookSeriesHasIssueNumber(comicBook.Id, comicBook.SeriesId, comicBook.IssueNumber))
                {
                    ModelState.AddModelError("ComicBook.IssueNumber",
                        "The provided Issue Number has already been entered for the selected Series.");
                }
            }
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

/*
 * The IDisposable interface provides a mechanism for releasing unmanaged resources, through its single Dispose() method. The Entity Framework DbContext class implements the IDisposable interface.
 *
 * The Database `SetInitializer` method should NOT be called every time that the database context class is instantiated. The Database `SetInitializer` method only needs to be called once per application start—before the first use of the context.
 *
 * Unmanaged resources are resources that are not managed by the .NET runtime and therefore need to be manually released or cleaned up.
 *
 * Adding a database connection string to your app's configuration file with a name that matches your database context class name will cause Entity Framework to implicitly use it when attempting to connect to the database. Both the server and database name can be configured via the database connection string.
 *
 * The dispose pattern is a commonly misunderstood design pattern that is, unfortunately, often implemented incorrectly.
 */

/*
 * Utilizing a custom base controller class gives you a way to share code across your application's controllers.  Having a custom base controller class can help you to adhere to the DRY (don't repeat yourself) design principle.
 *
 * A long-living database context can—over time—have a negative impact on the performance of your application. We don't want to instantiate a context every time we need to retrieve or persist data, but we also don't want a context that lives for too long.
 *
 * Utilizing class library projects is a common approach for sharing code across two or more projects. While class library projects provide functionality that other projects can consume, they can't be directly ran or executed.
 *
 * Query and command classes should be created for queries and commands that are relatively complex or used in more than one location. Query and command classes can be used as an alternative to repositories.
 *
 * A "logical" delete leaves the record in the database but flags it as "inactive" by setting a special property or column value. This is typically done by setting an "Active" property or column to a value of "false".
 */