using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotiBook.Data;
using SpotiBook.Models;
using SpotiBook.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotiBook.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private readonly FeedService service;
        private readonly UserManager<ApplicationUser> userManager;

        public FeedController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.service = new FeedService(context, userManager, signInManager);
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return this.View(this.service.GetPosts(await this.userManager.GetUserAsync(this.HttpContext.User)));
        }
    }
}