
namespace MycoMgmt.Domain.Models
{
    public class Strain : ModelBase
    {
        public string? Effects { get; set; }
        
        
        // Create
        public override string CreateNode()
        {
            var additionalData = "";
        
            if (Effects != null)
                additionalData += $",Effects: '{Effects}'";
        
            var query = $@"
                            CREATE 
                            (
                                x:{EntityType} {{ 
                                                     Name: '{Name}',
                                                     Id: '{Id}'
                                                     {additionalData} 
                                                  }}
                            )
                            RETURN x
                            ";

            return query;
        }

        // Update
        public override List<string> UpdateQueryList()
        {
            var queryList = new List<string>
            {
                UpdateName(),
                UpdateEffects(),
                UpdateModifiedRelationship(),
                UpdateModifiedOnRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        
        public string? UpdateEffects()
        {
            return
                Name is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType}) 
                        WHERE 
                            x.Id = '{Id}' 
                        SET 
                            x.Effects = '{Name}' 
                        RETURN 
                            x
                      ";
        }
        
        public override string GetAllQuery(int skip = 0, int limit = 0) =>
            "MATCH (s:Strain) WHERE s.Name IS NOT NULL RETURN s.Name as result";
    }
}