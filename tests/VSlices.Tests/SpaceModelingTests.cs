using VSlices.Space.Modeling;

namespace VSlices.Tests;

public class SpaceModelingTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void LocationName_rejects_empty_values(string value)
    {
        var result = Usage.CreateName(value);

        Assert.True(result.IsFail);
    }

    [Fact]
    public void LocationName_rejects_values_longer_than_100_characters()
    {
        var result = Usage.CreateName(new string('a', 101));

        Assert.True(result.IsFail);
    }

    [Fact]
    public void LocationName_accepts_100_characters()
    {
        var result = Usage.CreateName(new string('a', 100));

        Assert.True(result.IsSucc);
    }

    [Fact]
    public void LocationName_trims_the_value_before_materialization()
    {
        var name = Success(Usage.CreateName("  Warehouse  "));

        Assert.Equal("Warehouse", name.Value);
    }

    [Fact]
    public void Location_is_created_from_an_already_established_name()
    {
        var name = Success(Usage.CreateName("Warehouse"));
        var location = Success(Usage.Create(name));

        Assert.Same(name, location.CurrentState.Name);
        Assert.Equal(10, location.CurrentState.X);
        Assert.Equal(20, location.CurrentState.Y);
    }

    [Fact]
    public void Update_materializes_a_new_location_without_mutating_the_source()
    {
        var location = Success(Usage.Create());
        var updated = Success(Usage.Move(location));

        Assert.NotSame(location, updated);

        Assert.Equal(10, location.CurrentState.X);
        Assert.Equal(20, location.CurrentState.Y);

        Assert.Equal(11, updated.CurrentState.X);
        Assert.Equal(21, updated.CurrentState.Y);
    }

    [Fact]
    public void Update_preserves_creation_fixed_name()
    {
        var location = Success(Usage.Create());
        var originalName = location.CurrentState.Name;

        var updated = Success(Usage.Move(location));

        Assert.Same(originalName, updated.CurrentState.Name);
        Assert.Equal(originalName, updated.CurrentState.Name);
    }

    [Fact]
    public void Sequential_updates_each_materialize_a_distinct_location()
    {
        var first = Success(Usage.Create());
        var second = Success(Usage.Move(first));
        var third = Success(Usage.Move(second));

        Assert.NotSame(first, second);
        Assert.NotSame(second, third);
        Assert.NotSame(first, third);

        Assert.Equal((10, 20), (first.CurrentState.X, first.CurrentState.Y));
        Assert.Equal((11, 21), (second.CurrentState.X, second.CurrentState.Y));
        Assert.Equal((12, 22), (third.CurrentState.X, third.CurrentState.Y));
        Assert.Same(first.CurrentState.Name, third.CurrentState.Name);
    }

    private static T Success<T>(Fin<T> result) =>
        result.Match(
            Succ: value => value,
            Fail: error => throw new Xunit.Sdk.XunitException(
                $"Expected success, but got: {error}"));
}
