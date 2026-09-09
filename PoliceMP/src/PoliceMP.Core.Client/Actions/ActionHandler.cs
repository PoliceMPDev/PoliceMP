using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Actions
{
    public class ActionHandler<TAction> : IActionHandler where TAction : IAction
    {
        public virtual Task Initialise()
        {
            return Task.FromResult(0);
        }

        public Type GetActionType()
        {
            return typeof(TAction);
        }

        public Task<bool> Handle(IAction action)
        {
            if (action is TAction tAction)
            {
                return Handle(tAction);
            }

            throw new ActionException($"Given IAction ({action.GetType()}) was not a type of TAction ${GetActionType()}");
        }

        protected virtual Task<bool> Handle(TAction action)
        {
            return Task.FromResult(true);
        }

        protected async Task Delay(int ms)
        {
            await BaseScript.Delay(ms);
        }

        protected async Task Delay(TimeSpan delay)
        {
            await BaseScript.Delay((int)delay.TotalMilliseconds);
        }
    }
}
