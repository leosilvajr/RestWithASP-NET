using RestWithASPNET.Configurations;
using RestWithASPNET.Data.VO;
using RestWithASPNET.Repository;
using RestWithASPNET.Services;
using RestWithASPNETUdemy.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RestWithASPNET.Business.Implementations
{
    public class LoginBusinessImplementation : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy:MM:dd:HH:mm:ss";
        private TokenConfiguration _configurtion;
        private IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginBusinessImplementation(TokenConfiguration configurtion, IUserRepository userRepository, ITokenService tokenService)
        {
            _configurtion = configurtion;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public TokenVO ValidateCredentials(UserVO userCredentials)
        {
            //Validar o usuario credenciais no banco
            var user = _userRepository.ValidateCredentials(userCredentials);
            if (user == null) return null;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            //Criando o AccessToken e o RefreshToken
            var accessToken = _tokenService.GenerateAccessToken(claims); //Token para autenticar 
            var refreshToken = _tokenService.GenerateRefreshToken(); //Usar caso o accessToken estiver expirado


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configurtion.DaysToExpiry);

            DateTime createDate = DateTime.Now; //Data de criação do Token
            DateTime expirationDate = createDate.AddMinutes(_configurtion.Minutes);

            //Vamos persistir isso na base.
            _userRepository.RefreshUserInfo(user);

            return new TokenVO(
                true, 
                createDate.ToString(DATE_FORMAT), 
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public TokenVO ValidateCredentials(TokenVO token)
        {
            var accessToken = token.AccessToken;  //Token para autenticar 
            var refreshToken = token.RefreshToken; //Usar caso o accessToken estiver expirado

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            //Recuperando o Usuario
            var username = principal.Identity.Name;
            var user = _userRepository.ValidateCredentials(username);
            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now)
                return null;


            accessToken = _tokenService.GenerateAccessToken(principal.Claims);
            refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            //Vamos persistir isso na base.
            _userRepository.RefreshUserInfo(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configurtion.DaysToExpiry);

            DateTime createDate = DateTime.Now; //Data de criação do Token
            DateTime expirationDate = createDate.AddMinutes(_configurtion.Minutes);


            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public bool RevokeToken(string userName)
        {
            return _userRepository.RevokeToken(userName);
        }
    }
}
