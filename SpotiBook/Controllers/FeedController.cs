﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotiBook.Data;
using SpotiBook.Enums;
using SpotiBook.Models;
using SpotiBook.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpotiBook.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public FeedController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await this.GetCurrentUserAsync();
            this.context.Entry(user)
                .Collection(x => x.Posts)
                .Load();

            this.context.Entry(user)
                .Collection(x => x.Following)
                .Load();

            this.context.Entry(user)
                .Collection(x => x.Followers)
                .Load();


            List<Post> posts = user.Posts
                .ToList();

            foreach (FollowerRelation relation in user.Following)
            {
                this.context.Entry(relation)
                    .Reference(x => x.Follower)
                    .Load();
                this.context.Entry(relation.Follower)
                    .Collection(x => x.Posts)
                    .Load();

                posts.AddRange(relation.Follower.Posts);
            }

            foreach(Post post in posts)
            {
                this.context.Entry(post)
                    .Reference(x => x.OriginalPost)
                    .Load();

                Post originalPost = post.OriginalPost;
                while(originalPost != null)
                {
                    this.context.Entry(originalPost)
                        .Reference(x => x.OriginalPost)
                        .Load();

                    originalPost = originalPost.OriginalPost;
                }
            }

            return this.View(posts.OrderByDescending(x => x.PostedOn));
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                Post post = new Post
                {
                    Description = model.Description,
                    Author = await this.GetCurrentUserAsync(),
                    Privacy = model.Privacy,
                    PostedOn = DateTime.Now
                };

                if (model.Source == SongSource.Upload)
                {
                    if (model.UploadedSong.Length > 0)
                    {
                        byte[] p1 = null;
                        using (Stream fs1 = model.UploadedSong.OpenReadStream())
                        {
                            using (MemoryStream ms1 = new MemoryStream())
                            {
                                fs1.CopyTo(ms1);
                                p1 = ms1.ToArray();
                            }
                        }
                        post.Mp3 = p1;
                    }
                }
                else
                {
                    post.YoutubeUrl = model.YouTubeURL;
                }

                this.context.Posts.Add(post);
                await this.context.SaveChangesAsync();

                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        public IActionResult Share(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            Post post = this.context.Posts.FirstOrDefault(x => x.Id == id);

            if(post == null)
            {
                return RedirectToAction("Index");
            }

            return this.View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Share(int postId, string description)
        {
            if(ModelState.IsValid)
            {
                this.context.Posts.Add(new Post
                {
                    Author = await this.GetCurrentUserAsync(),
                    PostedOn = DateTime.Now,
                    Description = description,
                    OriginalPost = this.context.Posts.First(x => x.Id == postId)
                });

                await this.context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View();
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return this.userManager.GetUserAsync(this.HttpContext.User);
        }
    }
}