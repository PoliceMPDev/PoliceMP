using System.Collections.Generic;
using System.Threading.Tasks;
using PoliceMP.Shared.Models;

namespace PoliceMP.Core.Server.Interfaces.Services
{
    public interface IQuestionService
    {
        List<Question> GetAll();
        List<Question> Get(string list);

    }

}