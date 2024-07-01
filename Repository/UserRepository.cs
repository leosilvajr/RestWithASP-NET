using RestWithASPNET.Data.VO;
using RestWithASPNET.Model;
using RestWithASPNETUdemy.Model.Context;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace RestWithASPNET.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySQLContext _context;

        public UserRepository(MySQLContext context)
        {
            _context = context;
        }


        public User ValidateCredentials(UserVO user)
        {
            //Validar as credenciais do usuario.
            //Vamos encripitar a senha para depois comparar
            var pass = ComputeHash(user.Password, SHA256.Create());
            return _context.Users.FirstOrDefault(u =>
                                (u.UserName == user.UserName) &&
                                (u.Password == user.Password));

        }

        public User RefreshUserInfo(User user)
        {
            //Se a request não encontrar usuario nenhum, retorna null
            if (!_context.Users.Any(u => u.Id.Equals(user.Id))) return null;

            var result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));
            if (result != null) //Se o resultado for diferente de null, vamos atualizar as informaçoes do usuario.
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return result;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return result;
        }


        //Se der ruim, muda pra object
        private string ComputeHash(string input, HashAlgorithm hashAlgorithm)
        {
            //Metodo responsavel por encripitaar a senha.
            Byte[] hashedBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            foreach (var item in hashedBytes)
            {
                sBuilder.Append(item.ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
