using System.Collections;
using System.Collections.Generic;
using CitizenFX.Core;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts
{
    public interface IAiCallout
    {
        /**
         * Unique Identifier for this Callout
         */
        int Id { get; set; }
        
        /// <summary>
        /// A link back to the controller to call public methods on it.
        /// </summary>
        AiCalloutsController Controller { get; set; }

        /**
         * Title for the emergency call notification.
         */
        string Title();

        /**
         * Subtitle for the emergency call notification.
         */
        string Subtitle();

        /**
         * Body text for the emergency call notification.
         */
        string Body();

        /**
         * A list of the divisions you wish this callout to target.
         * Set to null to target all divisions.
         * This should match the selected division from F1->Change Role for that division to be notified.
         */
        List<UserRole>? TargetedDivisions();

        public void AddAttendee(Player player, UserRole division);

        /**
         * Sets the status of the callout.
         */
        void SetStatus(AiCalloutStatus created);

        /**
         * Gets the current status of the callout.
         */
        AiCalloutStatus GetStatus();

        /**
         * The callout has been announced to players.
         * Spawn in any Entities you need.
         */
        void OnCreate();

        /**
         * Can this call be started? (e.g. check if a unit is within a certain radius, return true if they have).
         * Only called when the status is created.
         */
        bool CanBeStarted();

        /**
         * Start the callout.
         * Usually when the first unit arrives on scene.
         */
        void OnStart();

        /**
         * Can the callout be marked as resolved?
         * Only called when the status is Started.
         */
        bool CanBeResolved();

        /**
         * Resolve the callout
         * This might mean stopping certain behaviours and/or spawning a new Callout (for example, CID need to attend to a dead body)
         * Return null for no new callout needed, return a callout if you wish to start a new 999 call (e.g. calling up CID for bodies)
         */
        IAiCallout? OnResolve();

        /**
         * Can this callout be completed?
         * Only called when the status is Resolved
         */
        bool CanBeCompleted();

        /**
         * Complete the callout
         * Clean up any leftover Peds or Vehicles etc. to reset the area.
         */
        void OnComplete();

        /// <summary>
        /// Blip to draw for the callout
        /// </summary>
        /// <returns></returns>
        AiCalloutBlip CalloutBlip();

        bool IsPlayerAttached(Player player);

        bool HasAttendees();

        /**
         * Has the callout expired? This could be due to nobody accepting the callout.
         */
        bool HasExpired();

        void DetachPlayer(Player player);

        void DetachAllAttendees();
        
        List<(Player player, UserRole role)> GetAttendees();
    }
}