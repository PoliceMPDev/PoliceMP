using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Core.Client;

namespace PoliceMP.Client.Overlays.Interaction
{
    public class InteractionContextMessage
    {
        public string Name { get; set; }
        public IList<InteractionActionMessage> Actions { get; set; }
        
        public InteractionContextMessage(string name, IList<PlayerAction> actions)
        {
            Name = name;

            Actions = actions?.Select(a =>
                new InteractionActionMessage(a.Name, a.Text, a.GamepadControl, a.MouseAndKeyboardControl, a.Flags.HasFlag(PlayerActionFlags.HoldControl))).ToList()
                ?? new List<InteractionActionMessage>();
        }

        public override bool Equals(object obj)
        {
            if (obj is not InteractionContextMessage other)
            {
                return false;
            }

            if (Name != other.Name)
            {
                return false;
            }

            if (Actions.Count != other.Actions.Count)
            {
                return false;
            }

            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].Id != other.Actions[i].Id)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
