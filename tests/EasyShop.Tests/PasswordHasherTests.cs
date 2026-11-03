using EasyShop.Infrastructure.Security;

namespace EasyShop.Tests;

public class PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_never_contains_the_password()
    {
        var hash = _hasher.Hash("Secret#123");

        Assert.DoesNotContain("Secret#123", hash);
    }

    [Fact]
    public void Hash_stores_iterations_salt_and_key()
    {
        var parts = _hasher.Hash("Secret#123").Split('.');

        Assert.Equal(3, parts.Length);
        Assert.Equal("600000", parts[0]);
    }

    [Fact]
    public void The_same_password_gets_a_different_hash_every_time()
    {
        Assert.NotEqual(_hasher.Hash("Secret#123"), _hasher.Hash("Secret#123"));
    }

    [Fact]
    public void Verify_accepts_the_right_password()
    {
        var hash = _hasher.Hash("Secret#123");

        Assert.True(_hasher.Verify("Secret#123", hash));
    }

    [Fact]
    public void Verify_rejects_a_wrong_password()
    {
        var hash = _hasher.Hash("Secret#123");

        Assert.False(_hasher.Verify("secret#123", hash));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("1.2")]
    [InlineData("abc.def.ghi")]
    public void Verify_rejects_a_missing_or_malformed_hash(string? stored)
    {
        Assert.False(_hasher.Verify("Secret#123", stored));
    }
}
