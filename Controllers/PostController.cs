using BMS.Models;
using BMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BMS.Controllers
{
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePostMessage()
        {
            return View();
        }

        
    }
}
