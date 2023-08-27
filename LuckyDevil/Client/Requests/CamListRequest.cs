namespace LuckyDevil.Client.Requests;

public record CamListRequest(string Session, CamListResetOptions? Reset = CamListResetOptions.None);

public enum CamListResetOptions
{
    None = 0,
    StatCount = 1,
    AlertCount = 2,
    Both = 3,
}