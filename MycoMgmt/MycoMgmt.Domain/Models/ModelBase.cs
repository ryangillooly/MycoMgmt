using System;
using System.Runtime.CompilerServices;

#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models
{
    public abstract class ModelBase
    {
        // Properties
        public string? ElementId { get; set; }
        public List<string> Tags { get; set; } = new ();
        public string? EntityType { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public string   CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string?   ModifiedBy { get; set; }
        
        
        // Create
        public virtual string CreateNode()
        {
            var additionalData = "";

            if (Notes != null) additionalData += $",Notes: '{Notes}'";
            if (Type != null) additionalData += $",Type: '{Type}'";

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
        public virtual string? CreateNodeLabels()
        {
            return
                $@"
                    MATCH (x:{Tags[0]} {{ Name: '{Name}' }})
                    SET x:{string.Join(":", Tags)}
                    RETURN x
                ";
        }
        public virtual List<string?> CreateQueryList()
        {
            var queryList = new List<string>
            {
                CreateNode(),
                CreateCreatedRelationship(),
                CreateCreatedOnRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        public virtual List<string?> UpdateQueryList()
        {
            var queryList = new List<string>
            {
                UpdateName(),
                UpdateType(),
                UpdateNotes(),
                UpdateModifiedOnRelationship(),
                UpdateModifiedRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        public virtual string? CreateCreatedRelationship()
        {
            return
                $@"
                    MATCH 
                        (x:{Tags[0]} {{ Name: '{Name}'      }}),
                        (u:User               {{ Name: '{CreatedBy}' }})
                    CREATE
                        (u)-[r:CREATED]->(x)
                    RETURN r
                ";
        }
        public virtual string? CreateCreatedOnRelationship()
        {
            return
                $@"
                    MATCH 
                        (x:{Tags[0]} {{ Name: '{Name}' }}), 
                        (d:Day                {{ day:   {CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {CreatedOn.Year} }})
                    CREATE
                        (x)-[r:CREATED_ON]->(d)
                    RETURN r
                ";
        }
       
        // Read
        public virtual string SearchByNameQuery()
        {
            return
                $@"
                    MATCH 
                        (x:{Tags[0]}) 
                    WHERE 
                        toUpper(x.Name) CONTAINS toUpper('{Name}') 
                    RETURN 
                        x 
                    ORDER BY 
                        x.Name ASC
                    LIMIT 
                        100
                ";
        }
        public virtual string GetByNameQuery()
        {
            return
                $@"
                    MATCH 
                        (x:{Tags[0]}) 
                    WHERE 
                        toUpper(x.Name) = toUpper('{Name}') 
                    RETURN 
                        x
                ";
        }
        public virtual string GetByIdQuery()
        {
            return
                $@"
                    MATCH 
                        (x:{Tags[0]}) 
                    WHERE 
                        elementId(x) = '{ElementId}'
                    RETURN 
                        x
                ";
        }
        public virtual string GetAllQuery(int skip, int limit)
        {
            return
                $@"
                    MATCH 
                        (x:{Tags[0]})
                    OPTIONAL MATCH 
                        (x)-[r:INOCULATED_ON]->(d:Day)<-[rd:HAS_DAY]-(m:Month)-[:HAS_MONTH]-(y:Year)
                    WITH 
                        x, y, m, d
                    RETURN 
                        x as Culture, 
                        datetime({{year: y.year, month: m.month, day: d.day}}) as InoculationDate
                    ORDER BY
                        d.day   DESC,
                        m.month DESC,
                        y.year  DESC
                    SKIP 
                        {skip}
                    LIMIT
                        {limit}
                ";
            
            
        }
        
        // Update
        public virtual string? UpdateName()
        {
            return
                Name is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{Tags[0]}) 
                        WHERE 
                            elementId(x) = '{ElementId}' 
                        SET 
                            x.Name = '{Name}' 
                        RETURN 
                            x
                      ";    
        }
        public virtual string? UpdateModifiedOnRelationship()
        {
            DateTime.TryParse(ModifiedOn.ToString(), out var parsedDateTime);

            var query = $@"
                                MATCH 
                                    (x:{Tags[0]})
                                WHERE
                                    elementId(x) = '{ElementId}'
                                OPTIONAL MATCH
                                    (x)-[r:MODIFIED_ON]->(d)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (d:Day {{ day: {parsedDateTime.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {parsedDateTime.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {parsedDateTime.Year} }})
                                MERGE
                                    (x)-[r:MODIFIED_ON]->(d)
                                RETURN 
                                    r
                            ";

            return query;
        }
        public virtual string? UpdateModifiedRelationship()
        {
            return
                $@"
                    MATCH 
                        (x:{Tags[0]})
                    WHERE
                        elementId(x) = '{ElementId}'
                    OPTIONAL MATCH
                        (u)-[r:MODIFIED]->(x)
                    DELETE
                        r
                    WITH
                        x
                    MATCH
                        (u:User {{ Name: '{ModifiedBy}'}} )
                    MERGE 
                        (u)-[r:MODIFIED]->(x)
                    RETURN
                        r  
                ";
        }
        public virtual string? UpdateNotes()
        {
            return
                Notes is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{Tags[0]}) 
                        WHERE 
                            elementId(x) = '{ElementId}' 
                        SET 
                            x.Notes = '{Notes}' 
                        RETURN s 
                      ";
        }
        public virtual string? UpdateType()
        {
            return
                Type is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{Tags[0]}) 
                        WHERE 
                            elementId(x) = '{ElementId}' 
                        SET 
                            x.Type = '{Type}' 
                        RETURN s 
                      ";
        }

        // Delete
        public virtual string? Delete()
        {
            return
                ElementId is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{Tags[0]}) 
                        WHERE 
                            elementId(x) = '{ ElementId }' 
                        DETACH DELETE 
                            x
                        RETURN 
                            x
                    ";
        }
    }
}