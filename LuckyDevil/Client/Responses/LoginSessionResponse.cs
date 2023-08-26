using System.Text.Json.Serialization;

namespace LuckyDevil.Client.Responses;

public record LoginSessionResponse(string Result, string Session, LoginSessionResponseData Data) : JsonResponse(Session);

public record LoginSessionResponseData(
    bool Admin,
    bool ChangeProfile,
    bool Ptz,
    bool Audio,
    bool Clips,
    int SessionLimit,
    int StreamLimit,
    int DayLimit,
    int DayUsed,
    bool Dio,
    string Version,
    string License,
    string Support,
    string User,
    decimal Latitude,
    decimal Longitude,
    string Tzone,
    List<string> Streams,
    List<string> Sounds,
    List<string> Www_Sounds,
    List<string> Profiles,
    List<string> Schedules,
    bool News)
{
    [JsonPropertyName("system name")]
    public string System_Name { get; init; }
};
public static class LoginResult
{
    public const string Success = "success";
    public const string Failure = "fail";
}