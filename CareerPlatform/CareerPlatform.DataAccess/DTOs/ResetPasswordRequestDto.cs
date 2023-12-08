namespace CareerPlatform.DataAccess.DTOs;

public class ResetPasswordRequestDto
{
    public string UserEmail { get; private set; }
    public string Token { get; private set; }
    public string? Password { get; private set; }

    public ResetPasswordRequestDto(string email, string token, string password)
    {
        UserEmail = email;
        Token = token;
        Password = password;
    }

    public ResetPasswordRequestDto()
    {

    }
}
