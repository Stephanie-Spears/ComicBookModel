using ComicBookShared.Models;

namespace ComicBookShared.Data.Commands
{
    internal class AddComicBookArtistCommand
    {
        private readonly Context _context;

        public AddComicBookArtistCommand(Context context)
        {
            _context = context;
        }

        public void Execute(int comicBookId, int artistId, int roleId)
        {
            var comicBookArtist = new ComicBookArtist()
            {
                ComicBookId = comicBookId,
                ArtistId = artistId,
                RoleId = roleId
            };
            _context.ComicBookArtists.Add(comicBookArtist);
            _context.SaveChanges();
        }
    }
}