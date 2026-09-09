using System;
using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PoliceMP.Core.Server.Interfaces.Services;

namespace PoliceMP.Server.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ILegacyServerCommunicationsManager comms,
            IQuestionService questionService,
            ILogger<QuestionController> logger)
        {
            _comms = comms;
            _questionService = questionService;
            _logger = logger;

            _comms.OnRequest<string, List<Question>>(ServerEvents.GetAllQuestions, OnGetAllQuestions);
        }

        private async Task<List<Question>> OnGetAllQuestions(Player player, string questionList = null)
        {
            //_logger.Debug($"Player {player.Name} requested all questions{(questionList == null ? "" : $" from {questionList}")}.");

            var questions = await Task.Run(() =>
            {
                return questionList == null
                    ? _questionService.GetAll()
                    : _questionService.Get(questionList);
            });

            //_logger.Debug($"Returning {questions?.Count} to player {player.Name}");
            return questions;
        }
    }
}