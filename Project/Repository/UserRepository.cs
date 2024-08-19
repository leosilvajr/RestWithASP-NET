using RestWithASPNET.Business.Implementations;
using RestWithASPNET.Data.VO;
using RestWithASPNET.Model;
using RestWithASPNETUdemy.Model.Context;
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
        public User ValidateCredentials(UserVO user)
        {
            var pass = ComputeHash(user.Password);
            var userFound = _context.Users.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == pass));
            var allUsers = _context.Users;

            return userFound;
        }

        //Validação para receber apenas o username
        public User ValidateCredentials(string username)
        {
            return _context.Users.SingleOrDefault(u => (u.UserName == username));
        }


        private string ComputeHash(string input)
        {
            using (var algorithm = SHA256.Create())
            {
                Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }


        public bool RevokeToken(string userName)
        {
            var user = _context.Users.SingleOrDefault(u => (u.UserName == userName));
            if (user is null) return false;

            user.RefreshToken = null;
            _context.SaveChanges();

            return true;
        }

        public User CreateUser(User newUser)
        {
            if (_context.Users.Any(u => u.UserName == newUser.UserName))
            {
                return null; // Usuário já existe
            }

            var user = new User
            {
                UserName = newUser.UserName,
                Password = ComputeHash(newUser.Password),
                FullName = newUser.FullName,
                RefreshToken = string.Empty,
                RefreshTokenExpiryTime = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

    }
}
