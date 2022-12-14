using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;

namespace MycoMgmt.API.Repositories.Recipe
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;

        private ILogger<RecipeRepository> _logger;

        public RecipeRepository(INeo4JDataAccess neo4JDataAccess, ILogger<RecipeRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        public async Task<List<Dictionary<string, object>>> SearchByName(string searchString)
        {
            const string query = @"MATCH (r:Recipe) WHERE toUpper(r.name) CONTAINS toUpper($searchString) RETURN r{ name: r.name, type: r.type } ORDER BY r.Name LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", searchString } };

            var persons = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "p", parameters);

            return persons;
        }

        public async Task<string> Add(Domain.Models.Recipe recipe)
        {
            if (recipe != null && !string.IsNullOrWhiteSpace(recipe.Name))
            {
                var query = $@"
                                CREATE 
                                (
                                    :Recipe
                                    {{
                                        Name:  '{ recipe.Name        }',
                                        Type:  '{ recipe.Type        }',
                                        Desc:  '{ recipe.Description }',
                                        Steps: '{ recipe.Steps       }'
                                    }}        
                                ) 
                            ";

                 return await _neo4JDataAccess.ExecuteWriteTransactionAsync<string>(query);
            }
            else
            {
                throw new System.ArgumentNullException(nameof(recipe), "Recipe must not be null");
            }
        }

        public async Task<long> GetCount()
        {
            const string query = @"Match (r:Recipe) RETURN count(r) as recipeCount";
            var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
            return count;
        }
    }
}