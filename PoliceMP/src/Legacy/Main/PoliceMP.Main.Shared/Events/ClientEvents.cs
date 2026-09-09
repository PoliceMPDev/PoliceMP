namespace PoliceMP.Main.Shared.Events
{
    /// <summary>
    ///     All the events that are triggered on the client.
    /// </summary>
    public static class ClientEvents
    {
        /**
         * Person Actions
         */
        public const string ACTION_GRAB = "PoliceMP:ActionGrab";
        public const string ACTION_CUFF = "PoliceMP:ActionCuff";
        public const string ACTION_KNEEL = "PoliceMP:ActionKneel";
        public const string ACTION_FOLLOW = "PoliceMP:ActionFollow";
        public const string ACTION_HANDSUP = "PoliceMP:ActionHandsup";
        public const string ACTION_LIE_DOWN = "PoliceMP:ActionLieDown";
        public const string ACTION_STOP = "PoliceMP:ActionStop";
        public const string ACTION_STAND = "PoliceMP:ActionStand";
        public const string ACTION_BOOK = "PoliceMP:ActionBook";
        public const string ACTION_RELEASE = "PoliceMP:ActionRelease";
        public const string ACTION_SEARCH = "PoliceMP:ActionSearch";
        public const string ACTION_ASK_FOR_ID = "PoliceMP:ActionAskForId";
        public const string ACTION_RUN_NAME = "PoliceMP:ActionRunName";
        public const string ACTION_BREATHALYSE = "PoliceMP:ActionBreathalyse";
        public const string ACTION_DRUGALYSE = "PoliceMP:ActionDrugalyse";
        public const string ACTION_QUESTION = "PoliceMP:ActionQuestion";
        public const string ACTION_TICKET = "PoliceMP:ActionTicket";
        public const string ACTION_WARNING = "PoliceMP:ActionWarning";

        public const string ACTION_MIMIC = "PoliceMP:ActionMimic";
        public const string ACTION_RUN_PLATE = "PoliceMP:ActionRunPlate";
        public const string ACTION_SEARCH_CAR = "PoliceMP:ActionSearchCar";
        public const string ACTION_RELEASE_CAR = "PoliceMP:ActionReleaseCar";
        public const string ACTION_STEP_OUT = "PoliceMP:ActionStepOut";

        public const string ACTION_PRISONER_TRANSPORT = "PoliceMP:ActionPrisonerTransport";
        public const string ACTION_TOW_TRUCK = "PoliceMP:ActionTowTruck";

        public const string STOP_PED = "PoliceMP:StopPed";

        /**
         * Arrests
         */
        public const string ADD_ARRESTED_PERSON = "PoliceMP:VelcroStrappedRustlersBurger_19";
        public const string REMOVE_ARRESTED_PERSON = "PoliceMP:VelcroStrappedRustlersBurger_20";

        /**
         * Person Selection
         */
        public const string RECEIVE_PERSON = "PoliceMP:VelcroStrappedRustlersBurger_22";
        public const string RECEIVE_PERSON_ERROR = "PoliceMP:VelcroStrappedRustlersBurger_23";
        public const string ON_PERSON_SELECTED = "PoliceMP:VelcroStrappedRustlersBurger_24";
        public const string ON_PERSON_UNSELECTED = "PoliceMP:VelcroStrappedRustlersBurger_25";

        /**
         * Cars
         */
        public const string RECEIVE_CAR = "PoliceMP:VelcroStrappedRustlersBurger_26";

        public const string RUN_PLATE = "PoliceMP:VelcroStrappedRustlersBurger_28";
        public const string RUN_PLATE_NO_DIALOG = "PoliceMP:VelcroStrappedRustlersBurger_29";

        public const string ENABLE_ANPR = "PoliceMP:VelcroStrappedRustlersBurger_30";

        public const string PULLOVER_SELECT_SPOT = "PoliceMP:VelcroStrappedRustlersBurger_31";
        public const string MIMIC_CAR = "PoliceMP:VelcroStrappedRustlersBurger_32";

        public const string STEP_OUT_CAR = "PoliceMP:VelcroStrappedRustlersBurger_33";
        public const string RELEASE_CAR = "PoliceMP:VelcroStrappedRustlersBurger_34";

        public const string TOW_CAR = "PoliceMP:VelcroStrappedRustlersBurger_35";
        public const string SEARCH_CAR = "PoliceMP:VelcroStrappedRustlersBurger_36";

        public const string ON_INTERACT_WITH_DRIVER = "PoliceMP:OnInteractWithDriver";


        /**
         * Data
         */
        public const string RECEIVE_ALL_OFFENCES = "PoliceMP:VelcroStrappedRustlersBurger_40";
        public const string RECEIVE_ALL_QUESTIONS = "PoliceMP:VelcroStrappedRustlersBurger_42";


        public const string ON_INTERACTION_MENU_CLOSED = "PoliceMP:VelcroStrappedRustlersBurger_41";

        public const string RECEIVE_USER = "PoliceMP:ReceiveUser";
        public const string ON_RECEIVE_EXPERIENCE = "PoliceMP:OnReceiveExperience";


        public const string RECEIVE_SERVER_REQUEST = "PoliceMP:ReceiveServerRequest";
        public const string FAILED_SERVER_REQUEST = "PoliceMP:FailedServerRequest";
    }
}