using HouseRentingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Core.Models.House
{
    public class HouseModel
    {
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(150, MinimumLength = 20)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(900, MinimumLength = 30)]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(250)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [Precision(18, 2)]
        [Display(Name = "Price per month")]
        [Range(typeof(decimal), "0.00", "2000", ErrorMessage = "decimal with min value 0 and max value 2000 ")]
        public decimal PricePerMonth { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<HouseCategoryModel> HouseCategories { get; set; } = new List<HouseCategoryModel>();   
    }
}
