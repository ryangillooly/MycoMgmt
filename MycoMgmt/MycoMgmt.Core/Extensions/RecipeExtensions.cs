using MycoMgmt.Domain.Models;

namespace MycoMgmt.Core.Helpers;

public static class RecipeExtensions
{
    public static void SplitStepsToList(this Recipe recipe, string? steps)
    {
        if (steps == null) return;
            
        recipe.Steps = steps.Split(",").ToList();
            
        for (var i = 0; i < recipe.Steps.Count; i++)
        {
            recipe.Steps[i] = recipe.Steps[i].Trim();
        }
    }

    public static void SplitIngredientsToList(this Recipe recipe, string? ingredients)
    {
        if (ingredients is null) return;
            
        recipe.Ingredients = ingredients.Split(",").ToList();

        for (var i = 0; i < recipe.Ingredients.Count; i++)
        {
            recipe.Ingredients[i] = recipe.Ingredients[i].Trim();
        }
    }
}