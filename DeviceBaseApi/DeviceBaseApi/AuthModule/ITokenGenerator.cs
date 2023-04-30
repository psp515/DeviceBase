namespace DeviceBaseApi.AuthModule;

public interface ITokenGenerator
{
    Task<(Guid, string)> GenerateAccessToken(User user);
    (Guid, string) GenerateRefreshToken();
}