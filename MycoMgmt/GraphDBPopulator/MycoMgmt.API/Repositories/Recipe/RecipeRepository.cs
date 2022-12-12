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

        /// <summary>
        /// Initializes a new instance of the "PersonRepository" class.
        /// </summary>
        public RecipeRepository(INeo4JDataAccess neo4JDataAccess, ILogger<RecipeRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        /// <summary>
        /// Searches the name of the person.
        /// </summary>
        public async Task<List<Dictionary<string, object>>> SearchRecipeByName(string searchString)
        {
            const string query = @"MATCH (r:Recipe) WHERE toUpper(r.name) CONTAINS toUpper($searchString) RETURN r{ name: r.name, type: r.type } ORDER BY r.Name LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", searchString } };

            var persons = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "p", parameters);

            return persons;
        }

        /// <summary>
        /// Adds a new person
        /// </summary>
        public async Task<string> AddRecipe(Models.Recipe recipe)
        {
            if (recipe != null && !string.IsNullOrWhiteSpace(recipe.Name))
            {
                var query = $@"
                                CREATE 
                                (
                                    :Recipe
                                    {{
                                        Id:    '{ recipe.Id          }',
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

        /// <summary>
        /// Get count of persons
        /// </summary>
        public async Task<long> GetRecipeCount()
        {
            const string query = @"Match (r:Recipe) RETURN count(r) as recipeCount";
            var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
            return count;
        }
    }
}