﻿
namespace MycoMgmt.Domain.Models
{
    public class Strain : ModelBase
    {
        public Strain()
        {
            Tags.Add(GetType().Name);
        }

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
                                                     Name: '{Name}'
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
                            elementId(x) = '{ElementId}' 
                        SET 
                            x.Effects = '{Name}' 
                        RETURN 
                            x
                      ";
        }
    }
}