using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Constants.States;

namespace PoliceMP.Core.Client.Scripts
{
    public abstract class PedBehaviorBase : IDisposable
    {
        private readonly ITickManager _ticks;
        private EntityStateBagProxy<Type> _behaviorState;
        public double TookControlTime { get; protected set; }

        private Ped _thePed;
        protected abstract void CheckBrainScript();
        public abstract Type GetBehaviorType();

        private int DebugTextIndex { get; set; }
        public Action OnDispose { get; set; }

        public Ped ThePed
        {
            get => _thePed;
            private set
            {
                if (_thePed != null)
                    throw new Exception("BIG BOABY TRYING TO OVERWRITE!");
                
                _thePed = value;
            }
        }

        protected PedBehaviorBase(ITickManager ticks)
        {
            _ticks = ticks;
        }

        public virtual void SetPed(Ped ped, bool resetBlackboard)
        {
            if (ThePed != null)
                throw new ArgumentException("Cannot set ped after it has been set. Don't do this.");

            ThePed = ped;
            Debug.WriteLine($"PED ID {ThePed.NetworkId}");
        }


        public void StartInternal()
        {
            _behaviorState = new EntityStateBagProxy<Type>(ThePed, PedStates.AttachedBrain);

            CheckBrainScript();

            Start();

            _ticks.On(ThinkInternal);
            _ticks.On(OnDrawDebugInternal);
        }

        public virtual async Task ThinkInternal()
        {
            CheckBrainScript();

            if (ThePed is null || !ThePed.Exists())
            {
                Debug.WriteLine("DISPOSE!");
                StopInternal();
                Dispose();
                return;
            }

            await CheckMigrate();

            if (ThePed.HasNetworkControl())
            {
                API.NetworkSetNetworkIdDynamic(ThePed.NetworkId, false);
                await Think();
            }
            else
            {
                await ThinkRemote();
            }
        }

        protected abstract Task CheckMigrate();

        public void StopInternal()
        {
            if (ThePed.Exists())
            {
                ThePed.MarkAsNoLongerNeededKeepHandle();
            }
            _ticks.Off(ThinkInternal);
            Stop();
        }

        public void Dispose()
        {
            _ticks.Off(ThinkInternal);
            OnDispose?.Invoke();
        }

        private Task OnDrawDebugInternal()
        {
            if (!DebugUtils.DebugEnabled)
            {
                return Task.FromResult(0);
            }
            
            if (ThePed.IsOnScreen)
            {
                DebugTextIndex = 0;
                OnDrawDebug();
            }

            return Task.FromResult(0);
        }

        protected virtual void OnDrawDebug()
        {
        }

        protected void DrawDebugText(string text = null, System.Drawing.Color color = default)
        {
            if (text == null)
            {
                DebugTextIndex++;
                return;
            }

            if (color == default)
                color = System.Drawing.Color.FromArgb(255, 255, 255, 255);

            var startPos = Screen.WorldToScreen(ThePed.Position);
            startPos.Y += 7f * DebugTextIndex;
            var t = new Text(text, startPos, 0.2f, color);
            t.Draw();

            DebugTextIndex++;
        }

        /// <summary>
        /// Called when the behavior is first created after the Ped is set. Use this to initialize the blackboard!
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Called when the behavior is first created and after a migration. Use this to initialize callbacks etc.!
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        /// Called when the behavior is being destroyed.
        /// </summary>
        protected virtual void Stop()
        {
        }

        /// <summary>
        /// Called every tick.
        /// </summary>
        protected abstract Task Think();

        protected virtual Task ThinkRemote() => Task.FromResult(0);

        public abstract void SetMigrateInformation();
    }
}