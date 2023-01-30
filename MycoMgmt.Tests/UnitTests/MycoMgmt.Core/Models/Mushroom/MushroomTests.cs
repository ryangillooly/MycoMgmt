using AutoFixture;
using Xunit;
using FluentAssertions;
using MycoMgmt.Core.Models.Mushrooms;

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Tests.UnitTests;

public class MushroomTests
{
    private Mushroom Mushroom { get; set; }

    public MushroomTests()
    {
        Mushroom = new Fixture().Create<Mushroom>();
    }
    
    [Theory]
    [InlineData(null,  null,  "InProgress")]
    [InlineData(false, null,  "InProgress")]
    [InlineData(true,  null,  "Failed")]
    [InlineData(true,  false, "Failed")]
    [InlineData(true,  true,  "Successful")]
    private void IsSuccessfulShouldReturnCorrectStatus(bool? finished, bool? successful, string expected)
    {
        Mushroom.Finished  = finished;
        Mushroom.Successful = successful;
        
        var status = Mushroom.IsSuccessful();
        
        status.Should().Be(expected);
    }

    [Fact]
    private void CreateFinishedOnRelationshipShouldReturnQuery()
    {
        var input   = Mushroom.CreateFinishedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}' }}), 
                                    (d:Day {{ day:   {Mushroom.FinishedOn!.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {Mushroom.FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {Mushroom.FinishedOn.Value.Year} }})
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
        var input   = Mushroom.CreateRecipeRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (c:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}'   }}),
                                    (recipe:Recipe {{ Name: '{Mushroom.Recipe}' }})
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
        var input   = Mushroom.CreateStrainRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}'   }}), 
                                    (s:Strain  {{ Name: '{Mushroom.Strain}' }})
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
        var input   = Mushroom.CreateParentRelationship()?.Replace(" ", "");
        var expected = $@"
                                  MATCH 
                                      (c:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}' }}), 
                                      (p:{Mushroom.ParentType} {{ Name: '{Mushroom.Parent}' }})
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
        var input   = Mushroom.CreateInoculatedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}' }}), 
                                    (d:Day  {{ day:   {Mushroom.InoculatedOn!.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {Mushroom.InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {Mushroom.InoculatedOn.Value.Year} }})
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
        var input   = Mushroom.CreateInoculatedRelationship()?.Replace(" ", "");
        var expected = $@"
                                  MATCH 
                                      (x:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}'}}),
                                      (u:User {{ Name: '{Mushroom.InoculatedBy}'}})
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
        var input   = Mushroom.CreateChildRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (p:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}' }}), 
                                    (c:{Mushroom.ChildType} {{ Name: '{Mushroom.Children}' }})
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
        var input   = Mushroom.CreateLocationRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType}  {{ Name: '{Mushroom.Name}' }}), 
                                    (l:Location   {{ Name: '{Mushroom.Location}' }})
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
        var input   = Mushroom.CreateVendorRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType} {{ Name: '{Mushroom.Name}'   }}),
                                    (v:Vendor            {{ Name: '{Mushroom.Vendor}' }})
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
        var input   = Mushroom.UpdateParentRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (c:{Mushroom.EntityType})
                                WHERE
                                    c.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (c)-[r:HAS_PARENT]->(p)
                                DELETE
                                    r
                                WITH
                                    c
                                MATCH 
                                    (p:{Mushroom.ParentType} {{Name: '{Mushroom.Parent}' }})
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
        var input   = Mushroom.UpdateChildRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (p:{Mushroom.EntityType})
                                WHERE
                                    p.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (c)-[r:HAS_PARENT]->(p)
                                DELETE
                                    r
                                WITH
                                    p
                                MATCH 
                                    (c:{Mushroom.ChildType} {{Name: '{Mushroom.Children}' }})
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
        var input   = Mushroom.UpdateStrainRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE
                                    x.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (x)-[r:HAS_STRAIN]->(:Strain)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (s:Strain {{ Name: '{Mushroom.Strain}' }})
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
        var input = Mushroom.UpdateLocationRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE
                                    x.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (x)-[r:STORED_IN]->(:Location)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (l:Location {{ Name: '{Mushroom.Location}' }})
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
        var input = Mushroom.UpdateRecipeRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (c:{Mushroom.EntityType})
                                WHERE
                                    c.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (c)-[r:CREATED_USING]->(:Recipe)
                                DELETE 
                                    r
                                WITH
                                    c
                                MATCH
                                    (recipe:Recipe {{ Name: '{Mushroom.Recipe}' }})
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
        var input = Mushroom.UpdateStatusLabel()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE 
                                    x.Id = '{Mushroom.Id}'
                                REMOVE 
                                    x :InProgress:Successful:Failed
                                WITH 
                                    x                    
                                SET 
                                    x:{Mushroom.IsSuccessful()}
                                RETURN 
                                    x
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateStatusShouldReturnQuery()
    {
        var input = Mushroom.UpdateStatus()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE 
                                    x.Id = '{Mushroom.Id}'
                                SET 
                                    x {{ Status: '{Mushroom.IsSuccessful()}' }}
                                RETURN 
                                    x
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }

    [Fact]
    private void UpdateInoculatedRelationshipShouldReturnQuery()
    {
        var input = Mushroom.UpdateInoculatedRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE
                                    x.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (u:User)-[r:INOCULATED]->(x)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (u:User {{ Name: '{Mushroom.InoculatedBy}' }})
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
        var input = Mushroom.UpdateInoculatedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE
                                    x.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (x)-[r:INOCULATED_ON]->(d)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (d:Day {{ day: {Mushroom.InoculatedOn!.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {Mushroom.InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {Mushroom.InoculatedOn.Value.Year} }})
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
        var input = Mushroom.UpdateFinishedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE
                                    x.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (x)-[r:FINISHED_ON]->(d)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (d:Day {{ day: {Mushroom.FinishedOn!.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {Mushroom.FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {Mushroom.FinishedOn.Value.Year} }})
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
        var input = Mushroom.UpdateVendorRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{Mushroom.EntityType})
                                WHERE
                                    x.Id = '{Mushroom.Id}'
                                OPTIONAL MATCH
                                    (x)-[r:PURCHASED_FROM]->(v)
                                DELETE 
                                    r
                                WITH
                                    x
                                MATCH
                                    (v:Vendor  {{ Name: '{Mushroom.Vendor}' }})
                                CREATE
                                    (x)-[r:PURCHASED_FROM]->(v)
                                RETURN 
                                    r
                            ";

        expected = expected.Replace(" ", "");

        input.Should().Be(expected);
    }
}