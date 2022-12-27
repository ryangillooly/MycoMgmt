using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Helpers;

public static class RecipeExtensions
{
    public static string Create(this Recipe recipe)
    {
        var additionalData = "";
        
        if (recipe.Notes != null)
            additionalData += $",Notes: '{recipe.Notes}'";

        if (recipe.Description != null)
            additionalData += $",Description: '{recipe.Description}'";
        
        if (recipe.Steps != null)
            additionalData += $",Steps: '{recipe.Steps.ToNumberedStringList()}'";
        
        var query = $@"CREATE 
                                (
                                    x:{recipe.Tags[0]} {{ 
                                                         Name: '{recipe.Name}',
                                                         Type: '{recipe.Type}'
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

        return query;
    }

    public static string? CreateIngredientRelationship(this Recipe recipe)
    {
        return
            recipe.Ingredients is null
                ? null
                : $@"
                      MATCH 
                          (recipe:{recipe.Tags[0]} {{ Name: '{recipe.Name}' }}), 
                          (i:Ingredient)
                      WHERE
                          i.Name IN ['{string.Join("','", recipe.Ingredients)}']
                      MERGE
                          (recipe)-[r:CREATED_USING]->(i)
                      RETURN r
                  ";
    }
    
    public static string? UpdateIngredientRelationship(this Recipe recipe, string elementId)
    {
        return
            recipe.Ingredients is null
                ? null
                : $@" 
                    MATCH 
                        (recipe:{recipe.Tags[0]})
                    WHERE
                        elementId(recipe) = '{elementId}'
                    OPTIONAL MATCH
                        (recipe)-[r:CREATED_USING]->(i)
                    DELETE
                        r
                    WITH
                        recipe
                    MATCH
                        (i:Ingredient)
                    WHERE
                        i.Name IN ['{string.Join("','", recipe.Ingredients)}']
                    MERGE 
                        (recipe)-[r:CREATED_USING]->(i)
                    RETURN
                        r  
                  ";
    }

    public static string ToNumberedStringList(this IEnumerable<string> recipeSteps) =>
        string.Join(", ", recipeSteps.Select((item, index) => $"{index + 1}.{item}"));
}