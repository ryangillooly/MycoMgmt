using Xunit;
using FluentAssertions;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Tests.UnitTests.MycoMgmt.Core.Models.Mushroom;

public class ModelBaseTests
{
    [Fact]
    public void NewEntityShouldReturnEntityType()
    {
        var model = new ModelBase();

        model.EntityType.Should().Be(model.GetType().Name);
        model.EntityType.Should().Be(model.GetType().Name);
    }
}