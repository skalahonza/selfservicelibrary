using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;

using SelfServiceLibrary.Web.Attributes;

namespace SelfServiceLibrary.Web.Extensions
{
    public static class QueryStringParameterExtensions
    {
        // Apply the values from the query string to the current component
        public static void SetParametersFromQueryString<T>(this T component, NavigationManager navigationManager)
            where T : ComponentBase
        {
            if (!Uri.TryCreate(navigationManager.Uri, UriKind.RelativeOrAbsolute, out var uri))
                throw new InvalidOperationException("The current url is not a valid URI. Url: " + navigationManager.Uri);

            // Parse the query string
            var queryString = QueryHelpers.ParseQuery(uri.Query);

            // Enumerate all properties of the component
            foreach (var property in GetProperties<T>())
            {
                // Get the name of the parameter to read from the query string
                var parameterName = GetQueryStringParameterName(property);
                if (parameterName == null)
                    continue; // The property is not decorated by [QueryStringParameterAttribute]

                if (queryString.TryGetValue(parameterName, out var value))
                {
                    // Convert the value from string to the actual property type
                    var convertedValue = ConvertValue(value, property.PropertyType);
                    property.SetValue(component, convertedValue);
                }
            }
        }

        private static string BuildNewUri<T>(T component, NavigationManager navigationManager)
            where T : ComponentBase
        {
            if (!Uri.TryCreate(navigationManager.Uri, UriKind.RelativeOrAbsolute, out var uri))
                throw new InvalidOperationException("The current url is not a valid URI. Url: " + navigationManager.Uri);

            // Fill the dictionary with the parameters of the component
            var parameters = QueryHelpers.ParseQuery(uri.Query);
            foreach (var property in GetProperties<T>())
            {
                var parameterName = GetQueryStringParameterName(property);
                if (parameterName == null)
                    continue;

                var value = property.GetValue(component);
                if (value is null)
                {
                    parameters.Remove(parameterName);
                }
                else
                {
                    parameters[parameterName] = ConvertToString(value);
                }
            }

            // Compute the new URL
            var newUri = uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped);
            foreach (var parameter in parameters)
            {
                foreach (var value in parameter.Value)
                {
                    newUri = QueryHelpers.AddQueryString(newUri, parameter.Key, value);
                }
            }

            // https://github.com/dotnet/aspnetcore/issues/16840
            // replace the %20 to + before the signalR method invoked
            return newUri.Replace("%20", "+");
        }

        public static ValueTask UpdateQueryString<T>(this T component, NavigationManager navigationManager, IJSRuntime jsRuntime)
            where T : ComponentBase
        {
            var newUri = BuildNewUri(component, navigationManager);
#pragma warning disable CS8625 // JavaScript method invocation
            return jsRuntime.InvokeVoidAsync("window.history.replaceState", null, "", newUri);
#pragma warning restore CS8625 // JavaScript method invocation
        }

        private static object ConvertValue(StringValues value, Type type) =>
            Convert.ChangeType(value[0], type, CultureInfo.InvariantCulture);

        private static string? ConvertToString(object value) =>
            Convert.ToString(value, CultureInfo.InvariantCulture);

        private static PropertyInfo[] GetProperties<T>() =>
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static string? GetQueryStringParameterName(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<QueryStringParameterAttribute>();
            if (attribute == null)
                return null;

            return attribute.Name ?? property.Name;
        }
    }
}
