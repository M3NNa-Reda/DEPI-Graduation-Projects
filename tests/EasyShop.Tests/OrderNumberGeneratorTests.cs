using EasyShop.Application.Services;

namespace EasyShop.Tests;

public class OrderNumberGeneratorTests
{
    [Fact]
    public void Numbers_have_the_expected_shape()
    {
        var number = OrderNumberGenerator.Next();

        Assert.Matches("^ES-[2-9A-HJKMNP-Z]{10}$", number);
    }

    [Fact]
    public void Numbers_are_not_repeated()
    {
        var numbers = Enumerable.Range(0, 1000).Select(_ => OrderNumberGenerator.Next()).ToHashSet();

        Assert.Equal(1000, numbers.Count);
    }
}
