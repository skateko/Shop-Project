using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace L11.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "To short name")]
        [MaxLength(40, ErrorMessage = "Write from 1 to 6 digits")]
        public string? CategoryName { get; set; }
    }
}

// https://www.gushiciku.cn/pl/pQRs/zh-hk
// https://entityframeworkcore.com/knowledge-base/57342964/how-can-i-hint-the-csharp-8-0-nullable-reference-system-that-a-property-is-initalized-using-reflection
// https://github.com/dotnet/roslyn/issues/44046