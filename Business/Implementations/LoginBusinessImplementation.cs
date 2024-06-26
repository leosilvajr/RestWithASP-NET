﻿using RestWithASPNET.Configurations;
using RestWithASPNET.Data.VO;
using RestWithASPNET.Repository;
using RestWithASPNET.Services;
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

            //Criando o AcessToken e o RefreshToken
            var accessToken = _tokenService.GenerateAccessToken(claims); //Token para autenticar 
            var refreshToken = _tokenService.GenerateRefreshToken(); //Usar caso o accessToken estiver expirado

            //Vamos persistir isso na base.
            _userRepository.RefreshUserInfo(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTipe = DateTime.Now.AddDays(_configurtion.DaysToExpiry);

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
    }
}
