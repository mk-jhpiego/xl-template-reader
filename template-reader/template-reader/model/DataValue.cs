using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template_reader.model
{
    public static class MyExtensions
    {
        public static double ToDouble(this string strValue)
        {
            var asLower = strValue.ToLowerInvariant();
            if (asLower == "x" || asLower == "na")
                return Constants.NOVALUE;
            return double.Parse(asLower);
        }
    }
    public class Constants
    {
        public const double NOVALUE = -999999;
    }
}
