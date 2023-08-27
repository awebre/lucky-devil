namespace LuckyDevil.Client.Requests;

public record AttemptLoginRequest(string Username, string Password, string Session);