using SpotiBook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Models
{
    public class FollowerRelation
    {
        public string FollowerId { get; set; }

        public string FollowingId { get; set; }

        public virtual ApplicationUser Follower { get; set; }

        public virtual ApplicationUser Following { get; set; }
    }
}
