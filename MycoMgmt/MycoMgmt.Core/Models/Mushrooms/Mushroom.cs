namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Mushroom : ModelBase
    {
        // Properties
        public string? Strain { get; set; }
        public string? Location { get; set; }
        public string? Parent { get; set; }
        public string? ParentType { get; set; }
        public string? Children { get; set; }
        public string? ChildType { get; set; }
        public string? Status { get; set; }
        public bool? Successful { get; set; }
        public bool? Finished { get; set; }
        public DateTime? FinishedOn { get; set; }
        public DateTime? InoculatedOn { get; set; }
        public string? InoculatedBy { get; set; }
        public string? Recipe { get; set; }
        public bool? Purchased { get; set; }
        public string? Vendor { get; set; }
     
        
        // Create
        public virtual string? CreateFinishedOnRelationship()
        {
            return
                FinishedOn is null
                    ? null
                    : $@"
                            MATCH 
                                (x:{EntityType} {{ Name: '{Name}' }}), 
                                (d:Day {{ day:   {FinishedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {FinishedOn.Value.Year} }})
                            CREATE
                                (x)-[r:FINISHED_ON]->(d)
                            RETURN 
                                r
                      ";
        }
        public virtual string? CreateRecipeRelationship()
        {
            return
                Recipe is null
                    ? null
                    : $@"
                        MATCH 
                            (c:{EntityType} {{ Name: '{Name}'   }}),
                            (recipe:Recipe {{ Name: '{Recipe}' }})
                        CREATE
                            (c)-[r:CREATED_USING]->(recipe)
                        RETURN 
                            r
                    ";
        }
        public virtual string? CreateStrainRelationship()
        {
            return
                Strain is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType} {{ Name: '{Name}'   }}), 
                            (s:Strain  {{ Name: '{Strain}' }})
                        CREATE
                            (x)-[r:HAS_STRAIN]->(s)
                        RETURN r
                    ";
        }
        public virtual string? CreateParentRelationship()
        {
            return
                Parent is null
                    ? null
                    : $@"
                          MATCH 
                              (c:{EntityType} {{ Name: '{Name}' }}), 
                              (p:{ParentType} {{ Name: '{Parent}' }})
                          CREATE
                              (c)-[r:HAS_PARENT]->(p)
                          RETURN r
                      ";
        }
        public virtual string? CreateInoculatedOnRelationship()
        {
            return
                InoculatedOn is null
                    ? null
                    : $@"
                            MATCH 
                                (x:{EntityType} {{ Name: '{Name}' }}), 
                                (d:Day  {{ day:   {InoculatedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {InoculatedOn.Value.Year} }})
                            CREATE
                                (x)-[r:INOCULATED_ON]->(d)
                            RETURN r
                      ";
        }
        public virtual string? CreateInoculatedRelationship()
        {
            return
                InoculatedBy is null
                    ? null
                    : $@"
                          MATCH 
                              (x:{EntityType} {{ Name: '{Name}'         }}),
                              (u:User  {{ Name: '{InoculatedBy}' }})
                          CREATE
                              (u)-[r:INOCULATED]->(x)
                          RETURN r
                      ";
        }
        public virtual string? CreateChildRelationship()
        {
            return 
                Children is null 
                    ? null 
                    : $@"
                        MATCH 
                            (p:{EntityType} {{ Name: '{Name}' }}), 
                            (c:{ChildType} {{ Name: '{Children}' }})
                        CREATE
                            (c)-[r:HAS_PARENT]->(p)
                        RETURN r
                    ";
        }
        public virtual string? CreateLocationRelationship()
        {
            return
                Location is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType}  {{ Name: '{Name}'     }}), 
                            (l:Location   {{ Name: '{Location}' }})
                        CREATE
                            (x)-[r:STORED_IN]->(l)
                        RETURN r
                    ";
        }
        public virtual string? CreateVendorRelationship()
        {
            return
                Vendor is null
                    ? null
                    : $@"
                            MATCH 
                                (x:{EntityType} {{ Name: '{Name}'   }}),
                                (v:Vendor            {{ Name: '{Vendor}' }})
                            CREATE
                                (x)-[r:PURCHASED_FROM]->(v)
                            RETURN 
                                r
                        ";
        }
        
        
        // Update
        public virtual string? UpdateParentRelationship()
        {
            return
                Parent is null
                    ? null
                    : $@"
                        MATCH 
                            (c:{EntityType})
                        WHERE
                            c.Id = '{Id}'
                        OPTIONAL MATCH
                            (c)-[r:HAS_PARENT]->(p)
                        DELETE
                            r
                        WITH
                            c
                        MATCH 
                            (p:{ParentType} {{Name: '{Parent}' }})
                        CREATE 
                            (c)-[r:HAS_PARENT]->(p) 
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateChildRelationship()
        {
            return
                Children is null
                    ? null
                    : $@"
                        MATCH 
                            (p:{EntityType})
                        WHERE
                            p.Id = '{Id}'
                        OPTIONAL MATCH
                            (c)-[r:HAS_PARENT]->(p)
                        DELETE
                            r
                        WITH
                            p
                        MATCH 
                            (c:{ChildType} {{Name: '{Children}' }})
                        CREATE 
                            (c)-[r:HAS_PARENT]->(p) 
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateStrainRelationship()
        {
            return
                Strain is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            x.Id = '{Id}'
                        OPTIONAL MATCH
                            (x)-[r:HAS_STRAIN]->(:Strain)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (s:Strain {{ Name: '{Strain}' }})
                        CREATE
                            (x)-[r:HAS_STRAIN]->(s)
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateLocationRelationship()
        {
            return
                Location is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            x.Id = '{Id}'
                        OPTIONAL MATCH
                            (x)-[r:STORED_IN]->(:Location)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (l:Location {{ Name: '{Location}' }})
                        CREATE
                            (x)-[r:STORED_IN]->(l) 
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateRecipeRelationship()
        {
            return
                Recipe is null
                    ? null
                    : $@"
                        MATCH 
                            (c:{EntityType})
                        WHERE
                            c.Id = '{Id}'
                        OPTIONAL MATCH
                            (c)-[r:CREATED_USING]->(:Recipe)
                        DELETE 
                            r
                        WITH
                            c
                        MATCH
                            (recipe:Recipe {{ Name: '{Recipe}' }})
                        CREATE
                            (c)-[r:CREATED_USING]->(recipe)
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateStatusLabel()
        {
            return
                Successful is null && Finished is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE 
                            x.Id = '{Id}'
                        REMOVE 
                            x :InProgress:Successful:Failed
                        WITH 
                            x                    
                        SET 
                            x:{IsSuccessful()}
                        RETURN 
                            x
                    ";
        }
        public virtual string? UpdateStatus()
        {
            return
                Successful is null && Finished is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE 
                            x.Id = '{Id}'
                        SET 
                            x {{ Status: '{IsSuccessful()}' }}
                        RETURN 
                            x
                    ";
        }
        public virtual string? UpdateInoculatedRelationship()
        {
            return
                InoculatedBy is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            x.Id = '{Id}'
                        OPTIONAL MATCH
                            (u:User)-[r:INOCULATED]->(x)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (u:User {{ Name: '{InoculatedBy}' }})
                        CREATE
                            (u)-[r:INOCULATED]->(x)
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateInoculatedOnRelationship()
        {
            return
                InoculatedOn is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            x.Id = '{Id}'
                        OPTIONAL MATCH
                            (x)-[r:INOCULATED_ON]->(d)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (d:Day {{ day: {InoculatedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {InoculatedOn.Value.Year} }})
                        CREATE
                            (x)-[r:INOCULATED_ON]->(d)
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateFinishedOnRelationship()
        {
            return
                FinishedOn is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            x.Id = '{Id}'
                        OPTIONAL MATCH
                            (x)-[r:FINISHED_ON]->(d)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (d:Day {{ day: {FinishedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {FinishedOn.Value.Year} }})
                        CREATE
                            (x)-[r:FINISHED_ON]->(d)
                        RETURN 
                            r
                    ";
        }
        public virtual string? UpdateVendorRelationship()
        {
            return
                Vendor is null
                    ? null
                    : $@"
                            MATCH 
                                (x:{EntityType})
                            WHERE
                                x.Id = '{Id}'
                            OPTIONAL MATCH
                                (x)-[r:PURCHASED_FROM]->(v)
                            DELETE 
                                r
                            WITH
                                x
                            MATCH
                                (v:Vendor  {{ Name: '{Vendor}' }})
                            CREATE
                                (x)-[r:PURCHASED_FROM]->(v)
                            RETURN 
                                r
                        ";
        }
        
        
        // Read
        public virtual string IsSuccessful()
        {
            if (Finished is null || (bool)!Finished) 
                return "InProgress";

            return 
                Successful switch
                {
                    true  => "Successful",
                    _ => "Failed"
                };
        }
    }
}