using Domain.GenericResult;
using Application.Home_Management.DTOs;
using Application.Home_Management.Interfaces;
using Domain.RepositotyInterfaces;
using Application.User_Dashboard.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Home_Management.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _work;
        private readonly ILogger<RoomService> _logger;

        public RoomService(
            IUnitOfWork work,
            ILogger<RoomService> logger)
        {
            _work = work;
            _logger = logger;
        }

    }
}
