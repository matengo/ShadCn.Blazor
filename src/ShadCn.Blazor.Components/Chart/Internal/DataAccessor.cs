using System.Collections;
using System.Reflection;

namespace ShadCn.Blazor.Components.Chart.Internal;

/// <summary>
/// Helper for extracting values from data items by property name (DataKey).
/// Supports Dictionary&lt;string, object&gt; and POCO objects via reflection.
/// </summary>
public static class DataAccessor
{
    /// <summary>
    /// Gets a double value from a data item by key.
    /// </summary>
    public static double GetDouble(object item, string key)
    {
        var raw = GetValue(item, key);
        return raw switch
        {
            null => 0,
            double d => d,
            int i => i,
            float f => f,
            long l => l,
            decimal m => (double)m,
            _ => Convert.ToDouble(raw)
        };
    }

    /// <summary>
    /// Gets a string value from a data item by key.
    /// </summary>
    public static string GetString(object item, string key)
    {
        var raw = GetValue(item, key);
        return raw?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the raw value from a data item by key.
    /// </summary>
    public static object? GetValue(object item, string key)
    {
        if (item is IDictionary<string, object> dict)
        {
            return dict.TryGetValue(key, out var val) ? val : null;
        }

        if (item is IDictionary nonGenericDict)
        {
            return nonGenericDict.Contains(key) ? nonGenericDict[key] : null;
        }

        // Reflection fallback for POCO
        var type = item.GetType();
        var prop = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return prop?.GetValue(item);
    }

    /// <summary>
    /// Extracts all double values for a given key from a data collection.
    /// </summary>
    public static double[] GetAllDoubles(IEnumerable<object> data, string key) =>
        data.Select(item => GetDouble(item, key)).ToArray();

    /// <summary>
    /// Extracts all string values for a given key from a data collection.
    /// </summary>
    public static string[] GetAllStrings(IEnumerable<object> data, string key) =>
        data.Select(item => GetString(item, key)).ToArray();
}
