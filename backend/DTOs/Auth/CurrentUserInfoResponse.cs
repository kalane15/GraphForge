namespace GraphForge.Api.DTOs.Auth;

public record CurrentUserInfoResponse(string Login, DateTimeOffset CreatedAt);
