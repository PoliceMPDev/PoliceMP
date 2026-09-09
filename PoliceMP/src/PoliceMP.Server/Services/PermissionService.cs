using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Crmf;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Extensions;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Services
{
    public class PermissionService : IPermissionService
    {
        private static Dictionary<string, UserRole> _userRoles = new();
        private readonly PlayerList _playerList;
        private readonly ILogger<PermissionService> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;

        public PermissionService(PlayerList playerList, ILogger<PermissionService> logger, ILegacyServerCommunicationsManager comms)
        {
            _playerList = playerList;
            _logger = logger;
            _comms = comms;
            comms.On<UserRole>(ServerEvents.SendUserRole, SetUserRole);
            comms.On<string>(ServerEvents.SendUserDivision, SendUserDivision);
        }

        public Task<List<Player>> GetAllAdmins()
        {
            return Task.FromResult(_playerList.ToArray().Where(player => API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth")).ToList());
        }


        public Task<UserAces> GetUserAces(Player player)
        {
            if (player == null || player.Handle == null)
            {
                return Task.FromResult<UserAces>(null);
            }
            
            if (API.IsPlayerAceAllowed(player.Handle, "group.loa"))
            {
                var userAcesLoa = new UserAces
                {
                    IsLoa = true,
                    IsWhiteListed = false,
                    IsAdmin = API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth"),
                    IsDeveloper = API.IsPlayerAceAllowed(player.Handle, "Police.developer"),
                    IsModerator = false,
                    IsSeniorModerator = false,
                    IsAfoTrained = false,
                    IsRpuTrained = false,
                    IsCidTrained = false,
                    IsNpasTrained = false,
                    IsCommsTrained = false,
                    IsMpuTrained = false,
                    IsDsuTrained = false,
                    IsTsgTrained = false,
                    IsBtpTrained = false,
                    IsFireTrained = false,
                    IsBasicDonator = API.IsPlayerAceAllowed(player.Handle, "Don.basic"),
                    IsProDonator = API.IsPlayerAceAllowed(player.Handle, "Don.pro"),
                    IsCivTrained = false,
                    IsCivCommand = false,
                    IsControl = false,
                    IsContentCreator = false,
                    IsDigitalTeam = false,
                    IsRetired = false,
                    IsHighwaysTrained = false,
                    IsHighwaysManager = false,
                    IsCollegeStaff = false,
                    IsSeniorCiv = false,
                    IsFireBoroughCommander = false,
                    IsFireStationCommander = false,
                    IsFireOfficer = false,
                    IsNhsHems = false,
                    IsNhsParamedic = false,
                    IsStudentPara = false,
                    IsNhsClinicalAdv = false,
                    IsNhsClinicalTl = false,
                    IsNhsDoctor = false,
                    IsNhsSectionLeader = false,
                    IsNhsHemsTl = false,
                    IsCivGunTrained = false,
                    IsBandOne = false,
                    IsBandTwo = false,
                    IsBandThree = false,
                    IsBandFour = false,
                    HasCityPoliceDlc = API.IsPlayerAceAllowed(player.Handle, "Don.colp"),
                    HasNhsBloodDlc = API.IsPlayerAceAllowed(player.Handle, "don.blood"),
                    IsHartTrained = false,
                    IsFimTrained = false,
                    IsJruTrained = false,
                    IsTaserTrained = false,
                    IsAroTrained = false,
                    IsRuralTrained = false,
                    IsCoastTrained = false,
                    IsMrescueTrained = false,
                    IsRNLITrained = false,
                    IsHeadModTrained = API.IsPlayerAceAllowed(player.Handle, "group.headmod"),
                    IsTierTwo = false,
                    IsTierOne = false,
                    IsZoflora = false,
                    IsBeepDoctor = false,
                    IsSurviveDonator = API.IsPlayerAceAllowed(player.Handle, "group.survive"),
                    IsBtpDonator = API.IsPlayerAceAllowed(player.Handle, "group.donBtp"),
                    IsElecDlc = API.IsPlayerAceAllowed(player.Handle, "group.elecDlc"),
                    IsChiefInspector = false
                };

                return Task.FromResult(userAcesLoa);
            }

            var userAces = new UserAces
            {
                IsWhiteListed = API.IsPlayerAceAllowed(player.Handle, "Police.whitelisted"),
                IsAdmin = API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth"),
                IsDeveloper = API.IsPlayerAceAllowed(player.Handle, "Police.developer"),
                IsModerator = API.IsPlayerAceAllowed(player.Handle, "Police.modAuth"),
                IsSeniorModerator = API.IsPlayerAceAllowed(player.Handle, "Police.seniorModAuth"),
                IsAfoTrained = API.IsPlayerAceAllowed(player.Handle, "Police.afoTrained"),
                IsRpuTrained = API.IsPlayerAceAllowed(player.Handle, "Police.rpuTrained"),
                IsCidTrained = API.IsPlayerAceAllowed(player.Handle, "Police.cidTrained"),
                IsNpasTrained = API.IsPlayerAceAllowed(player.Handle, "Police.npasTrained"),
                IsCommsTrained = API.IsPlayerAceAllowed(player.Handle, "Police.CommsTrained"),
                IsMpuTrained = API.IsPlayerAceAllowed(player.Handle, "Police.mpuTrained"),
                IsDsuTrained = API.IsPlayerAceAllowed(player.Handle, "Police.dogTrained"),
                IsTsgTrained = API.IsPlayerAceAllowed(player.Handle, "Police.tsg"),
                IsBtpTrained = API.IsPlayerAceAllowed(player.Handle, "Police.btpTrained"),
                IsFireTrained = API.IsPlayerAceAllowed(player.Handle, "Fire.Trained"),
                IsBasicDonator = API.IsPlayerAceAllowed(player.Handle, "Don.basic"),
                IsProDonator = API.IsPlayerAceAllowed(player.Handle, "Don.pro"),
                IsCivTrained = API.IsPlayerAceAllowed(player.Handle, "Civ.Trained"),
                IsCivCommand = API.IsPlayerAceAllowed(player.Handle, "Civ.Command"),
                IsControl = API.IsPlayerAceAllowed(player.Handle, "control.trained"),
                IsContentCreator = API.IsPlayerAceAllowed(player.Handle, "Content.Creator"),
                IsDigitalTeam = API.IsPlayerAceAllowed(player.Handle, "Digital.Team"),
                IsRetired = API.IsPlayerAceAllowed(player.Handle, "Retired"),
                IsHighwaysTrained = API.IsPlayerAceAllowed(player.Handle, "HETO.Trained"),
                IsHighwaysManager = API.IsPlayerAceAllowed(player.Handle, "Highways.Manager"),
                IsCollegeStaff = API.IsPlayerAceAllowed(player.Handle, "Police.collegeStaff"),
                IsSeniorCiv = API.IsPlayerAceAllowed(player.Handle, "SeniorCiv.Trained"),
                IsFireBoroughCommander = API.IsPlayerAceAllowed(player.Handle, "fire.boroughCommander"),
                IsFireStationCommander = API.IsPlayerAceAllowed(player.Handle, "fire.stationCommander"),
                IsFireOfficer = API.IsPlayerAceAllowed(player.Handle, "fire.fireofficer"),
                IsNhsHems = API.IsPlayerAceAllowed(player.Handle, "nhs.hems"),
                IsNhsParamedic = API.IsPlayerAceAllowed(player.Handle, "nhs.paramedic"),
                IsNhsClinicalAdv = API.IsPlayerAceAllowed(player.Handle, "nhs.clinicalADV"),
                IsNhsClinicalTl = API.IsPlayerAceAllowed(player.Handle, "nhs.clinicalTL"),
                IsNhsDoctor = API.IsPlayerAceAllowed(player.Handle, "group.lasDoctor"),
                IsNhsSectionLeader = API.IsPlayerAceAllowed(player.Handle, "nhs.sectionleader"),
                IsNhsHemsTl = API.IsPlayerAceAllowed(player.Handle, "nhs.hemsTL"),
                IsCivGunTrained = API.IsPlayerAceAllowed(player.Handle, "command.givegun"),
                IsBandOne = API.IsPlayerAceAllowed(player.Handle, "staff.bandOne"),
                IsBandTwo = API.IsPlayerAceAllowed(player.Handle, "staff.bandTwo"),
                IsBandThree = API.IsPlayerAceAllowed(player.Handle, "staff.bandThree"),
                IsBandFour = API.IsPlayerAceAllowed(player.Handle, "staff.bandFour"),
                HasCityPoliceDlc = API.IsPlayerAceAllowed(player.Handle, "Don.colp"),
                HasNhsBloodDlc = API.IsPlayerAceAllowed(player.Handle, "don.blood"),
                IsHartTrained = API.IsPlayerAceAllowed(player.Handle, "nhs.hart"),
				IsFimTrained = API.IsPlayerAceAllowed(player.Handle, "control.fim"),
				IsTaserTrained = API.IsPlayerAceAllowed(player.Handle, "trained.taser"),
				IsJruTrained = API.IsPlayerAceAllowed(player.Handle, "trained.jru"),
                IsAroTrained = API.IsPlayerAceAllowed(player.Handle, "Police.aro"),
                IsRuralTrained = API.IsPlayerAceAllowed(player.Handle, "Police.rural"),
                IsCoastTrained = API.IsPlayerAceAllowed(player.Handle, "coast.guard"),
                IsMrescueTrained = API.IsPlayerAceAllowed(player.Handle, "group.mountain"),
                IsRNLITrained = API.IsPlayerAceAllowed(player.Handle, "group.RLNI"),
                IsHeadModTrained = API.IsPlayerAceAllowed(player.Handle, "group.headmod"),
                IsTierTwo = API.IsPlayerAceAllowed(player.Handle, "group.TierTwo"),
                IsTierOne = API.IsPlayerAceAllowed(player.Handle, "group.TierOne"),
                IsZoflora = API.IsPlayerAceAllowed(player.Handle, "group.zoflora"),
                IsSurviveDonator = API.IsPlayerAceAllowed(player.Handle, "group.survive"),
                IsBtpDonator = API.IsPlayerAceAllowed(player.Handle, "group.donBtp"),
                IsElecDlc = API.IsPlayerAceAllowed(player.Handle, "group.elecDlc"),
                IsFruTrained = API.IsPlayerAceAllowed(player.Handle, "group.fru"),
                IsAssMedDir = API.IsPlayerAceAllowed(player.Handle, "group.nhs.assmeddirector"),
                IsDirOfOps = API.IsPlayerAceAllowed(player.Handle, "group.DOO"),
                IsLfbChiefCommissioner = API.IsPlayerAceAllowed(player.Handle, "group.fire.chiefCommissioner"),
                IsNhsChiefMedOfficer = API.IsPlayerAceAllowed(player.Handle, "group.CMO"),
                IsLfbDepCommissioner = API.IsPlayerAceAllowed(player.Handle, "group.fire.deputyCommissioner"),
                IsStudentPara = API.IsPlayerAceAllowed(player.Handle, "nhs.lasStudent"),
                IsQaTeam = API.IsPlayerAceAllowed(player.Handle, "group.qa"),
                IsHartTL = API.IsPlayerAceAllowed(player.Handle, "nhs.hartTL"),
                IsFireTrainer = API.IsPlayerAceAllowed(player.Handle, "fire.trainer"),
                IsJamPackDlc = API.IsPlayerAceAllowed(player.Handle, "group.jam"),
                IsTacOpsDlc = API.IsPlayerAceAllowed(player.Handle, "group.tacOpsDlc"),
                IsBeepDoctor = API.IsPlayerAceAllowed(player.Handle, "group.beepDoc"),
                IsMatrix = API.IsPlayerAceAllowed(player.Handle, "group.matrix"),
                IsDevFunNight = API.IsPlayerAceAllowed(player.Handle, "group.devFunNight"),
                IsChiefInspector = API.IsPlayerAceAllowed(player.Handle, "Police.chiefinspector")
            };

            return Task.FromResult(userAces);
        }

        public Task<UserRole> GetUserRole(Player player)
        {
            var doesContain = _userRoles.TryGetValue(player.Handle, out var userRole);
            if (!doesContain)
            {
                userRole = new UserRole
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Ert
                };
            }
            return Task.FromResult(userRole);
        }

        public Task SetUserRole(Player player, UserRole userRole)
        {
            var doesContain = _userRoles.TryGetValue(player.Handle, out UserRole currentRole);
            if (doesContain)
            {
                lock (_userRoles)
                {
                    _userRoles.Remove(player.Handle);
                    _userRoles.Add(player.Handle, userRole);
                }
            }
            else
            {
                lock (_userRoles)
                {
                    _userRoles.Add(player.Handle, userRole);
                }
            }

            player.Character.State.Set<UserRole>(PlayerStates.CurrentRole, userRole, true);
            player.TriggerEvent("PoliceMP:BroadcastUserBranchDivision", $"{userRole.Branch}#{userRole.Division}");
            return Task.CompletedTask;
        }

        private Task SendUserDivision(Player player, string division)
        {
            BaseScript.TriggerClientEvent("PoliceMP:userDivision", division);
            return Task.CompletedTask;
        }
    }
}
