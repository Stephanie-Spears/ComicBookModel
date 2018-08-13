using System.Collections.Generic;
using System.Linq;
using ComicBookShared.Models;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class Repository
    {
        private readonly Context _context; // default value is null

        public Repository(Context context)
        {
            _context = context;
        }

        public IList<Artist> GetArtists()
        {
            return _context.Artists.OrderBy(a => a.Name).ToList();
        }

        public IList<Role> GetRoles()
        {
            return _context.Roles.OrderBy(r => r.Name).ToList();
        }

        public IList<Series> GetSeriesList()
        {
            return _context.Series.OrderBy(s => s.Title).ToList();
        }
    }
}