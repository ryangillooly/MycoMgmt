using System.Runtime.InteropServices;
using AutoFixture;
using Xunit;
using FluentAssertions;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.Tests.UnitTests;

public class ModelBaseTests
{
    private Mushroom mushroom { get; set; }

    public ModelBaseTests()
    {
        mushroom = new Fixture().Create<Mushroom>();
    }


    [Fact]
    public void MultipleEntityCreationsShouldReturnMultipleEntityIds()
    {
        const int entityCount     = 5;
        const int expectedIdCount = 5;
        var mushroomName    = mushroom.Name;
        
        var resultList = new List<string>();
        
        for (var i = 1; i <= entityCount; i++)
        {
            mushroom.Id = Guid.NewGuid().ToString();
            mushroom.Name = mushroomName + "-" + i.ToString("D2");
            resultList.Add(mushroom.Id);
        }

        resultList.Distinct().Count().Should().Be(expectedIdCount);
    }
    
    
    [Fact]
    public void NewEntityShouldReturnEntityType()
    {
        var model = new ModelBase();

        model.EntityType.Should().Be(model.GetType().Name);
        model.EntityType.Should().Be(model.GetType().Name);
    }
    
    [Fact]
    private void CreateNodeShouldReturnQuery()
    {
        var input   = mushroom.CreateNode()?.Replace(" ", "");
        var expected = $@"
                                CREATE 
                                (
                                    x:{mushroom.EntityType} {{ 
                                        Name: '{mushroom.Name}',
                                        Id: '{mushroom.Id}'
                                        ,Notes: '{mushroom.Notes}',Type: '{mushroom.Type}'
                                    }}
                                )
                                RETURN x
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateNodeLabelsShouldReturnQuery()
    {
        var input   = mushroom.CreateNodeLabels()?.Replace(" ", "");
        var expected = $@"
                                MATCH (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}' }})
                                SET x:{string.Join(":", mushroom.Tags)}
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateQueryListShouldReturnQuery()
    {
        
    }
    
    [Fact]
    private void UpdateQueryListShouldReturnQuery()
    {

    }
    
    [Fact]
    private void CreateCreatedRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateCreatedRelationship()?.Replace(" ", "");
        var expected =  $@"
                                MATCH 
                                    (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}'      }}),
                                    (u:User  {{ Name: '{mushroom.CreatedBy}' }})
                                CREATE
                                    (u)-[r:CREATED]->(x)
                                RETURN r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void CreateCreatedOnRelationshipShouldReturnQuery()
    {
        var input   = mushroom.CreateCreatedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType} {{ Name: '{mushroom.Name}' }}), 
                                    (d:Day {{ day: {mushroom.CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.CreatedOn.Year} }})
                                CREATE
                                    (x)-[r:CREATED_ON]->(d)
                                RETURN r
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void SearchByNameQueryShouldReturnQuery()
    {
        var input   = mushroom.SearchByNameQuery()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}) 
                                WHERE 
                                    toUpper(x.Name) CONTAINS toUpper('{mushroom.Name}') 
                                RETURN 
                                    x 
                                ORDER BY 
                                    x.Name ASC
                                LIMIT 
                                    100
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void GetByNameQueryShouldReturnQuery()
    {
        var input   = mushroom.GetByNameQuery()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}) 
                                WHERE 
                                    toUpper(x.Name) = toUpper('{mushroom.Name}') 
                                RETURN 
                                    x
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void GetByIdQueryShouldReturnQuery()
    {
        var input   = mushroom.GetByIdQuery()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}) 
                                WHERE 
                                    x.Id = '{mushroom.Id}'
                                RETURN 
                                    x
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void GetAllQueryShouldReturnQuery()
    {
        var input   = mushroom.GetAllQuery().Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
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
                                    0
                                LIMIT
                                    20
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void UpdateNameShouldReturnQuery()
    {
        var input   = mushroom.UpdateName()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}) 
                                WHERE 
                                    x.Id = '{mushroom.Id}' 
                                SET 
                                    x.Name = '{mushroom.Name}' 
                                RETURN 
                                    x
                              "; 
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void UpdateModifiedOnRelationshipShouldReturnQuery()
    {
        DateTime.TryParse(mushroom.ModifiedOn.ToString(), out var parsedDateTime);
        
        var input   = mushroom.UpdateModifiedOnRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    x.Id = '{mushroom.Id}'
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
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void UpdateModifiedRelationshipShouldReturnQuery()
    {
        var input   = mushroom.UpdateModifiedRelationship()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType})
                                WHERE
                                    x.Id = '{mushroom.Id}'
                                OPTIONAL MATCH
                                    (u)-[r:MODIFIED]->(x)
                                DELETE
                                    r
                                WITH
                                    x
                                MATCH
                                    (u:User {{ Name: '{mushroom.ModifiedBy}'}} )
                                CREATE 
                                    (u)-[r:MODIFIED]->(x)
                                RETURN
                                    r  
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void UpdateNotesShouldReturnQuery()
    {
        var input   = mushroom.UpdateNotes()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}) 
                                WHERE 
                                    x.Id = '{mushroom.Id}' 
                                SET 
                                    x.Notes = '{mushroom.Notes}' 
                                RETURN s 
                              ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void UpdateTypeShouldReturnQuery()
    {
        var input   = mushroom.UpdateType()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}) 
                                WHERE 
                                    x.Id = '{mushroom.Id}' 
                                SET 
                                    x.Type = '{mushroom.Type}' 
                                RETURN s 
                              ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
    [Fact]
    private void DeleteShouldReturnQuery()
    {
        var input   = mushroom.Delete()?.Replace(" ", "");
        var expected = $@"
                                MATCH 
                                    (x:{mushroom.EntityType}) 
                                WHERE 
                                    x.Id = '{ mushroom.Id }' 
                                DETACH DELETE 
                                    x
                                RETURN 
                                    x
                            ";
        
        expected = expected.Replace(" ", "");
        
        input.Should().Be(expected);
    }
    
}