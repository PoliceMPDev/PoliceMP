using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.FailToStop;

namespace PoliceMP.Client.Actions.TacklePed
{
    public class TacklePed : IAction
    {
        public Ped Tackler { get; }
        public Ped Target { get; }

        public TacklePed(Ped tackler, Ped target)
        {
            Tackler = tackler;
            Target = target;
        }
    }

    public class TacklePedHandler : ActionHandler<TacklePed>
    {
        private readonly ILogger<TacklePedHandler> _log;
        private readonly IScriptManager _scriptManager;
        private readonly IBehaviorService _behaviors;
        private const string AnimDict = "missmic2ig_11";
        private const string AnimOne = "mic_2_ig_11_intro_goon";
        private const string AnimTwo = "mic_2_ig_11_intro_p_one";
        private const string AnimCam = "mic_2_ig_11_intro_cam";

        public TacklePedHandler(ILogger<TacklePedHandler> log, IScriptManager scriptManager, IBehaviorService behaviors)
        {
            _log = log;
            _scriptManager = scriptManager;
            _behaviors = behaviors;
        }

        protected override async Task<bool> Handle(TacklePed action)
        {
            var tackler = action.Tackler;
            var target = action.Target;
            bool doTackler = true;
            bool doTarget = true;
            if (!target.Exists())
            {
                _log.Error($"Target {target.Handle} does not exist!");
                return false;
            }

            if (!tackler.IsHuman || !target.IsHuman)
            {
                _log.Error("Cannot tackle as either the tackler or target is not human!");
                return false;
            }

            if (target.IsAttached())
            {
                //_log.Debug("Could not tackle as the ped is attached to another entity");
                return false;
            }

            if (tackler.IsPlayer && target.IsPlayer)
            {
                if (tackler == Game.PlayerPed)
                {
                    doTarget = false;
                }
                else if (target == Game.PlayerPed)
                {
                    doTackler = false;
                }
                else
                {
                    return false;
                }
            }

            var direction = target.Position - tackler.Position;
            direction.Normalize();
            var rotation = PmpMath.DirectionToRotationFixed(direction, 0f);
            tackler.Rotation = rotation;
            var position = target.Position;
            position.Z = World.GetGroundHeight(position);

            if (doTackler)
            {
                tackler.Task.ClearAllImmediately();
                tackler.Task.PlayAnimation(AnimDict, AnimOne);
            }

            if (doTarget)
            {
                _behaviors.RemovePedBehaviors(target);
                target.Task.ClearAllImmediately();
                target.AttachTo(tackler, new Vector3(0.25f, 0.5f, 0.0f), new Vector3(0.5f, 0.5f, 180.0f));
                target.Task.PlayAnimation(AnimDict, AnimTwo);
            }

            await Script.Delay(3000);

            if (doTackler)
            {
                tackler.Task.PlayAnimation("get_up@standard", "front");
            }

            target.Detach();
            if (doTarget)
            {
                target.Ragdoll(10000);
            }

            await Script.Delay(2000);

            if (doTarget && target != Game.PlayerPed)
            {
                var releaseAction = new Func<Task>(async () =>
                {
                    await Script.Delay(8000);
                    if (!target.IsCuffed)
                    {
                        target.Task.ClearAll();
                        _behaviors.SetPedBehavior<FailToStopBehavior>(target);
                    }
                });

                var _ = releaseAction.Invoke();
            }

            return true;
        }
    }
}
