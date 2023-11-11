using System.Reflection;
using System.Text;

namespace Repository.Extensions.Utility;

public static class OrderQueryBuilder
{
    /// <summary>
    /// Builds a dynamic LINQ order clause from a query string such as "price desc,number".
    /// Only public properties of <typeparamref name="T"/> are accepted, so unknown or
    /// malicious input is ignored instead of being passed to the expression parser.
    /// </summary>
    public static string CreateOrderQuery<T>(string orderByQueryString)
    {
        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderQueryBuilder = new StringBuilder();

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var propertyFromQueryName = param.Trim().Split(' ')[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi =>
                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty == null)
                continue;

            var direction = param.Trim().EndsWith(" desc") ? "descending" : "ascending";

            orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
        }

        return orderQueryBuilder.ToString().TrimEnd(',', ' ');
    }
}
