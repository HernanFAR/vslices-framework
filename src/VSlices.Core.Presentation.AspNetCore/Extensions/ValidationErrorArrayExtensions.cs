// ReSharper disable once CheckNamespace
namespace VSlices.Core.Abstracts.Responses;

#pragma warning disable CS1591
public static class ValidationErrorArrayExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Converts a <see cref="ValidationError"/> array to a dictionary
    /// </summary>
    /// <param name="errors">Validation error array</param>
    /// <returns>A dictionary with the property name as key and the errors as values</returns>
    public static Dictionary<string, string[]> ToDictionary(this ValidationError[] errors)
    {
        var dictionary = new Dictionary<string, string[]>();
        var propertyNames = errors.Select(x => x.Name).Distinct();

        foreach (var propertyName in propertyNames)
        {
            dictionary.Add(propertyName, errors.Where(x => x.Name == propertyName).Select(e => e.Detail).ToArray());
        }

        return dictionary;
    }
}
