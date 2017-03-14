using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WaruniAzureAssignement.Models;
using WaruniAzureAssignement.Services;

namespace WaruniAzureAssignement.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private PostSearch _postSearch = new PostSearch();

        // GET: Post
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRepository<UserPost>.GetItemsAsync();
            return View(items);
        }

        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            ViewBag.loggedInUser = User.Identity.Name;
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "PostId,Heading,Content,Tag,CreatedOn,Author")] UserPost item)
        {
            if (ModelState.IsValid)
            {
                item.CreatedOn = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                item.Author = User.Identity.Name;
                await DocumentDBRepository<UserPost>.CreateItemAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "PostId,Heading,Content,Tag,CreatedOn,Author")] UserPost item)
        {
            if (ModelState.IsValid)
            {
                UserPost post = await DocumentDBRepository<UserPost>.GetItemAsync(item.PostId);
                post.Heading = item.Heading;
                post.Content = item.Content;
                post.Tag = item.Tag;
                post.CreatedOn = item.CreatedOn;
                post.Author = item.Author;

                await DocumentDBRepository<UserPost>.UpdateItemAsync(item.PostId, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserPost item = await DocumentDBRepository<UserPost>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            await DocumentDBRepository<UserPost>.DeleteItemAsync(id);

            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserPost item = await DocumentDBRepository<UserPost>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            if (item.Comments == null)
            {
                item.Comments = new List<PostComment>();
            }

            return View(item);
        }

        [ActionName("Comment")]
        public async Task<ActionResult> CommentAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserPost item = await DocumentDBRepository<UserPost>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.PostId = item.PostId;
            return View();
        }

        [HttpPost]
        [ActionName("Comment")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentAsync(PostComment comment)
        {
            if (ModelState.IsValid)
            {
                comment.UserName = User.Identity.Name;
                UserPost item = await DocumentDBRepository<UserPost>.GetItemAsync(comment.PostId);
                if (item.Comments == null)
                {
                    item.Comments = new List<PostComment>();
                }
                item.Comments.Add(comment);

                await DocumentDBRepository<UserPost>.UpdateItemAsync(item.PostId, item);
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        [ActionName("CommentList")]
        public async Task<ActionResult> CommentListAsync(string id)
        {
            UserPost item = await DocumentDBRepository<UserPost>.GetItemAsync(id);
            return View(item);
        }

        [ActionName("Search")]
        public async Task<ActionResult> SearchAsync(string searchText = "")
        {

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = "*";
            var searchResults = _postSearch.Search(searchText);

             var userPosts = new List<UserPost>();
            if (searchResults != null)
             {
                foreach (var p in searchResults.Results)
                {
                    var fields = p.Document.ToArray();
                    var userPost = new UserPost()
                    {
                        PostId = fields[0].Value.ToString(),
                        Heading = fields[1].Value.ToString(),
                        Content = fields[2].Value.ToString(),
                        Tag = fields[3].Value.ToString(),
                        CreatedOn = fields[4].Value.ToString(),
                        Author = fields[5].Value.ToString()
                    };
                    userPosts.Add(userPost);
                }
            } 
            return View(userPosts);
        }
    }
}