using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using EmmaLykkeMadBlog.Services.Interfaces;

namespace EmmaLykkeMadBlog.Services;

// Makes the anonymous visitor value: HMAC-SHA256 over address and browser, keyed with a random salt.
// The salt lives only in memory, is replaced every UTC day and is never saved or logged.
// Without the salt the value cannot be reversed, and after the day is over nobody can recreate it.
public class VisitorIdService(TimeProvider clock) : IVisitorIdService
{
    private readonly object gate = new();
    private DateOnly saltDay;
    private byte[] salt = [];

    public string Create(string address, string userAgent)
    {
        var hash = HMACSHA256.HashData(CurrentSalt(), Encoding.UTF8.GetBytes($"{address}|{userAgent}"));
        return Base64Url.EncodeToString(hash.AsSpan(0, 16)); // 22 URL-safe characters
    }

    private byte[] CurrentSalt()
    {
        var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);

        lock (gate)
        {
            if (today != saltDay)
            {
                salt = RandomNumberGenerator.GetBytes(32);
                saltDay = today;
            }

            return salt;
        }
    }
}