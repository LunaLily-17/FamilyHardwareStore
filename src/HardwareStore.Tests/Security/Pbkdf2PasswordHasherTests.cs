using HardwareStore.Infrastructure.Security;

namespace HardwareStore.Tests.Security;

public sealed class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void HashAndVerify_WithMatchingPassword_ReturnsTrue()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.Hash("Admin@123");

        var isValid = hasher.Verify(hash, "Admin@123");

        Assert.True(isValid);
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        var hasher = new Pbkdf2PasswordHasher();
        var hash = hasher.Hash("Admin@123");

        var isValid = hasher.Verify(hash, "WrongPassword");

        Assert.False(isValid);
    }
}
