using CitizenFX.Core;
using PoliceMP.Callouts.Server.Enums;
using PoliceMP.Callouts.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Server.Enums;

namespace PoliceMP.Callouts.Server.Models
{
    public class Callout : BaseScript
    {
        /// <summary>
        /// The unique callout identifier.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The callout grade.
        /// </summary>
        public int Grade { get; protected set; }

        /// <summary>
        /// The location of the callout.
        /// </summary>
        public Vector3 Location { get; set; }

        /// <summary>
        /// The title of the callout.
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// A short description about the callout.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// The rank required for this callout.
        /// </summary>
        public string Rank { get; protected set; }

        /// <summary>
        /// The players that are on the callout.
        /// </summary>
        public new readonly List<CalloutPlayer> Players;

        /// <summary>
        /// The callout entities, including peds and vehicles.
        /// </summary>
        protected readonly IDictionary<string, CalloutEntity> _entities;

        /// <summary>
        /// The time that this callout was created.
        /// </summary>
        public DateTime TimeCreated { get; private set; }

        /// <summary>
        /// The time that the first player arrived on scene.
        /// </summary>
        public DateTime TimeOfArrival { get; private set; }

        /// <summary>
        /// The callout status.
        /// </summary>
        public CalloutStatus Status { get; protected set; }

        /// <summary>
        /// The result of the callout.
        /// </summary>
        public CalloutResult Result { get; protected set; }

        /// <summary>
        /// Set to true when the first player has arrived to the callout.
        /// </summary>
        public bool FirstPlayerArrived { get; private set; }

        /// <summary>
        /// Set to true when the first player is within spawning range of the callout.
        /// This player is then set as the host.
        /// </summary>
        public bool FirstPlayerWithinRange { get; private set; }

        /// <summary>
        /// Whether a search area should be generated for the callout.
        /// </summary>
        public bool IsSearch { get; protected set; }

        /// <summary>
        /// Whether the location should be manually defined or dynamically via either
        /// GetNextPositionOnStreet() or GetNextPositionOnSidewalk()
        /// </summary>
        public LocationSetting LocationSetting { get; protected set; }

        /// <summary>
        /// Create a new callout.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="location">The location</param>
        /// <param name="title">The title</param>
        /// <param name="description">The description</param>
        /// <param name="caller">The caller</param>
        /// <param name="isTimed">Whether the callout is timed</param>
        /// <param name="timeLeft">How many seconds the timer should be set to</param>
        public Callout(int id)
        {
            Id = id;
            Players = new List<CalloutPlayer>();
            _entities = new Dictionary<string, CalloutEntity>();
            Status = CalloutStatus.Initial;
            Result = CalloutResult.NA;
            LocationSetting = LocationSetting.Manual;
            TimeCreated = DateTime.Now;
            FirstPlayerArrived = false;
            FirstPlayerWithinRange = false;
            IsSearch = false;
        }

        public virtual void Setup()
        {

        }

        public virtual void Update()
        {
            if (Status == CalloutStatus.Initial && !HasAnyPlayer())
            {
                var secondsSinceCreation = (DateTime.Now - TimeCreated).TotalSeconds;
                if (secondsSinceCreation > 300)
                {
                    End(CalloutResult.NoPlayerJoined);
                    return;
                }
            }
        }

        /// <summary>
        /// Starts the callout.
        /// </summary>
        /// <returns>Whether the callout started successfully</returns>
        public bool Start()
        {
            if (Status == CalloutStatus.Started) return false;

            SetBlips();

            Status = CalloutStatus.Started;

            Debug.WriteLine($"CALLOUTS: Callout {Id} started");

            OnStarted();

            return true;
        }

        /// <summary>
        /// Ends the callout
        /// </summary>
        /// <param name="result">The result of the callout. Why was it ended?</param>
        public void End(CalloutResult result)
        {
            Status = CalloutStatus.Ended;
            Result = result;

            switch (result)
            {
                case CalloutResult.NA:
                    Debug.WriteLine($"CALLOUTS: Callout {Id} ended due to NA (this result shouldn't be there if callout is ended)");
                    break;
                case CalloutResult.AllPlayersLeft:
                    Debug.WriteLine($"CALLOUTS: Callout {Id} ended due to all players leaving");
                    break;
                case CalloutResult.NoPlayerArrived:
                    Debug.WriteLine($"CALLOUTS: Callout {Id} ended due to no player arriving in time");
                    break;
                case CalloutResult.NoPlayerJoined:
                    Debug.WriteLine($"CALLOUTS: Callout {Id} ended due to no player joining in time");
                    break;
                case CalloutResult.EntitiesDespawned:
                    Debug.WriteLine($"CALLOUTS: Callout {Id} ended due to entities despawning");
                    break;
            }

            BaseScript.TriggerEvent(ServerEvents.CALLOUT_ENDED, Id);

            OnEnded(result);
        }

        public virtual void OnPlayerWithinRange(Player player)
        {
            var calloutPlayer = GetPlayer(player);
            if (calloutPlayer == null) return;

            calloutPlayer.WithinRange = true;

            if (!HasPlayer(player) || HasHost())
                return;

            calloutPlayer.IsHost = true;

            SpawnEntities();

            if (!FirstPlayerWithinRange)
            {
                FirstPlayerWithinRange = true;
                OnFirstPlayerWithinRange(player);
            }

            Debug.WriteLine("Player within range");
        }

        protected virtual void OnFirstPlayerWithinRange(Player player) { }

        /// <summary>
        /// Spawns the callout entities.
        /// </summary>
        private void SpawnEntities()
        {
            try
            {
                var hostPlayer = GetHost();
                if (GetHost() == null)
                {
                    Debug.WriteLine($"CALLOUTS: Could not spawn entities on callout {Id} due to host null");
                    return;
                }

                foreach (var entity in _entities.ToList())
                {
                    Debug.WriteLine($"Spawning vehicle for callout {Id} with host {hostPlayer.Player.Name}");
                    // First spawn all entities
                    if (entity.Value is CalloutVehicle vehicle)
                        ClientEventAPI.SpawnVehicle(hostPlayer.Player,
                            Id,
                            vehicle.Key,
                            vehicle.Plate,
                            (uint)vehicle.Hash,
                            vehicle.SpawnLocation,
                            (int)vehicle.Colour,
                            vehicle.BodyHealth,
                            vehicle.EngineHealth,
                            vehicle.PetrolTankHealth);
                    else if (entity.Value is CalloutPed ped)
                    {
                        Debug.WriteLine($"Spawning ped for callout {Id} with host {hostPlayer.Player.Name}");

                        ClientEventAPI.SpawnPed(hostPlayer.Player, Id, ped.Key, (uint)ped.Hash, ped.SpawnLocation, (uint)ped.WeaponHash);
                    }

                }

                Debug.WriteLine($"CALLOUTS: No entities spawned for callout {Id} due to not required");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        /// <summary>
        /// Set the network ID of an entity. After the entities spawn, the client
        /// will trigger an event on server to set this.
        /// </summary>
        /// <param name="entityKey">The entity key</param>
        /// <param name="networkId">The network ID</param>
        public void SetEntityNetworkId(string entityKey, int networkId, string plate = "")
        {
            var entity = GetEntity(entityKey);
            if (entity == null)
                return;

            entity.NetworkId = networkId;

            if (!string.IsNullOrEmpty(plate) && entity is CalloutVehicle vehicle)
                vehicle.Plate = plate;

            if (HaveAllNetworkIds())
            {
                Debug.WriteLine("I have all them now mate");
                SetBlips();
                SetPedsIntoVehicles();
                PersonCarGeneration();
                OnEntitiesReady();
            }
        }

        /// <summary>
        /// Called when the entities have spawned AND the network IDs
        /// have been retrieved from the client.
        /// </summary>
        protected virtual void OnEntitiesReady()
        {

        }

        private void PersonCarGeneration()
        {
            foreach (var entity in _entities.Values.ToList())
            {
                if (entity is CalloutPed ped)
                {
                    if (!ped.GenerateInfoManual) continue;

                    BaseScript.TriggerEvent("PoliceMP:AddPersonManual",
                        ped.NetworkId,
                        ped.FirstName,
                        ped.LastName,
                        ped.BirthDate.ToBinary(),
                        ped.AlcoholLevel,
                        ped.OnCocaine,
                        ped.OnCannabis,
                        ped.OnHeroin,
                        ped.OnEcstasy,
                        ped.HasDrivingLicense,
                        ped.IsBannedFromDriving,
                        ped.DrivingLicensePoints,
                        ped.WearsSeatbelt,
                        ped.Items,
                        ped.Warrants,
                        ped.Charges,
                        ped.Markers);
                }
                else if (entity is CalloutVehicle vehicle)
                {
                    if (!vehicle.GenerateInfoManual) continue;

                    BaseScript.TriggerEvent("PoliceMP:AddCarManual",
                        vehicle.NetworkId,
                        vehicle.Plate,
                        vehicle.Owner,
                        vehicle.HasInsurance,
                        vehicle.HasTax,
                        vehicle.HasMot,
                        vehicle.Markers,
                        vehicle.Items);
                }
            }
        }

        private void SetPedsIntoVehicles()
        {
            var hostPlayer = GetHost();

            // Now loop through again to set the peds into the vehicles where applicable
            foreach (var entity in _entities.ToList())
            {
                if (entity.Value is CalloutVehicle vehicle)
                {
                    // Passengers
                    for (var i = 0; i < vehicle.Passengers.Count; i++)
                    {
                        var seat = VehicleSeat.RightFront;
                        if (i == 1) seat = VehicleSeat.RightRear;
                        else if (i == 2) seat = VehicleSeat.LeftRear;
                        else if (i != 0) break;

                        ClientEventAPI.SetPedIntoVehicle(hostPlayer.Player,
                            GetEntity(vehicle.Passengers[i]).NetworkId,
                            vehicle.NetworkId,
                            (int)seat);
                    }

                    // Driver
                    if (!string.IsNullOrEmpty(vehicle.Driver))
                        ClientEventAPI.SetPedIntoVehicle(hostPlayer.Player,
                            GetEntity(vehicle.Driver).NetworkId,
                            vehicle.NetworkId,
                            (int)VehicleSeat.Driver);
                }
            }
        }

        /// <summary>
        /// Returns whether we have all the network IDs.
        /// </summary>
        private bool HaveAllNetworkIds()
        {
            return _entities.Values.ToList().FirstOrDefault(v => v.NetworkId < 1) == null;
        }

        /// <summary>
        /// Check whether the callout has a host
        /// </summary>
        /// <returns>True if there is a host</returns>
        public bool HasHost() => GetHost() != null;

        /// <summary>
        /// Check whether the specified player is the host.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>Whether the player is the host</returns>
        public bool IsHost(Player player) => GetHost().Player == player;

        /// <summary>
        /// Check whether the callout has a specific player
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>True if the player is on the callout</returns>
        public bool HasPlayer(Player player) => Players.FirstOrDefault(p => p.Player == player) != null;

        /// <summary>
        /// Checks if there are any players on the callout.
        /// </summary>
        /// <returns>Whether there are any players on the callout</returns>
        public bool HasAnyPlayer() => Players.Count > 0;

        /// <summary>
        /// Gets the host player
        /// </summary>
        /// <returns>The host player</returns>
        public CalloutPlayer GetHost() => Players.FirstOrDefault(p => p.IsHost);

        /// <summary>
        /// Gets the CalloutPlayer object corresponding to the specified player
        /// if they are on the callout.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The CalloutPlayer</returns>
        public CalloutPlayer GetPlayer(Player player) => Players.FirstOrDefault(p => p.Player == player);

        /// <summary>
        /// Adds a player to the callout.
        /// </summary>
        /// <param name="player">The player to add</param>
        /// <returns>Whether the player was added successfully</returns>
        public bool AddPlayer(Player player)
        {
            if (HasPlayer(player))
            {
                Debug.WriteLine($"CALLOUTS: Attempted to add player to callout who was already in callout ({player.Name} to callout {Id})");
                return false;
            }

            if (!HasAnyPlayer())
            {
                Start();
            }

            var calloutPlayer = new CalloutPlayer(player);
            Players.Add(calloutPlayer);

            SetBlips();

            return true;
        }

        /// <summary>
        /// Called when a player arrives at the callout.
        /// </summary>
        /// <param name="player">The player who arrived</param>
        public virtual void OnPlayerArrived(Player player)
        {
            if (!HasPlayer(player)) return;

            var calloutPlayer = GetPlayer(player);
            var time = DateTime.Now;
            calloutPlayer.HasArrived = true;
            calloutPlayer.TimeOfArrival = time;

            if (!FirstPlayerArrived)
            {
                FirstPlayerArrived = true;
                TimeOfArrival = time;
                OnFirstPlayerArrived(player);
            }

            SetBlips();
        }

        protected virtual void OnFirstPlayerArrived(Player player)
        {

        }

        /// <summary>
        /// Called straight after the callout has been started
        /// </summary>
        protected virtual void OnStarted() { }

        /// <summary>
        /// Called straight after the callout has ended
        /// </summary>
        /// <param name="result"></param>
        protected virtual void OnEnded(CalloutResult result) { }

        /// <summary>
        /// Sets the blips for the callout.
        /// </summary>
        private void SetBlips()
        {
            Players.ToList().ForEach(p => SetBlips(p));
        }

        private void SetBlips(CalloutPlayer player)
        {
            try
            {
                ClientEventAPI.RemoveAllBlips(player.Player);

                if (!FirstPlayerArrived && IsSearch)
                {
                    Debug.WriteLine("Doing the set blips for search mate");
                    ClientEventAPI.AddBlipForRadius(player.Player, (int)BlipSprite.Standard, Location, 200f, (int)BlipColor.Yellow, true, false, Title);
                    return;
                }

                // If there are no callout entities, or the player does not have the entities spawned on their client,
                // or if the player is not within range, or if none of the entities have blips enabled,
                // set a blip for the general callout position
                if (_entities.Count == 0 ||
                    !player.WithinRange ||
                    _entities.Where(e => e.Value.HasBlip).Count() == 0)
                {
                    ClientEventAPI.AddBlipForCoord(
                        player.Player,
                        (int)BlipSprite.Standard,
                        Location,
                        (int)BlipColor.Yellow,
                        true,
                        false,
                        Title);

                    return;
                }

                // Else, set a blip for all the entities that have blips
                foreach (var entity in _entities.Values.ToList())
                {
                    if (!entity.HasBlip) continue;
                    ClientEventAPI.AddBlipForEntity(
                        player.Player,
                        entity.NetworkId,
                        (int)entity.BlipSprite,
                        (int)entity.BlipColor,
                        false,
                        true,
                        entity.Key);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Removes a player from the callout.
        /// </summary>
        /// <param name="player">The player to remove</param>
        /// <returns>Whether the player was removed successfully</returns>
        public virtual bool RemovePlayer(Player player)
        {
            if (!HasPlayer(player))
            {
                Debug.WriteLine($"CALLOUTS: Attempted to remove player from callout who was not in callout ({player.Name} from callout {Id})");
                return false;
            }

            var calloutPlayer = GetPlayer(player);
            var wasHost = calloutPlayer.IsHost;

            Players.Remove(calloutPlayer);
            ClientEventAPI.RemoveAllBlips(calloutPlayer.Player);

            Debug.WriteLine($"CALLOUTS: Player {player.Name} was removed from callout {Id}");

            if (Players.Count < 1)
            {
                Debug.WriteLine($"CALLOUTS: Abandoning callout {Id} due to no players left.");
                End(CalloutResult.AllPlayersLeft);
                return true;
            }

            if (wasHost)
            {
                Debug.WriteLine($"CALLOUTS: Host player {player.Name} left callout {Id}... selecting new host.");
                Players.ToList().ForEach(p =>
                    ClientEventAPI.SendChatMessage(p.Player, "Host player has left the callout. The entities might have also despawned."));
            }

            return true;
        }

        /// <summary>
        /// Adds an entity to the callout.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        protected void AddEntity(CalloutEntity entity)
        {
            if (entity == null || _entities == null)
                return;

            if (_entities.ContainsKey(entity.Key))
                throw new Exception("Trying to add entity with a key that already exists");

            _entities.Add(entity.Key, entity);
        }

        /// <summary>
        /// Gets a callout entity by its key
        /// </summary>
        /// <param name="key">The entity key</param>
        /// <returns>The entity or null</returns>
        protected CalloutEntity GetEntity(string key)
        {
            return _entities[key];
        }
    }
}
