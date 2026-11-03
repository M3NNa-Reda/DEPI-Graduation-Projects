using EasyShop.MVC.Helpers.Extensions;

namespace EasyShop.Tests;

public class CurrencyExtensionsTests
{
    [Theory]
    [InlineData(0, "0.00 EGP")]
    [InlineData(70, "70.00 EGP")]
    [InlineData(1234.5, "1,234.50 EGP")]
    [InlineData(890.516, "890.52 EGP")]
    public void ToEgp_formats_with_two_decimals_and_the_currency_code(double amount, string expected)
    {
        Assert.Equal(expected, ((decimal)amount).ToEgp());
    }

    [Fact]
    public void ToEgp_shows_a_dash_when_there_is_no_amount()
    {
        decimal? amount = null;

        Assert.Equal("-", amount.ToEgp());
    }

    [Fact]
    public void ToEgp_formats_a_nullable_amount_that_has_a_value()
    {
        decimal? amount = 25m;

        Assert.Equal("25.00 EGP", amount.ToEgp());
    }
}
