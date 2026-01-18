using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business
{
    //GET  used when returning category data
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    // POST : Used when creating a new category
    public class CategoryCreateViewModel
    {
        [Required, StringLength(100,MinimumLength =2)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

    }
    //PUT :Used to update a category
    public class CategoryUpdateViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required, StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }
        [StringLength(300)]
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
