using PoliceMP.Shared.Services.Interfaces;
using System;

namespace PoliceMP.Shared.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}