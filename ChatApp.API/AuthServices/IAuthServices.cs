using ChatApp.API.Dtos;
using ChatApp.API.Helper;

namespace ChatApp.API.AuthServices
{
    public interface IAuthServices
    {
        Task<AuthModel> Register(RegisterDto registerDTO);
        Task<AuthModel> Login(LoginDTO loginDTO);
    }
}
