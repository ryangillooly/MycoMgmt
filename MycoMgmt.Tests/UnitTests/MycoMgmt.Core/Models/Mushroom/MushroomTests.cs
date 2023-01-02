using Xunit;
using FluentAssertions;


namespace MycoMgmt.Tests.UnitTests.MycoMgmt.Core.Models.Mushroom;

public class MushroomTests
{
    [Xunit.Theory]
    [InlineData(null,  null,  "InProgress")]
    [InlineData(false, null,  "InProgress")]
    [InlineData(true,  null,  "Failed")]
    [InlineData(true,  false, "Failed")]
    [InlineData(true,  true,  "Successful")]
    private void IsSuccessfulShouldReturnCorrectStatus(bool? finished, bool? successful, string expected)
    {
        var mushroom = new Domain.Models.Mushrooms.Mushroom
        {
            Finished   = finished,
            Successful = successful
        };

        var status = mushroom.IsSuccessful();
        
        status.Should().Be(expected);
    }
}