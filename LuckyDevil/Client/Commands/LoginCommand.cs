namespace LuckyDevil.Client.Commands;
internal record LoginCommand(string? Session = null, string? Response = null) : JsonCommand("login");