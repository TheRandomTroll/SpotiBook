using Microsoft.AspNetCore.Http;
using SpotiBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Models.ViewModels
{
    public class CreatePostViewModel
    {
        [Required]
        public string Description { get; set; }

        public SongSource Source { get; set; }

        [Display(Name = "Youtube URL")]
        public string YouTubeURL { get; set; }

        [Display(Name = "Song")]
        public IFormFile UploadedSong { get; set; }

        public PostPrivacyOptions Privacy { get; set; }
    }
}