using AutoFixture;
using Xunit;
using FluentAssertions;
using MycoMgmt.Domain.Models.Mushrooms;


namespace MycoMgmt.Tests.UnitTests;

public class MushroomTests
{
    private Mushroom mushroom { get; set; }

    public MushroomTests()
    {
        mushroom = new Fixture().Create<Mushroom>();
    }
    
    [Xunit.Theory]
    [InlineData(null,  null,  "InProgress")]
    [InlineData(false, null,  "InProgress")]
    [InlineData(true,  null,  "Failed")]
    [InlineData(true,  false, "Failed")]
    [InlineData(true,  true,  "Successful")]
    private void IsSuccessfulShouldReturnCorrectStatus(bool? finished, bool? successful, string expected)
    {
        mushroom.Finished  = finished;
        mushroom.Successful = successful;
        
        var status = mushroom.IsSuccessful();
        
        status.Should().Be(expected);
    }

    [Fact]
    private void CreateFinishedOnRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateFinishedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}' }}), 
                                    (d:Day {{ day:   {mushroom.FinishedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.FinishedOn.Value.Year} }})
                                CREATE
                                    (x)-[r:FINISHED_ON]->(d)
                                RETURN 
                                    r
                             ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateRecipeRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateRecipeRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (c:{mushroom.EntityType} {{ Name: '{mushroom.Name}'   }}),
                                    (recipe:Recipe {{ Name: '{mushroom.Recipe}' }})
                                CREATE
                                    (c)-[r:CREATED_USING]->(recipe)
                                RETURN 
                                    r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateStrainRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateStrainRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}'   }}), 
                                    (s:Strain  {{ Name: '{mushroom.Strain}' }})
                                CREATE
                                    (x)-[r:HAS_STRAIN]->(s)
                                RETURN r
                             ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateParentRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateParentRelationship()?.Replace(" ", "");
        var expected = $@"
                                  MATCH 
                                      (c:{mushroom.EntityType} {{ Name: '{mushroom.Name}' }}), 
                                      (p:{mushroom.ParentType} {{ Name: '{mushroom.Parent}' }})
                                  CREATE
                                      (c)-[r:HAS_PARENT]->(p)
                                  RETURN r
                              ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateInoculatedOnRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateInoculatedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}' }}), 
                                    (d:Day  {{ day:   {mushroom.InoculatedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.InoculatedOn.Value.Year} }})
                                CREATE
                                    (x)-[r:INOCULATED_ON]->(d)
                                RETURN r
                             ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateInoculatedRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateInoculatedRelationship()?.Replace(" ", "");
        var expected = $@"
                                  MATCH 
                                      (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}'}}),
                                      (u:User {{ Name: '{mushroom.InoculatedBy}'}})
                                  CREATE
                                      (u)-[r:INOCULATED]->(x)
                                  RETURN r
                              ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateChildRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateChildRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (p:{mushroom.EntityType} {{ Name: '{mushroom.Name}' }}), 
                                    (c:{mushroom.ChildType} {{ Name: '{mushroom.Children}' }})
                                CREATE
                                    (c)-[r:HAS_PARENT]->(p)
                                RETURN r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateLocationRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateLocationRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}  {{ Name: '{mushroom.Name}' }}), 
                                    (l:Location   {{ Name: '{mushroom.Location}' }})
                                CREATE
                                    (x)-[r:STORED_IN]->(l)
                                RETURN r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }

    [Fact]
    private void CreateVendorRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateVendorRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}'   }}),
                                    (v:Vendor            {{ Name: '{mushroom.Vendor}' }})
                                CREATE
                                    (x)-[r:PURCHASED_FROM]->(v)
                                RETURN 
                                    r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateParentRelationshipShouldReturnQuery()
    {
        var input   = mushroom.UpdateParentRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (c:{mushroom.EntityType})
                                WHERE
                                    elementId(c) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (c)-[r:HAS_PARENT]->(p)
                                DELETE
                                    r
                                WITH
                                    c
                                MATCH 
                                    (p:{mushroom.ParentType} {{Name: '{mushroom.Parent}' }})
                                CREATE 
                                    (c)-[r:HAS_PARENT]->(p) 
                                RETURN 
                                    r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateChildRelationshipShouldReturnQuery()
    {
        var input   = mushroom.UpdateChildRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (p:{mushroom.EntityType})
                                WHERE
                                    elementId(p) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (c)-[r:HAS_PARENT]->(p)
                                DELETE
                                    r
                                WITH
                                    p
                                MATCH 
                                    (c:{mushroom.ChildType} {{Name: '{mushroom.Children}' }})
                                CREATE 
                                    (c)-[r:HAS_PARENT]->(p) 
                                RETURN 
                                    r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateStrainRelationshipShouldReturnQuery()
    {
        var input   = mushroom.UpdateStrainRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    elementId(x) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (x)-[r:HAS_STRAIN]->(:Strain)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (s:Strain {{ Name: '{mushroom.Strain}' }})
                                CREATE
                                    (x)-[r:HAS_STRAIN]->(s)
                                RETURN 
                                    r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateLocationRelationshipShouldReturnQuery()
    {
        var input = mushroom.UpdateLocationRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    elementId(x) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (x)-[r:STORED_IN]->(:Location)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (l:Location {{ Name: '{mushroom.Location}' }})
                                CREATE
                                    (x)-[r:STORED_IN]->(l) 
                                RETURN 
                                    r
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }
    
    [Fact]
    private void UpdateRecipeRelationshipShouldReturnQuery()
    {
        var input = mushroom.UpdateRecipeRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (c:{mushroom.EntityType})
                                WHERE
                                    elementId(c) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (c)-[r:CREATED_USING]->(:Recipe)
                                DELETE 
                                    r
                                WITH
                                    c
                                MATCH
                                    (recipe:Recipe {{ Name: '{mushroom.Recipe}' }})
                                CREATE
                                    (c)-[r:CREATED_USING]->(recipe)
                                RETURN 
                                    r
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateStatusLabelShouldReturnQuery()
    {
        var input = mushroom.UpdateStatusLabel()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE 
                                    elementId(x) = '{mushroom.ElementId}'
                                REMOVE 
                                    x :InProgress:Successful:Failed
                                WITH 
                                    x                    
                                SET 
                                    x:{mushroom.IsSuccessful()}
                                RETURN 
                                    x
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateStatusShouldReturnQuery()
    {
        var input = mushroom.UpdateStatus()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE 
                                    elementId(x) = '{mushroom.ElementId}'
                                SET 
                                    x {{ Status: '{mushroom.IsSuccessful()}' }}
                                RETURN 
                                    x
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateInoculatedRelationshipShouldReturnQuery()
    {
        var input = mushroom.UpdateInoculatedRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    elementId(x) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (u:User)-[r:INOCULATED]->(x)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (u:User {{ Name: '{mushroom.InoculatedBy}' }})
                                CREATE
                                    (u)-[r:INOCULATED]->(x)
                                RETURN 
                                    r
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateInoculatedOnRelationshipShouldReturnQuery()
    {
        var input = mushroom.UpdateInoculatedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    elementId(x) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (x)-[r:INOCULATED_ON]->(d)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (d:Day {{ day: {mushroom.InoculatedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.InoculatedOn.Value.Year} }})
                                CREATE
                                    (x)-[r:INOCULATED_ON]->(d)
                                RETURN 
                                    r
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateFinishedOnRelationshipShouldReturnQuery()
    {
        var input = mushroom.UpdateFinishedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    elementId(x) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (x)-[r:FINISHED_ON]->(d)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (d:Day {{ day: {mushroom.FinishedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.FinishedOn.Value.Year} }})
                                CREATE
                                    (x)-[r:FINISHED_ON]->(d)
                                RETURN 
                                    r
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }
    
    [Fact]
    private void UpdateVendorRelationshipShouldReturnQuery()
    {
        var input = mushroom.UpdateVendorRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    elementId(x) = '{mushroom.ElementId}'
                                OPTIONAL MATCH
                                    (x)-[r:PURCHASED_FROM]->(v)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (v:Vendor  {{ Name: '{mushroom.Vendor}' }})
                                CREATE
                                    (x)-[r:PURCHASED_FROM]->(v)
                                RETURN 
                                    r
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }
}