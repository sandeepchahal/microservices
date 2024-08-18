using UserService.Models;

namespace UserService.Implementation;

public interface IUserManageService
{
    User? GetUser(int userId);
    string? Authenticate(string username, string password);
}