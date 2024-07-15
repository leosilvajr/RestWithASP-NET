namespace RestWithASPNET.Data.VO
{
    public class TokenVO
    {
        public TokenVO(bool authenticated, string created, string expiration, string accessToken, string refreshToken)
        {
            Authenticated = authenticated;
            Created = created;
            Expiration = expiration;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public bool Authenticated { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

/*
 Teste de Variavel no POSTMAN
    if (responseCode.code >= 200 && responseCode.code <= 299) {
    var jsonData = JSON.parse(responseBody);

    postman.setEnvironmentVariable('accessToken', jsonData.accessToken);
    postman.setEnvironmentVariable('refreshToken', jsonData.refreshToken);
}
 */