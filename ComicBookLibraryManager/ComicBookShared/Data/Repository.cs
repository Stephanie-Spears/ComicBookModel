using ComicBookShared.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace ComicBookShared.Data
{
    /// <summary>
    /// Repository class that provides various database queries
    /// and CRUD operations.
    /// </summary>
    public static class Repository
    {
        /// <summary>
        /// Private method that returns a database context.
        /// </summary>
        /// <returns>An instance of the Context class.</returns>
        private static Context GetContext()
        {
            var context = new Context();
            context.Database.Log = (message) => Debug.WriteLine(message);
            return context;
        }

        /// <summary>
        /// Returns a count of the comic books.
        /// </summary>
        /// <returns>An integer count of the comic books.</returns>
        public static int GetComicBookCount()
        {
            using (Context context = GetContext())
            {
                return context.ComicBooks.Count();
            }
        }

        /// <summary>
        /// Returns a list of comic books ordered by the series title
        /// and issue number.
        /// </summary>
        /// <returns>An IList collection of ComicBook entity instances.</returns>
        public static IList<ComicBook> GetComicBooks()
        {
            using (Context context = GetContext())
            {
                return context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a single comic book.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to retrieve.</param>
        /// <returns>A fully populated ComicBook entity instance.</returns>
        public static ComicBook GetComicBook(int comicBookId)
        {
            using (Context context = GetContext())
            {
                return context.ComicBooks
                    .Include(cb => cb.Series)
                    .Include(cb => cb.Artists.Select(a => a.Artist))
                    .Include(cb => cb.Artists.Select(a => a.Role))
                    .Where(cb => cb.Id == comicBookId)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Returns a list of series ordered by title.
        /// </summary>
        /// <returns>An IList collection of Series entity instances.</returns>
        public static IList<Series> GetSeries()
        {
            using (Context context = GetContext())
            {
                return context.Series
                    .OrderBy(s => s.Title)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a single series.
        /// </summary>
        /// <param name="seriesId">The series ID to retrieve.</param>
        /// <returns>A Series entity instance.</returns>
        public static Series GetSeries(int seriesId)
        {
            using (Context context = GetContext())
            {
                return context.Series
                    .Where(s => s.Id == seriesId)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Returns a list of artists ordered by name.
        /// </summary>
        /// <returns>An IList collection of Artist entity instances.</returns>
        public static IList<Artist> GetArtists()
        {
            using (Context context = GetContext())
            {
                return context.Artists
                    .OrderBy(a => a.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a list of roles ordered by name.
        /// </summary>
        /// <returns>An IList collection of Role entity instances.</returns>
        public static IList<Role> GetRoles()
        {
            using (Context context = GetContext())
            {
                return context.Roles
                    .OrderBy(r => r.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Adds a comic book.
        /// </summary>
        /// <param name="comicBook">The ComicBook entity instance to add.</param>
        public static void AddComicBook(ComicBook comicBook)
        {
            using (Context context = GetContext())
            {
                comicBook.Series = new Series()
                {
                    Id = 3,
                    Title = "Bone"
                };

                context.ComicBooks.Add(comicBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a comic book.
        /// </summary>
        /// <param name="comicBook">The ComicBook entity instance to update.</param>
        public static void UpdateComicBook(ComicBook comicBook)
        {
            using (Context context = GetContext())
            {
                context.ComicBooks.Attach(comicBook);
                var comicBookEntry = context.Entry(comicBook);
                comicBookEntry.State = EntityState.Modified;
                //Setting this property to IsModified = false means that the db won't change the value for this property. It basically protects it from changes. Might be good to use for ID or any other property you don't want to get changed when the other properties are being modified.
                // comicBookEntry.Property("IssueNumber").IsModified = false;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a comic book.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to delete.</param>
        public static void DeleteComicBook(int comicBookId)
        {
            using (Context context = GetContext())
            {
                var comicBook = new ComicBook() { Id = comicBookId };
                context.Entry(comicBook).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }
}

/* Notes
 * To delete an entity, it must be tracked by the context before calling the DbSet Remove method. -- If we want to avoid having to retrieve the entity from the database, we can create a simple stub entity and attach it to the context.
 * Defining foreign key properties gives you the ability to associate a related principal entity by just setting a foreign key property value on the dependent entity.
 * To successfully delete an entity, every property on the entity does not need to have a value. The only property that needs a value is the entity's key property, which is used to identify the database table row to delete.
 * Entities that are not being tracked by the context are said to be disconnected or detached.
 * When calling the context's Entry method, if the passed in entity is not in the context, EF will attach it and set its state to "Unchanged". --This allows you to attach and set an entity's state in a single line of code
 * Adding an entity to the context by calling the DbSet Add() method will set the entity's state to "Added". Not only will the entity's state be set to "Added", but each of the entity's related or child entity states will also be set to "Added".
 * Each entity in the context that has a state of "Added" will be inserted into the database when the context's SaveChanges() method is called. --Changing an entry's state from "Added" to "Unchanged" before calling the SaveChanges method will prevent the entity from being inserted into the database.
 * EF can be forced to treat an entity's values as "new" values by setting the associated entry's state to "Modified". --When calling the SaveChanges method, EF will persist each of the "Modified" entity's property values to the database.
 * After attaching a disconnected entity to the context using the Attach method, its state will be set to "Unchanged". --The entity's state is set to "Unchanged" as EF is unable to detect whether or not any of the entity's property values are different from the values that are currently in the database.
 * When deleting an entity, EF will—by default—cascade delete any dependent entities whose foreign key properties are not nullable. --If the foreign key property is nullable, then EF will set the property value to null when the principal entity is deleted.
 *
     */