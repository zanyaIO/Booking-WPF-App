﻿using SIMS_HCI_Project.Domain.Models;
using SIMS_HCI_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMS_HCI_Project.Applications.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService() 
        { 
            _userRepository= Injector.Injector.CreateInstance<IUserRepository>();
        }

        public List<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public User LogIn(string username, string password)
        {
            return _userRepository.LogIn(username, password);
        }
    }
}
