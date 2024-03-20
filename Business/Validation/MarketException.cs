using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validation
{
    public class MarketException : Exception
    {
        public MarketException(string message) : base(message)
        {
            
        }
    }
}
