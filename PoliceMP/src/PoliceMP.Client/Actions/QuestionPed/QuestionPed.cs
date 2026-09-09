using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Actions.QuestionPed
{
    public class QuestionPed : IAction
    {
        public Question Question { get; set; }
        public Ped Target { get; set; }

        public QuestionPed(Question question, Ped target)
        {
            Question = question;
            Target = target;
        }
    }
}