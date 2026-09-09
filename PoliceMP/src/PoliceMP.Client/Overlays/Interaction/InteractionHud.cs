using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Overlays;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;

namespace PoliceMP.Client.Overlays.Interaction
{
    public sealed class InteractionHud : Overlay, IInteractionHud
    {
        public Entity Entity { get; private set; }
        private string _name = string.Empty;
        private bool _knowsPedName;
        private IList<PlayerAction> _actions = new List<PlayerAction>(0);

        public InteractionHud(string id, INuiManager nuiManager, ILogger<Overlay> logger) 
            : base(id, nuiManager, logger)
        {
            Disable();

            On("GetInteractionContext", GetInteractionTarget);
        }

        void Sync()
        {
            Emit("SetInteractionContext", new InteractionContextMessage(_name, _actions));
        }

        private InteractionContextMessage GetInteractionTarget()
        {
            return new InteractionContextMessage(_name, _actions);
        }


        public void SetInteractionEntity(Entity entity)
        {
            if (Entity != entity)
            {
                Entity = entity;

                if (entity is null)
                {
                    _name = string.Empty;
                }
                else if (entity is Vehicle vehEntity)
                {
                    _name = $"{vehEntity.LocalizedName} [{vehEntity.GetPlateText()}]";
                }
                else if (entity is Ped pedEntity)
                {
                    if (!_knowsPedName && pedEntity.GetIsNameKnown())
                    {
                        _knowsPedName = true;
                    }

                    _name = _knowsPedName
                        ? pedEntity.State.Get<string>(PedStates.FullName)
                        : "Unknown Person";

                    if (pedEntity.IsPlayer)
                    {
                        var targetId = API.NetworkGetPlayerIndexFromPed(pedEntity.Handle);
                        _name = API.GetPlayerName(targetId);
                    }
                }
                else
                {
                    _name = entity.GetType().Name;
                }

                Sync();
            }
            
        }

        public void SetInteractionActions(IList<PlayerAction> actions)
        {
            if (actions is null)
            {
                throw new ArgumentException("actions cannot be null");
            }

            if (_actions is null || !_actions.SequenceEqual(actions))
            {
                _actions = actions;
                Sync();
            }
        }

        public void ClearInteractionContext()
        {
            if (Entity is not null || _actions?.Any() == true)
            {
                Emit("ClearInteractionContext");
                _name = string.Empty;
                _actions = new List<PlayerAction>(0);
                _knowsPedName = false;
                Entity = null;
            }
        }
    }
}
