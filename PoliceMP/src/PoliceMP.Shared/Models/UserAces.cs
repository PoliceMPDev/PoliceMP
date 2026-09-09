using System.Dynamic;

namespace PoliceMP.Shared.Models
{
    public class UserAces
    {
        public bool IsWhiteListed { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsTierTwo { get; set; }
        public bool IsTierOne { get; set; }
        public bool IsZoflora { get; set; }
        public bool IsModerator { get; set; }
        public bool IsSeniorModerator { get; set; }
        public bool IsAfoTrained { get; set; }
        public bool IsRpuTrained { get; set; }
        public bool IsCidTrained { get; set; }
        public bool IsNpasTrained { get; set; }
        public bool IsMpuTrained { get; set; }
        public bool IsDsuTrained { get; set; }
        public bool IsBtpTrained { get; set; }
        public bool IsTsgTrained { get; set; }
        public bool IsFireTrained { get; set; }
        public bool IsBasicDonator { get; set; }
        public bool IsProDonator { get; set; }
        public bool IsCivTrained { get; set; }
        public bool IsCivCommand { get; set; }
        public bool IsSeniorCiv { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsDigitalTeam { get; set; }
        public bool IsControl { get; set; }
        public bool IsRetired { get; set; }
        public bool IsHighwaysTrained { get; set; }
        public bool IsHighwaysManager { get; set; }
        public bool IsCollegeStaff { get; set; }
        public bool IsFireBoroughCommander { get; set; }
        public bool IsFireStationCommander { get; set; }
        public bool IsFireOfficer { get; set; }
        public bool IsNhsHems { get; set; }
        public bool IsNhsParamedic { get; set; }
        public bool IsNhsClinicalAdv { get; set; }
        public bool IsNhsClinicalTl { get; set; }
        public bool IsNhsDoctor { get; set; }
        public bool IsNhsSectionLeader { get; set; }
        public bool IsNhsHemsTl { get; set; }
        public bool IsCivGunTrained { get; set; }
        public bool IsBandOne { get; set; }
        public bool IsBandTwo { get; set; }
        public bool IsBandThree { get; set; }
        public bool IsBandFour { get; set; }
        public bool HasCityPoliceDlc { get; set; }
        public bool HasNhsBloodDlc { get; set; }
        public bool IsCommsTrained { get; set; }
        public bool IsHartTrained { get; set; }
        public bool IsFimTrained { get; set; }
		public bool IsJruTrained { get; set; }
		public bool IsTaserTrained { get; set; }
		public bool IsAroTrained { get; set; }
        public bool IsRuralTrained { get; set; }
        public bool IsCoastTrained { get; set; }
        public bool IsMrescueTrained { get; set; }
        public bool IsRNLITrained { get; set; }
        public bool IsHeadModTrained { get; set; }
        public bool IsSurviveDonator { get; set; }
        public bool IsBtpDonator { get; set; }
        public bool IsElecDlc { get; set; }
        public bool IsFruTrained { get; set; }
        public bool IsAssMedDir { get; set; }
        public bool IsDirOfOps { get; set; }
        public bool IsNhsChiefMedOfficer { get; set; }
        public bool IsLfbChiefCommissioner { get; set; }
        public bool IsLfbDepCommissioner { get; set; }
        public bool IsLoa { get; set; }
        public bool IsStudentPara { get; set; }
        public bool IsQaTeam { get; set; }
        public bool IsHartTL { get; set; }
        public bool IsJamPackDlc { get; set; }
        public bool IsMatrix { get; set; }
        public bool IsFireTrainer { get; set; }
        public bool IsTacOpsDlc { get; set; }
        public bool IsBeepDoctor { get; set; }
        public bool IsDevFunNight { get; set; }
        public bool IsChiefInspector { get; set; }

        public UserAces()
        {
        }
    }
}