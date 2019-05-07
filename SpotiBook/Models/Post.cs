using SpotiBook.Data;
using SpotiBook.Enums;
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

        public string PosterId { get; set; }

        public string AuthorId { get; set; }

        public PostPrivacyOptions Privacy { get; set; }

        public byte[] Mp3 { get; set; }

        public string YoutubeUrl { get; set; }

        public virtual ApplicationUser Poster { get; set; }

        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; }

        public DateTime PostedOn { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
