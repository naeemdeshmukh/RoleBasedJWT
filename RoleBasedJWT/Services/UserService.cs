using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoleBasedJWT.Entities;
using RoleBasedJWT.Helpers;

namespace RoleBasedJWT.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private List<User> _users = new List<User>
        {
            new User {Id=1,FirstName="Admin",LastName="User", Username="admin",Password="admin", Role=Role.Admin },
            new User {Id=2,FirstName="Normal",LastName="User", Username="user",Password="user", Role=Role.User },
            new User {Id=3,FirstName="Naeem",LastName="Deshmukhh", Username="emp",Password="emp", Role=Role.Emp },
            new User {Id=4,FirstName="Gat",LastName="Sat", Username="mngr",Password="mngr", Role=Role.Manager }
        };

        private readonly AppSettings _appSettings;

        
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            if (user == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            user.Password = null;

            return user;
        }


        public IEnumerable<User> GetAll()
        {
            //return users without psswd
            return _users.Select(x => {
                x.Password = null;
                return x;
            });
        }


        public User GetById(int id)
        {

            

            var user = _users.FirstOrDefault(x => x.Id == id);

            // return users without psswd

            if (user != null)
                user.Password = null;

            return user;

        }


    }
}
