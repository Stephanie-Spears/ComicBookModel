using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ComicBookShared.Models;

namespace ComicBookShared.Data
{
    public class SeriesRepository : BaseRepository<Series>
    {
        public SeriesRepository(Context context) : base(context)
        {
        }

        public override Series Get(int id, bool includeRelatedEntities = true)
        {
            var series = Context.Series.AsQueryable();

            if (includeRelatedEntities)
            {
                series = series
                    .Include(s => s.ComicBooks);
            }

            return series
                .Where(cb => cb.Id == id)
                .SingleOrDefault();
        }

        public override IList<Series> GetList()
        {
            return Context.Series
                .OrderBy(s => s.Title)
                .ToList();
        }

        public bool SeriesHasTitle(int seriesId, string title)
        {
            return Context.Series
                .Any(s => s.Id != seriesId && s.Title == title);
        }
    }
}