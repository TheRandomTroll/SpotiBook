using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotiBook.Data;
using SpotiBook.Enums;
using SpotiBook.Models;
using SpotiBook.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Controllers
{
    [Authorize]
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
            if (user == null)
            {
                return this.LocalRedirect("/");
            }

            ApplicationUser userData = this.context.Users.FirstOrDefault(x => x.UserName == user);

            if (userData == null)
            {
                return this.LocalRedirect("/");
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

            if (this.GetCurrentUserAsync().Result.UserName == userData.UserName)
            {
                model.Posts = userData.Posts.ToList();
            }
            else
            {
                foreach (FollowerRelation relation in userData.Followers)
                {
                    await this.context.Entry(relation)
                        .Reference(x => x.Follower)
                        .LoadAsync();
                    await this.context.Entry(relation)
                        .Reference(x => x.Following)
                        .LoadAsync();
                }
                model.IsFollowing = userData.Followers.Any(x => x.Following.UserName == this.GetCurrentUserAsync().Result.UserName);
                if(model.IsFollowing)
                {
                    model.Posts = userData.Posts.ToList();
                }
                else
                {
                    model.Posts = userData.Posts.ToList().Where(x => x.Privacy == PostPrivacyOptions.Public).ToList();
                }
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

                    await this.context.Entry(originalPost)
                        .Reference(x => x.Author)
                        .LoadAsync();

                    originalPost = originalPost.OriginalPost;
                }
            }

            model.Posts = model.Posts.OrderByDescending(x => x.PostedOn).ToList();

            return this.View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string username)
        {
            if (username != null)
            {
                this.context.FollowerRelations.Add(new FollowerRelation
                {
                    Following = await this.GetCurrentUserAsync(),
                    Follower = await this.userManager.FindByNameAsync(username)
                });

                await this.context.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index), new { user = username });
            }

            return this.LocalRedirect("/Feed");
        }

        public async Task<IActionResult> Unfollow(string username)
        {
            if (username != null)
            {
                FollowerRelation relation = this.context.FollowerRelations
                    .FirstOrDefault(
                    x => x.Following.UserName == this.GetCurrentUserAsync().Result.UserName && x.Follower.UserName == username);

                if (relation == null)
                {
                    return this.LocalRedirect("/");
                }

                this.context.FollowerRelations.Remove(relation);

                await this.context.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index), new { user = username });
            }

            return this.LocalRedirect("/Feed");
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return this.userManager.GetUserAsync(this.HttpContext.User);
        }
    }
}