using System.Collections.Generic;
using Oqtane.Models;

namespace BCN.Services
{
    public interface IUsersService
    {
        public List<User> GetUsers();
    }
}