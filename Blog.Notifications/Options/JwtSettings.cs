namespace Blog.Notifications.Options;

// Deliberately duplicated from Blog.Application/Options/JwtSettings.cs —
// same reasoning as the message contract duplication in milestone 1: two
// genuinely separate repos/solutions, no project reference between them.
// Values must match Blog.API's exactly, since this service validates the
// same tokens Blog.API issues. A real system would more likely use
// asymmetric (RS256) signing with a shared public key or a JWKS endpoint
// instead, so services validate tokens without holding the signing
// secret at all — sharing one symmetric secret is a simplification, only
// reasonable here because of the already-accepted low-stakes Dev-only key
// decision (learning-notes/notes/36-secrets-management.md).
public class JwtSettings
{
    public string? SigningKey { get; set; }
    public string? Issuer { get; set; }
    public string[]? Audiences { get; set; }
}
