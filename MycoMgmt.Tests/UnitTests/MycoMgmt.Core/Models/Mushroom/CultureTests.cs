using Xunit;
using FluentAssertions;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Tests.UnitTests;

public class CultureTests
{
    [Fact]
    private void CreateNodeShouldReturnValidCreationQuery()
    {
        var culture = new Culture
        {
            Name = "CP-01",
            Type = "Agar",
            Finished = true,
            Successful = true
        };
        var createNodeQuery = culture.CreateNode();

        var status = culture.IsSuccessful();
        var expected = $@"CREATE 
                                (
                                    x
                                        :`{culture.EntityType}`
                                        :`{culture.Type}`
                                        :`{status}`
                                    {{ 
                                        Name:       '{culture.Name}',
                                        Id:       '{culture.Id}',
                                        EntityType: '{culture.EntityType}',
                                        Type:       '{culture.Type}',
                                        Status:     '{status}'

                                     }}
                                )
                            RETURN x";

        var created = createNodeQuery.Replace(" ", "");
        expected = expected.Replace(" ", "");
        
        created.Should().Be(expected);
    }
}