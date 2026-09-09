using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Client.Services
{
    public class QuestionService : IQuestionService
    {
        private List<Question> _questions;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(ILegacyClientCommunicationsManager comms, ILogger<QuestionService> logger)
        {
            _comms = comms;
            _logger = logger;
        }

        public async Task<List<Question>> GetAllAsync(string questionList = null)
        {
            _logger.Debug("Don't have questions yet. Requesting them from server...");
            _questions = await _comms.Request<List<Question>>(ServerEvents.GetAllQuestions, questionList);
            _logger.Debug($"Received {_questions?.Count} questions from server.");
            return _questions;
        }
    }
}