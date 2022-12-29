namespace MycoMgmt.Domain.Helpers;

public static class RecipeExtensions
{
    public static string ToNumberedStringList(this IEnumerable<string> recipeSteps) =>
        string.Join(", ", recipeSteps.Select((item, index) => $"{index + 1}.{item}"));    
}