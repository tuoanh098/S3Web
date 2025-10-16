using System;
using System.Text.Json.Serialization;

namespace Student.Application.Dto
{
    public sealed class StudentUpdateDto
    {
        [JsonPropertyName("first_name")] public string? FirstName { get; init; }
        [JsonPropertyName("last_name")] public string? LastName { get; init; }
        [JsonPropertyName("birthday")] public DateTime? Birthday { get; init; }
        [JsonPropertyName("gender")] public string? Gender { get; init; }
        [JsonPropertyName("nation")] public string? Nation { get; init; }
        [JsonPropertyName("mobile")] public string? Mobile { get; init; }
        [JsonPropertyName("parent")] public string? Parent { get; init; }
        [JsonPropertyName("bio")] public string? Bio { get; init; }
    }
}
