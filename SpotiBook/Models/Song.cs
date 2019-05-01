using SpotiBook.Enums;

namespace SpotiBook.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string PathToMP3 { get; set; }

        public string YoutubeUrl { get; set; }
    }
}
