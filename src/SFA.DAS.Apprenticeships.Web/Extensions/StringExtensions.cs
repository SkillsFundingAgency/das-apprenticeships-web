using System.Text.Json;
using System.Web;

namespace SFA.DAS.Apprenticeships.Web.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Takes a json object and returns a flat dictionary of key value pairs
		/// </summary>
		public static Dictionary<string, JsonElement> GetFlatJson(this string json)
		{
			using (JsonDocument document = JsonDocument.Parse(json))

			return document.RootElement.EnumerateObject()
				.SelectMany(p => GetLeaves(null, p))
				.ToDictionary(k => k.Path, v => v.P.Value.Clone()); //Clone so that we can use the values outside of using
		}

		/// <summary>
		/// Encodes a string to encode html characters, it will return an empty string if the value is null
		/// </summary>
        public static string HtmlEncode(this string value)
        {
            return value != null ? HttpUtility.HtmlEncode(value) : string.Empty;
        }

        /// <summary>
        /// Recursively gets the leaves of a json object
        /// </summary>
        private static IEnumerable<(string Path, JsonProperty P)> GetLeaves(string? parentPath, JsonProperty property)
		{
			var path = parentPath == null ? property.Name : parentPath + "." + property.Name;

			if(property.Value.ValueKind != JsonValueKind.Object)
			{
				return new[] { (Path: path, property) };
			}

			return property.Value.EnumerateObject().SelectMany(child => GetLeaves(path, child));
			
		}
	}
}
