using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HouseRentingSystem.Core.Models.House
{
    public class HouseDetailsViewModel
    {
        public int Id { get; set; }
       
        public string Title { get; set; } = null!;

      
        public string Address { get; set; } = null!;


        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = null!;

    }
}
