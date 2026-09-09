using PoliceMP.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<List<Question>> GetAllAsync(string questionList = null);
    }
}