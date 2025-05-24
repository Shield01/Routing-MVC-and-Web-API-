using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Routing_MVC_and_Web_API_.CustomConstraints
{
    public class MonthsCustomCustomConstraints : IRouteConstraint
    {
        public bool Match(
            HttpContext? httpContext, 
            IRouter? route, 
            string routeKey, 
            RouteValueDictionary values, 
            RouteDirection routeDirection
        )
        {
            //Check whether the value exists
            if (!values.ContainsKey(routeKey)) {
                
                return false; // Not a match
            }

            Regex regex = new Regex("^(march|june|september|december)$");

            string? monthValue = Convert.ToString(values[routeKey]);

            if (regex.IsMatch(monthValue)) {
                return true; // A match
            } else
            {
                return false; // Not a match
            }
        }
    }
}
