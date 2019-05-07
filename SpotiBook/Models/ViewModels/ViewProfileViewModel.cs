using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Models.ViewModels
{
    public class ViewProfileViewModel
    {
        public string Username { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public List<Post> Posts { get; set; }
    }
}
