﻿using SIMS_HCI_Project.Domain.Models;
using SIMS_HCI_Project.Domain.RepositoryInterfaces;
using SIMS_HCI_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMS_HCI_Project.Applications.Services
{
    public class ForumCommentService
    {
        private readonly IForumCommentRepository _forumCommentRepository;

        public ForumCommentService()
        {
            _forumCommentRepository = Injector.Injector.CreateInstance<IForumCommentRepository>();
        }

        public ForumComment GetById(int id)
        {
            return _forumCommentRepository.GetById(id);
        }

        public List<ForumComment> GetAll()
        {
            return _forumCommentRepository.GetAll();
        }
        /*
        public void Add(Notification notification)
        {
            _forumCommentRepository.Add(notification);
        }
        */
    }
}