using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Core.Shared.Scripts;
using PoliceMP.Shared.Constants.States;
using Color = System.Drawing.Color;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Core.Client.Scripts
{
    public abstract class PedBehavior<T> : PedBehaviorBase
        where T : PedBehaviorDefinition
    {

        private readonly string _dataPrefix;

        private string GetPrefixedName(string name) => $"{_dataPrefix}{name}";
        public Blackboard<T> Blackboard { get; private set; }

        protected PedBehavior(ITickManager ticks) : base(ticks)
        {
            _dataPrefix = $"bb_{typeof(T).Name}_";
        }

        public override void SetPed(Ped ped, bool resetBlackboard)
        {
            base.SetPed(ped, resetBlackboard);
            Blackboard = Blackboard<T>.Create(ped);

            if (resetBlackboard)
            {
                Blackboard.ResetData();
            }
        }

        public override Type GetBehaviorType()
        {
            return typeof(T);
        }

        protected override void CheckBrainScript()
        {
            if (ThePed.NetworkId == 0)
            {
                StopInternal();
                Dispose();
                return;
            }
            
            var currentBrains = ThePed.State.Get<Stack<Type>>(PedStates.AttachedBrain);
            if (null == currentBrains || currentBrains.Contains(typeof(T))) return;
            StopInternal();
            Dispose();
            // @todo Fish - To be fixed, keeps crashing the server
                //throw new BrainScriptException(
                //     $"Expected PedBehavior for ped {ThePed.NetworkId} to be \"{typeof(T).FullName}\" but it was \"{string.Join(", ", currentBrains.Select(t => t.FullName).ToArray())}\"");
        }

        protected sealed override async Task CheckMigrate()
        {
            var canMigrate = Blackboard.Get(bb => bb.CanMigrate);
            API.SetNetworkIdCanMigrate(ThePed.NetworkId, canMigrate);
            if (!canMigrate)
            {
                API.NetworkSetNetworkIdDynamic(ThePed.NetworkId, false);
                API.NetworkDisableProximityMigration(ThePed.NetworkId);
                return;
            }

            var currentHost = API.NetworkGetEntityOwner(ThePed.Handle);
            var hostServerId = Blackboard.Get(bb => bb.HostServerId);
            var nextAbleToMigrate = Blackboard.Get(bb => bb.NextAbleToMigrate);
            var serverTime = ServerInfo.GetServerTime();

            if (nextAbleToMigrate > serverTime.TotalSeconds)
            {
                return;
            }

            if (hostServerId != Game.Player.ServerId
                && nextAbleToMigrate < serverTime.TotalSeconds)
            {
                var hostPed = Entity.FromHandle(API.GetPlayerPed(currentHost));
                var hostDistance = World.GetDistance(ThePed.Position, hostPed?.Position ?? Vector3.Zero);

                if (hostDistance > Blackboard.Get(bb => bb.HostMigrateThreshold))
                {
                    var distance = World.GetDistance(ThePed.Position, Game.PlayerPed.Position);
                    if (distance < hostDistance)
                    {
                        if (ThePed.CurrentVehicle != null)
                        {
                            if (await ThePed.TryRequestNetworkEntityControl(false, 100)
                                && await ThePed.CurrentVehicle.TryRequestNetworkEntityControl(false, 100))
                            {
                                API.SetEntityAsMissionEntity(ThePed.Handle, true, true);
                                API.SetEntityAsMissionEntity(ThePed.CurrentVehicle.Handle, true, true);
                            }
                        }
                        else if (await ThePed.TryRequestNetworkEntityControl(false, 100))
                        {
                            API.SetEntityAsMissionEntity(ThePed.Handle, true, true);
                        }

                        SetMigrateInformation();

                        return;
                    }
                }
            }

            int handle = ThePed.Handle;
            API.SetEntityAsNoLongerNeeded(ref handle);
            if (ThePed.CurrentVehicle != null)
            {
                handle = ThePed.CurrentVehicle.Handle;
                API.SetEntityAsNoLongerNeeded(ref handle);
            }
        }

        public override void SetMigrateInformation()
        {
            var serverTime = ServerInfo.GetServerTime();
            Blackboard.Set(bb => bb.HostServerId, Game.Player.ServerId);
            Blackboard.Set(bb => bb.MigrateTime, serverTime.TotalSeconds);
            Blackboard.Set(bb => bb.NextAbleToMigrate, serverTime.TotalSeconds + Blackboard.Get(bb => bb.TimeToKeepControlAfterMigrationSeconds));
            TookControlTime = serverTime.TotalSeconds;
        }

        protected override void OnDrawDebug()
        {
            base.OnDrawDebug();
            var color = Color.FromArgb(255, 50, 200, 40);
            var badColor = Color.FromArgb(255, 255, 50, 50);
            var owner = new Player(API.NetworkGetEntityOwner(ThePed.Handle));
            var missionEntity = API.IsEntityAMissionEntity(ThePed.Handle);

            DrawDebugText($"[{typeof(T).Name}] ({Game.GameTime})", color);
            DrawDebugText($"[{string.Join(", ", ThePed.GetActiveTasks())}]", color);
            DrawDebugText($"Host: {owner.Name}", owner == Game.Player ? color : badColor);
            DrawDebugText($"MissionEntity: {missionEntity}", missionEntity ? color : badColor);
            DrawDebugText();
            DrawDebugText($"Handle: {ThePed.Handle}; NetworkId: {ThePed.NetworkId}");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var property in properties)
            {
                var value = Blackboard.Get(property);

                if (value.GetType() != typeof(string) && value is IEnumerable enumValue)
                {
                    StringBuilder sb = new StringBuilder($"{property.Name}: ");
                    foreach(var v in enumValue)
                    {
                        sb.Append(v);
                        sb.Append(", ");
                    }

                    sb.Remove(sb.Length - 1, 1);
                    DrawDebugText(sb.ToString());
                }
                else
                {
                    DrawDebugText($"{property.Name}: {value}");
                }
            }
        }
    }
}