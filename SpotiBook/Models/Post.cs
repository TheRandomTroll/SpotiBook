using SpotiBook.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int? UploadedSongId { get; set; }

        public string PosterId { get; set; }

        public string AuthorId { get; set; }

        public virtual Song UploadedSong { get; set; }

        public virtual ApplicationUser Poster { get; set; }

        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; }

        public DateTime PostedOn { get; set; }
    }
}
