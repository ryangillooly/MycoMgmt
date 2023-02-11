using System;
using System.Runtime.CompilerServices;
using MycoMgmt.Core.Extensions;

#pragma warning disable CS8618

namespace MycoMgmt.Core.Models
{
    public class ModelBase
    {
        public ModelBase()
        {
            Tags.Add(GetType().Name);
            EntityType = GetType().Name;
            Id         = Guid.NewGuid();
        }
        
        // Properties
        public Guid Id { get; set; }
        public string? ElementId { get; set; }
        public List<string> Tags { get; set; } = new ();
        public string? EntityType { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string?   CreatedBy { get; set; }
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
                                                 Name: '{Name}',
                                                 Id: '{Id}'
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
                    MATCH (x:{EntityType} {{ Name: '{Name}' }})
                    SET x:{string.Join(":", Tags)}
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
                        (x:{EntityType} {{ Name: '{Name}'      }}),
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
                        (x:{EntityType} {{ Name: '{Name}' }}), 
                        (d:Day                {{ day:   {CreatedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {CreatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {CreatedOn.Value.Year} }})
                    CREATE
                        (x)-[r:CREATED_ON]->(d)
                    RETURN 
                        r
                ";
        }
       
        // Read
        public virtual string GetByNameQuery() => this.ToGetQuery(name: Name);
        public virtual string GetByIdQuery() => this.ToGetQuery(id: Id);
        public virtual string GetAllQuery(int skip = 0, int limit = 20) => this.ToGetQuery(skip: skip, limit:limit);
        public virtual string SearchByNameQuery(int skip = 0, int limit = 20) => this.ToGetQuery(searchName: Name, skip: skip, limit: limit);
        
        // Update
        public virtual string? UpdateName()
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
                                    (x:{EntityType})
                                WHERE
                                    x.Id = '{Id}'
                                OPTIONAL MATCH
                                    (x)-[r:MODIFIED_ON]->(d)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (d:Day {{ day: {parsedDateTime.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {parsedDateTime.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {parsedDateTime.Year} }})
                                CREATE
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
                        (x:{EntityType})
                    WHERE
                        x.Id = '{Id}'
                    OPTIONAL MATCH
                        (u)-[r:MODIFIED]->(x)
                    DELETE
                        r
                    WITH
                        x
                    MATCH
                        (u:User {{ Name: '{ModifiedBy}'}} )
                    CREATE 
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
                            (x:{EntityType}) 
                        WHERE 
                            x.Id = '{Id}' 
                        SET 
                            x.Notes = '{Notes}' 
                        RETURN 
                            x
                      ";
        }
        public virtual string? UpdateType()
        {
            return
                Type is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType}) 
                        WHERE 
                            x.Id = '{Id}' 
                        SET 
                            x.Type = '{Type}' 
                        RETURN 
                            x
                      ";
        }

        // Delete
        public virtual string? Delete()
        {
            return
                /*Id is null
                    ? null
                    :
                */ 
                    $@"
                        MATCH 
                            (x:{EntityType}) 
                        WHERE 
                            x.Id = '{ Id }' 
                        DETACH DELETE 
                            x
                        RETURN 
                            x
                    ";
        }
    }
}