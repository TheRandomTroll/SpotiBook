using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotiBook.Data;
using SpotiBook.Enums;
using SpotiBook.Models;
using SpotiBook.Models.ViewModels;

namespace SpotiBook.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string user)
        {
            if(user == null)
            {
                return LocalRedirect("/");
            }

            ApplicationUser userData = this.context.Users.FirstOrDefault(x => x.UserName == user);

            if(userData == null)
            {
                return LocalRedirect("/");
            }

            await this.context.Entry(userData)
                .Collection(x => x.Followers)
                .LoadAsync();

            await this.context.Entry(userData)
                .Collection(x => x.Following)
                .LoadAsync();

            await this.context.Entry(userData)
                .Collection(x => x.Posts)
                .LoadAsync();

            ViewProfileViewModel model = new ViewProfileViewModel
            {
                Username = userData.UserName,
                FollowersCount = userData.Followers.Count,
                FollowingCount = userData.Following.Count
            };

            if(this.GetCurrentUserAsync().Result.UserName == userData.UserName)
            {
                model.Posts = userData.Posts.ToList();
            }
            else
            {
                model.Posts = userData.Posts.ToList().Where(x => x.Privacy == PostPrivacyOptions.Public).ToList();
            }

            foreach (Post post in model.Posts)
            {
                await this.context.Entry(post)
                    .Reference(x => x.OriginalPost)
                    .LoadAsync();

                await this.context.Entry(post)
                    .Collection(x => x.Comments)
                    .LoadAsync();

                foreach (Comment comment in post.Comments)
                {
                    await this.context.Entry(comment)
                        .Reference(x => x.Author)
                        .LoadAsync();
                }

                Post originalPost = post.OriginalPost;
                while (originalPost != null)
                {
                    await this.context.Entry(originalPost)
                        .Reference(x => x.OriginalPost)
                        .LoadAsync();

                    originalPost = originalPost.OriginalPost;
                }
            }

            model.Posts = model.Posts.OrderByDescending(x => x.PostedOn).ToList();

            return View(model);

        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return this.userManager.GetUserAsync(this.HttpContext.User);
        }
    }
}