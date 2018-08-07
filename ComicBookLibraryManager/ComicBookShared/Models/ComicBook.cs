using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Models
{
    /// <summary>
    /// Represents a comic book.
    /// </summary>
    public class ComicBook
    {
        public ComicBook()
        {
            Artists = new List<ComicBookArtist>();
        }

        public int Id { get; set; }

        [Display(Name = "Series")]
        public int SeriesId { get; set; }

        [Display(Name = "Issue Number")]
        public int IssueNumber { get; set; }

        public string Description { get; set; }

        [Display(Name = "Published On")]
        public DateTime PublishedOn { get; set; }

        [Display(Name = "Average Rating")]
        public decimal? AverageRating { get; set; }

        public Series Series { get; set; }
        public ICollection<ComicBookArtist> Artists { get; set; }

        /// <summary>
        /// The display text for a comic book.
        /// </summary>
        public string DisplayText
        {
            get
            {
                return $"{Series?.Title} #{IssueNumber}";
            }
        }

        /// <summary>
        /// Adds an artist to the comic book.
        /// </summary>
        /// <param name="artist">The artist to add.</param>
        /// <param name="role">The role that the artist had on this comic book.</param>
        public void AddArtist(Artist artist, Role role)
        {
            Artists.Add(new ComicBookArtist()
            {
                Artist = artist,
                Role = role
            });
        }

        /// <summary>
        /// Adds an artist to the comic book.
        /// </summary>
        /// <param name="artistId">The artist ID to add.</param>
        /// <param name="roleId">The role ID that the artist had on this comic book.</param>
        public void AddArtist(int artistId, int roleId)
        {
            Artists.Add(new ComicBookArtist()
            {
                ArtistId = artistId,
                RoleId = roleId
            });
        }
    }
}

/*
 *
 Here's an overview of each of the entities in our data model.

ComicBook - This is our main entity, which represents a comic book in our application.
Series - A series is a collection of comic books, typically centered on a single main character or story (i.e. "The Amazing Spider-Man" or "Superman"). Each comic book is associated with a single series.
Artist - Represents an artist (i.e. "Stan Lee") in our application.
Role - Represents an artist's role on a comic book (i.e. "Script" or "Pencils"). It's not unusual for an artist to have more than one role per comic book.
ComicBookArtist - This is a bridge entity that allows us to associate a comic book with more than one artist, and each artist with more than one comic book. This entity defines a RoleId property in addition to the ArtistId property so each artist can be assigned a role for that comic book.
 *
 */