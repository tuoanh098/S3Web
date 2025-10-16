using System.Text.Json.Serialization;

public sealed class StudentProfileDto
{
    [JsonPropertyName("user_id")] public string UserId { get; init; } = null!;
    [JsonPropertyName("first_name")] public string? FirstName { get; init; }
    [JsonPropertyName("last_name")] public string? LastName { get; init; }
    [JsonPropertyName("birthday")] public DateTime? Birthday { get; init; }
    [JsonPropertyName("gender")] public string? Gender { get; init; }
    [JsonPropertyName("nation")] public string? Nation { get; init; }
    [JsonPropertyName("email")] public string? Email { get; init; }
    [JsonPropertyName("mobile")] public string? Mobile { get; init; }
    [JsonPropertyName("parent")] public string? Parent { get; init; }
    [JsonPropertyName("bio")] public string? Bio { get; init; }
    [JsonPropertyName("joined_at")] public DateTime JoinedAt { get; init; }
}
