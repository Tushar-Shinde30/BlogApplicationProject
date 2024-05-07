using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS.Models
{
    public class PostsViewModel
    {
        public int PostId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Please Enter a title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please Enter a Content")]
        public string Content {  get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual User User { get; set; }

        public List<SelectListItem> PostList { get; set; }
    }
}
