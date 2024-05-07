using BMS.Data;
using BMS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BMS.Services
{
    public class PostService
    {

        private readonly PostService _context;
        private readonly UserDBContext db;

        public PostService(UserDBContext context)
        {
            db = context;
        }
        
        public int? GetUserId(string username, string password)
        {
            // Query the User table to retrieve the userId based on username and password
            var user = db.BMSUsers.FirstOrDefault(u => u.UserName == username && u.Password == password);

            // Return userId if user is found, otherwise return null
            return user?.UserId;
        }

        public string? GetUserEmail(string username, string password)
        {
            // Query the User table to retrieve the userId based on username and password
            var user = db.BMSUsers.FirstOrDefault(u => u.UserName == username && u.Password == password);

            // Return userId if user is found, otherwise return null
            return user?.Email;
        }

        public void CreatePost(PostsViewModel postData)
        {
            var post = new Posts()
            {
                PostId = postData.PostId,
                UserId = postData.UserId,
                Title = postData.Title,
                Content = postData.Content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            db.BMSPosts.Add(post);
            db.SaveChanges();
        }

        public DateTime? GetPostCreatedAt(int postId)
        {
            try
            {
                var post = db.BMSPosts.Find(postId);

                if (post != null)
                {
                    return post.CreatedAt;
                }
                else
                {
                    // Post not found with the given postId
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error in GetPostCreatedAt: {ex.Message}");
                return null;
            }
        }


        public List<Posts> GetAllPosts()
        {
            // Query all posts from the database
            var posts = db.BMSPosts.ToList();

            return posts;
        }

        public PostsViewModel GetPostForEdit(int postId)
        {
            var post = db.BMSPosts.SingleOrDefault(x => x.PostId == postId);

            if (post != null)
            {
                return new PostsViewModel()
                {
                    PostId = post.PostId,
                    UserId = post.UserId, 
                    CreatedAt= post.CreatedAt,
                    UpdatedAt = DateTime.Now,
                    Title = post.Title,
                    Content = post.Content
                };
            }
            else
            {
                throw new Exception($"Post not found with the id: {postId}");
            }
        }


        public void UpdatePost(PostsViewModel model)
        {
            try
            {
                var existingPost = db.BMSPosts.FirstOrDefault(p => p.PostId == model.PostId);

                if (existingPost != null)
                {
                    // Update the existing post entity with the new values
                    existingPost.UserId = model.UserId;
                    existingPost.CreatedAt = model.CreatedAt;
                    existingPost.UpdatedAt = DateTime.Now;
                    existingPost.Title = model.Title;
                    existingPost.Content = model.Content;

                    db.BMSPosts.Update(existingPost); // Mark the entity as modified
                    db.SaveChanges(); // Save changes to the database
                }
                else
                {
                    throw new Exception($"Post not found with the id: {model.PostId}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating item details", ex);
            }
        }

        public PostsViewModel GetItemForDelete(int postId)
        {
            try
            {
                var post = db.BMSPosts.SingleOrDefault(x => x.PostId == postId);

                if (post != null)
                {
                    return new PostsViewModel()
                    {
                        PostId = post.PostId,
                        UserId = post.UserId,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = DateTime.Now,
                        Title = post.Title,
                        Content = post.Content
                    };
                }
                else
                {
                    throw new Exception($"Item details not available with the id: {postId}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching item details for deletion", ex);
            }
        }

        public void DeleteItem(int postId)
        {
            try
            {
                var post = db.BMSPosts.SingleOrDefault(x => x.PostId == postId);

                if (post != null)
                {
                    db.BMSPosts.Remove(post);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception($"Item details not available with the id: {postId}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting item", ex);
            }
        }
        
    }
}
