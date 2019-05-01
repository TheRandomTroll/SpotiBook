using Microsoft.AspNetCore.Identity;
using SpotiBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Data
{
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty("Author")]
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<FollowerRelation> Followers { get; set; }
        public virtual ICollection<FollowerRelation> Following { get; set; }
    }
}
