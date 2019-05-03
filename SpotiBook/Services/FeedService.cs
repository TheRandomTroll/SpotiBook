using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpotiBook.Data;
using SpotiBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Services
{
    public class FeedService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public FeedService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public List<Post> GetPosts(ApplicationUser currentUser)
        {
            this.context.Entry(currentUser)
                .Collection(x => x.Posts)
                .Load();

            this.context.Entry(currentUser)
                .Collection(x => x.Following)
                .Load();

            List<Post> posts = currentUser.Posts
                .ToList();

            foreach(FollowerRelation relation in currentUser.Following)
            {
                posts.AddRange(relation.Following.Posts);
            }

            return posts.OrderByDescending(x => x.PostedOn).ToList();
        }
    }
}
