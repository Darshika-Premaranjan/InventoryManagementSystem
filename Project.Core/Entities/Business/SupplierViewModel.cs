using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business
{
    public class SupplierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TpNumber { get; set; }
        public string? Email { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class SupplierCreateViewModel
    {
        [Required, StringLength(maximumLength: 100, MinimumLength = 4)]
        public string Name { get; set; }
        [Required]
        public string TpNumber { get; set; }
        public string? Email { get; set; }
        [Required, StringLength(maximumLength: 700)]
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class SupplierUpdateViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(maximumLength: 100, MinimumLength = 4)]
        public string Name { get; set; }
        [Required]
        public string TpNumber { get; set; }
        public string? Email { get; set; }
        [Required, StringLength(maximumLength: 700)]
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
