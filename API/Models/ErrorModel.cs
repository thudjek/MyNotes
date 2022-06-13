using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Models;

public class ErrorModel
{
    public string Message { get; set; } = "Something went wrong, please try again.";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[] Errors { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
