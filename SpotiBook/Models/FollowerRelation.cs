using SpotiBook.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Models
{
    public class FollowerRelation
    {
        public string FollowerId { get; set; }

        public string FollowingId { get; set; }

        [ForeignKey(nameof(FollowerId))]
        public virtual ApplicationUser Follower { get; set; }

        [ForeignKey(nameof(FollowingId))]
        public virtual ApplicationUser Following { get; set; }
    }
}
