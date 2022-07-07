using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedModels;
public class ErrorModel
{
    public ErrorModel()
    {
        Error = "Something went wrong, please try again.";
    }

    public ErrorModel(string error)
    {
        Error = error;
    }

    public string Error { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[] Errors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]> ErrorsGrouped { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}