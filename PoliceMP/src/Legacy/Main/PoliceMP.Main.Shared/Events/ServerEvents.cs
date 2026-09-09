namespace PoliceMP.Main.Shared.Events
{
    /// <summary>
    ///     All the events that are triggered on the server.
    /// </summary>
    public static class ServerEvents
    {
        /**
         * Person
         */
        public const string GET_RANDOM_PERSON = "PoliceMP:GetRandomPerson";
        public const string GET_LEGAL_PERSON = "PoliceMP:GetLegalPerson";
        public const string GET_PERSON_BY_NAME = "PoliceMP:GetPersonByName";
        public const string GET_CUSTOM_PERSON = "PoliceMP:GetCustomPerson";
        public const string ON_PERSON_UNSELECTED = "PoliceMP:OnPersonUnselected";
        public const string ADD_CUSTOM_PERSON = "PoliceMP:AddCustomPerson";
        public const string ADD_PERSON_MANUAL = "PoliceMP:AddPersonManual";
        public const string ADD_CAR_MANUAL = "PoliceMP:AddCarManual";

        /**
         * Car
         */
        public const string GET_CAR_BY_ID = "PoliceMP:GetCarById";

        /**
         * Data
         */
        public const string GET_ALL_OFFENCES = "PoliceMP:GetAllOffences";
        public const string GET_ALL_QUESTIONS = "PoliceMP:GetAllQuestions";

        public const string UPDATE_USER = "PoliceMP:UpdateUser";
        public const string GIVE_EXPERIENCE = "PoliceMP:GiveExperience";
        public const string REQUEST_USER = "PoliceMP:RequestUser";
    }
}