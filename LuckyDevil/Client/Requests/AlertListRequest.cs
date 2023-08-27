namespace LuckyDevil.Client.Requests;

public record AlertListRequest(
    string Session,
    string Camera,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    bool Tiles,
    bool Delete = false,
    bool Reset = false
);