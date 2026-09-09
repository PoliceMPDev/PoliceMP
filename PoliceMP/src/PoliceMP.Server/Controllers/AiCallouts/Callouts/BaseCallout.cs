using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Server.Extensions;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts
{
    public abstract class BaseCallout : IAiCallout
    {
        /// <summary>
        /// Current units attached to the callout.
        /// </summary>
        protected List<(Player player, UserRole division)> _attendees = new();

        /// <summary>
        /// Original units that were attached (who may not be attached anymore).
        /// </summary>
        protected readonly List<(Player player, UserRole division)> _originalAttendees = new();

        /// <summary>
        /// Current status of the callout
        /// </summary>
        private AiCalloutStatus _status;

        /// <summary>
        /// When the callout started
        /// </summary>
        protected long StartTimestamp { get; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

        /// <summary>
        /// Callout unique ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A link back to the controller to call public methods on it.
        /// </summary>
        public AiCalloutsController Controller { get; set; }

        /// <summary>
        /// Title for the callout notification
        /// </summary>
        /// <returns></returns>
        public abstract string Title();

        /// <summary>
        /// Subtitle for the callout notification
        /// </summary>
        /// <returns></returns>
        public abstract string Subtitle();

        /// <summary>
        /// Body text for the callout notification
        /// </summary>
        /// <returns></returns>
        public abstract string Body();

        /// <summary>
        /// Which divisions your callout is targeted to
        /// </summary>
        /// <returns>UserRoles to target or null for any unit</returns>
        public List<UserRole> TargetedDivisions()
        {
            return null;
        }

        /// <summary>
        /// Adds a player who is attending the callout
        /// </summary>
        /// <param name="player"></param>
        /// <param name="division"></param>
        public void AddAttendee(Player player, UserRole division)
        {
            _attendees.Add((player, division));
            _originalAttendees.Add((player, division));
        }

        /// <summary>
        /// Set the status of the callout
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(AiCalloutStatus status)
        {
            _status = status;
        }

        /// <summary>
        /// Get the status of the callout
        /// </summary>
        /// <returns></returns>
        public AiCalloutStatus GetStatus()
        {
            return _status;
        }

        public abstract void OnCreate();
        public abstract bool CanBeStarted();
        public abstract void OnStart();
        public abstract bool CanBeResolved();
        public abstract IAiCallout OnResolve();
        public abstract bool CanBeCompleted();
        public abstract void OnComplete();

        public abstract AiCalloutBlip CalloutBlip();

        public bool IsPlayerAttached(Player findPlayer)
        {
            foreach (var (player, _) in _attendees.ToList())
            {
                if (player.Equals(findPlayer))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAttendees()
        {
            return _attendees.Any();
        }

        public bool HasExpired()
        {
            if (_originalAttendees.Any())
            {
                // Its had attendees attached before, so therefore it can never expire.
                return false;
            }

            long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            return (now - StartTimestamp) > 60;
        }

        public void DetachPlayer(Player detachPlayer)
        {
            var itemToRemove = _attendees.Single(r => r.player.Handle == detachPlayer.Handle);
            _attendees.Remove(itemToRemove);
        }

        public void DetachAllAttendees()
        {
            _attendees = new();
        }

        public List<(Player player, UserRole role)> GetAttendees()
        {
            return _attendees.ToList();
        }

        protected static bool IsPedDead(int pedId)
        {
            try
            {
                var health = API.GetEntityHealth(pedId);
                return health <= 0;
            }
            catch (Exception _)
            {
                return true;
            }
        }

        /**
         * Checks if any of the attached players are within the given distance to the callout location
         */
        protected bool CheckIfAnyAttachedPlayerIsCloseEnough(List<(Player player, UserRole division)> players,
            Vector3 targetLocation, int distance)
        {
            if (players != null && players.Any())
            {
                foreach (var (player, _) in players)
                {
                    if (null == player || null == player.Character) continue;

                    Vector3 playerCoords = player.Character.Position;
                    float measuredDistance = playerCoords.Distance(targetLocation, true);

                    if (measuredDistance <= distance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected List<Player> GetAttendeesWithinRadius(Vector3 targetLocation, int distance)
        {
            List<Player> result = new();

            foreach (var (player, _) in _attendees)
            {
                Vector3 playerCoords = player.Character.Position;
                float measuredDistance = playerCoords.Distance(targetLocation, true);

                if (measuredDistance <= distance)
                {
                    result.Add(player);
                }
            }

            return result;
        }

        protected void DeletePedById(int pedId)
        {
            try
            {
                API.DeleteEntity(pedId);
            }
            catch (Exception)
            {
                // Ignore, it will be cleaned up by garbage collection anyway
            }
        }
    }
}