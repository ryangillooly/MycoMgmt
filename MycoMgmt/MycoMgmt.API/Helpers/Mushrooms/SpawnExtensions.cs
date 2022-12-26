using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Helpers;

public static class SpawnExtensions
{
    public static string Create(this Spawn spawn) =>
        $@"CREATE (x:{spawn.Tags[0]} {{ Name:  '{spawn.Name}' }}) RETURN x";
    
}