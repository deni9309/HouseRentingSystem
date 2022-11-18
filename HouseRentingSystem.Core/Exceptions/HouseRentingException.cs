using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Core.Exceptions
{
    public class HouseRentingException :ApplicationException
    {
        public HouseRentingException()
        {

        }

        public HouseRentingException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
