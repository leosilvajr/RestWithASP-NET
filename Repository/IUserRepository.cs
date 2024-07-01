using RestWithASPNET.Data.VO;
using RestWithASPNET.Model;

namespace RestWithASPNET.Repository
{
    public interface IUserRepository
    {
        User RefreshUserInfo(User user);
        User ValidateCredentials(UserVO user);

    }
}
