using BMS.Data;
using BMS.Models;
using BMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BMS.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly UserDBContext _Context;

        public UserController(UserService userService, PostService postService, UserDBContext Context)
        {
            _Context = Context;
            _postService = postService;
            _userService = userService;
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Edit/5
        public ActionResult Login()
        {
            return View();
        }



        //Post method for login
        [HttpPost]
        public IActionResult Login(User user)
        {
            try
            {
                if (user.UserName == "admin@44" && user.Password == "Pass@123")
                {
                    HttpContext.Session.SetString("Username", user.UserName);
                    TempData["successmessage"] = "Admin logged in successfully!!!";
                    return RedirectToAction("Administrator", "Home");
                }
                else
                {
                    var userId = _postService.GetUserId(user.UserName, user.Password);
                    var userEmail = _postService.GetUserEmail(user.UserName, user.Password);
                    //DateTime? createdAt = _postService.GetCreatedDate(user.UserName, user.Password);

                    if (userId != null) // Ensure both userId and createdAt are not null
                    {
                        HttpContext.Session.SetString("Username", user.UserName);
                        HttpContext.Session.SetInt32("UserId", userId.Value);
                        HttpContext.Session.SetString("UserEmail", userEmail);
                        TempData["successmessage"] = $"{user.UserName} logged in successfully!!!";
                        return RedirectToAction("LoggedUser","User");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Wrong Credentials!!!";
                        return View("Create","User");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Create", "User");
            }
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }
            else
            {
                try
                {
                    if (HttpContext.Session.GetString("Username") != null)
                    {
                        HttpContext.Session.Remove("Username");
                    }
                    TempData["successMessage"] = "You have logged out successfully!!!";
                    return RedirectToAction("Login", "User");
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("Login", "User");
                }
            }

        }


        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User userData)
        {
            try
            {
                var result = _userService.Create(userData);
                if (result.Success)
                {
                    TempData["successMessage"] = "User Created Successfully!!!";
                    return RedirectToAction("Login","User");
                }
                else
                {
                    TempData["errorMessage"] = "User already exists!!!";
                    return RedirectToAction("Create","User"); // Redirect to the signup view again if user already exists
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Create", "User"); // Redirect to the signup view with an error message
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult LoggedUser()
        {
            return View();
        }

        //public IActionResult DeleteUser()
        //{
        //    return View();
        //}

        
        public IActionResult DeleteUser(int userId)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }
            else
            {
                try
                {
                    var userView = _userService.GetUserForDelete(userId);
                    return View(userView);
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("ViewAllUser", "User");
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteUser(User model)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }
            else
            {
                try
                {
                    _userService.DeletingUser(model.UserId);
                    TempData["successMessage"] = "User deleted successfully!!!";
                    return RedirectToAction("ViewAllUser", "User");
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("DeleteUser", "User");
                }
            }
        }


        public IActionResult CreatePost()
        {
            return View();
        }

        // POST: PostController/Create
        [HttpPost]
        public IActionResult CreatePost(PostsViewModel postData)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }

            try
            {
                _postService.CreatePost(postData);
                TempData["successMessage"] = "Post added successfully";
                return RedirectToAction("ViewPostForOne", "User");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }

        public IActionResult ViewPostForOne()
        {
            // Check if the user is authenticated
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }
            else
            {
                // Retrieve the UserId of the currently logged-in user from the session
                int? userId = HttpContext.Session.GetInt32("UserId");

                if (!userId.HasValue)
                {
                    // If UserId is not found in session, handle the scenario accordingly
                    TempData["errorMessage"] = "User ID not found in session.";
                    return RedirectToAction("Login" , "User");
                }

                // Fetch posts belonging to the logged-in user
                var userPosts = _Context.BMSPosts.Where(post => post.UserId == userId.Value).ToList();
                return View(userPosts);
            }
        }


     
        public IActionResult ViewPostForAll()
        {
            var postData = _Context.BMSPosts.ToList();
            return View(postData);
        }


        public IActionResult ViewPostForAdmin()
        {
            var postData = _Context.BMSPosts.ToList();
            return View(postData);
        }

        public IActionResult EditPost(int postId)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }
            else
            {
                try
                {
                     //DateTime? createdAt = _postService.GetCreatedDate(user.UserName, user.Password);
                    var postView = _postService.GetPostForEdit(postId);
                    return View(postView);
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("ViewPostForOne", "User");
                }
            }
        }


        [HttpPost]
        public IActionResult EditPost(PostsViewModel model)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login","User");
            }
            else
            {
                try
                {
                    _postService.UpdatePost(model);
                    TempData["successMessage"] = "Post details updated successfully";
                    return RedirectToAction("ViewPostForOne","User");
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return View();
                }
            }
        }

        //get method for delete item
        [HttpGet]
        public IActionResult DeletePost(int postId)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }
            else
            {
                try
                {
                    var itemView = _postService.GetItemForDelete(postId);
                    return View(itemView);
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("ViewPostForOne", "User");
                }
            }
        }

        [HttpPost]
        public IActionResult DeletePost(PostsViewModel model)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToAction("Login", "User");
            }
            else
            {
                try
                {
                    _postService.DeleteItem(model.PostId);
                    TempData["successMessage"] = "Post deleted successfully!!!";
                    return RedirectToAction("ViewPostForOne", "User");
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("ViewPostForOne", "User");
                }
            }
        }

        public IActionResult ViewAllUser()
        {
            var user = _Context.BMSUsers.ToList();
            return View(user);
        }

        
        public IActionResult UserProfile()
        {
            return View();
        }

        public IActionResult UpdateUserProfile()
        {
            return View();
        }


       
    }
}
