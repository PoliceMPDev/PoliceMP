namespace PoliceMP.Server.Controllers.AiCallouts
{
    public enum AiCalloutStatus
    {
        /**
         * The 999 has been sent out, but nothing has been spawned yet.
         */
        Created,

        /**
         * The callout has been spawned and AI has started its behaviours.
         */
        Started,

        /**
         * The units attended and have killed the suspects or taken them to custody.
         */
        Resolved,

        /**
         * The callout is ready to be cleaned up when units are no longer in the area.
         */
        Completed,
    }
}