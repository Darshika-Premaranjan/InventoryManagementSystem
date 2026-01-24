using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.General
{
    [Table("Suppliers")]
    public class Supplier : Base<int>
    {
        [Required, StringLength(maximumLength: 100, MinimumLength = 4)]

        public string Name { get; set; }
        [Required]
        public string TpNumber { get; set; }
        public string Email { get; set; }
        [Required, StringLength(maximumLength: 700)]
        public string Address { get;set; }
        public bool IsActive { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
