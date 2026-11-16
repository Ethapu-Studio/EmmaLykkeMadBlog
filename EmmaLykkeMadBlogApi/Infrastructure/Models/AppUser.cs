using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EmmaLykkeMadBlog.Infrastructure.Models;

public class AppUser : IdentityUser<Guid>
{
    [Required, MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    // Set only when the person actively ticks the newsletter box.
    public DateTimeOffset? NewsletterOptInAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}