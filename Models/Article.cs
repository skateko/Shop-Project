using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace L11.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "To short name")]
        [MaxLength(40, ErrorMessage = "Too long name")]
        public string? ArticleName { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger or equal {0}")]
        public double Price { get; set; }
        [MinLength(1, ErrorMessage = "Too short path")]
        public string? ImageName { get; set; }
        [Required(ErrorMessage = "No CategoryId")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
