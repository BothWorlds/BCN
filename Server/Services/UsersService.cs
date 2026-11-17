using System.Collections.Generic;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;

namespace BCN.Services
{
    public class ServerUsersService : IUsersService
    {
        private readonly IUserRepository _usersRepository;

        public ServerUsersService(IUserRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public List<User> GetUsers()
        {
            return _usersRepository.GetUsers().ToList();
        }
    }
}