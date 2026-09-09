using PoliceMP.Shared.Services.Interfaces;
using System;

namespace PoliceMP.Shared.Services
{
    public class MockDateTimeService : IDateTimeService
    {
        public DateTime Now { get; }

        public MockDateTimeService(DateTime now)
        {
            Now = now;
        }
    }
}