using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.QuestionPed
{
    public class QuestionPedHandler : ActionHandler<QuestionPed>
    {
        private readonly IPedInfoService _pedInfoService;
        private readonly INotificationService _notificationService;
        private readonly ISpeechService _speech;

        public QuestionPedHandler(IPedInfoService pedInfoService,
            INotificationService notificationService,
            ISpeechService speech)
        {
            _pedInfoService = pedInfoService;
            _notificationService = notificationService;
            _speech = speech;
        }

        protected override async Task<bool> Handle(QuestionPed action)
        {
            var pedInfo = await _pedInfoService.GetByNetworkId(action.Target.NetworkId);
            if (pedInfo == null)
            {
                _notificationService.Error("Question", "Could not retrieve information for the ped.");
                return false;
            }

            bool positiveAnswer = true;

            switch (action.Question.Attribute)
            {
                case "Search":
                    positiveAnswer = !pedInfo.HasIllegalItems;
                    break;

                case "Drunk":
                    positiveAnswer = pedInfo.AlcoholLevel < 0.5f;
                    break;

                case "Drugs":
                    positiveAnswer = !pedInfo.IsOnAnyDrugs;
                    break;

                case "Attitude":
                    positiveAnswer = pedInfo.Attitude < 50;
                    break;
            }

            string answer = positiveAnswer
                ? action.Question.GetRandomPositiveAnswer()
                : action.Question.GetRandomNegativeAnswer();

            Game.PlayerPed.Task.PlayAnimation("special_ped@baygor@michael_2@michael_2c", "hey_how_you_doing2_2");

            _speech.Say(Game.PlayerPed, action.Question.Text, 3000);

            await Delay(TimeSpan.FromSeconds(2));

            if (!action.Target.IsInVehicle())
            {
                action.Target.Task.TurnTo(Game.PlayerPed);

                await Delay(1000);

                action.Target.Task.PlayAnimation("special_ped@baygor@michael_2@michael_2c",
                    "hey_how_you_doing2_2");
            }

            _speech.Say(action.Target, answer);

            if (!action.Target.IsInVehicle()) await action.Target.StandStillFacingPlayer();

            return true;
        }
    }
}