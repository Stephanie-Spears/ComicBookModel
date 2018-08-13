using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ComicBookShared.Models;

namespace ComicBookShared.Data
{
    public class ArtistsRepository : BaseRepository<Artist>
    {
        public ArtistsRepository(Context context) : base(context)
        {
        }

        public override Artist Get(int id, bool includeRelatedEntities = true)
        {
            var artist = Context.Artists.AsQueryable();
            if (includeRelatedEntities)
            {
                artist = artist
                    .Include(s => s.ComicBooks.Select(a => a.ComicBook.Series))
                    .Include(s => s.ComicBooks.Select(a => a.Role));
            }

            return artist
                .SingleOrDefault(cb => cb.Id == id);
        }

        public override IList<Artist> GetList()
        {
            return Context.Artists
                .OrderBy(a => a.Name)
                .ToList();
        }

        public bool ArtistHasName(int artistId, string name)
        {
            return Context.Artists
                .Any(a => a.Id != artistId && a.Name == name);
        }
    }
}