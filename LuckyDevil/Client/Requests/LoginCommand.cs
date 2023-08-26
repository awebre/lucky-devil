namespace LuckyDevil.Client.Requests;

internal record LoginCommand(string? Session = null, string? Response = null) : JsonCommand("login");