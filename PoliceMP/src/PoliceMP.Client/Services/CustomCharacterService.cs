using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services
{
    public class CustomCharacterService : Script, ICustomCharacterService
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IInputService _inputService;
        private readonly ITickManager _ticks;
        private readonly ILogger<CustomCharacterService> _logger;
        private readonly IPermissionService _permissionService;
        private Vector3 _playerPosition = Vector3.Zero;
        private PedOutfit _currentOutfit = null;
        private bool _inCustomisation = false;
        private UserAces _userAces;

        private int hairStyle;
        private int hairPalette;
        private readonly int hatHairStyle = 1;
        private bool noHat;

        public CustomCharacterService(ILegacyClientCommunicationsManager comms, IInputService inputService, ILogger<CustomCharacterService> logger, ITickManager ticks, IPermissionService permissionService)
        {
            _comms = comms;
            _inputService = inputService;
            _logger = logger;
            _ticks = ticks;
            _permissionService = permissionService;
            _comms.On<string>(ServerEvents.OnFinishCharacterCustomisation, OnFinishCustomisation);
            _comms.On(ServerEvents.OnCharacterAppearanceApplied, OnCharacterAppearanceApplied);
            _ticks.On(CustomisationTick);
            _ticks.On(HairClippingTick);
            _ticks.On(TSGAntiFire);
            _ticks.On(LFBAntiFire);
        }

        #region TSG Anti Fire Damage Tick
        private async Task TSGAntiFire()
        {
            await Delay(1500);
            var player = Game.PlayerPed.Handle;

            var tsgHelmet = 226;
            var tsgCap = 243;
            if (API.GetPedPropIndex(player, 0) != tsgHelmet && API.GetPedPropIndex(player, 0) != tsgCap) return;

            var isOnFire = API.IsEntityOnFire(player);
            if (!isOnFire) return;

            var maxHealth = API.GetEntityMaxHealth(player);
            API.SetEntityHealth(player, maxHealth);
            Debug.WriteLine("TSGAntiFire: Healing player to prevent fire damage!!!");
            return;
        }
        #endregion

        #region LFB Anti Fire Damage Tick
        private async Task LFBAntiFire()
        {
            await Delay(500);
            var currentUserRole = _permissionService.CurrentUserRole;
            var player = Game.PlayerPed.Handle;
            if (currentUserRole == null) return;
            if (currentUserRole.Branch != UserBranch.Fire) return;
            var isOnFire = API.IsEntityOnFire(player);
            if (!isOnFire) return;

            var maxHealth = API.GetEntityMaxHealth(player);
            API.SetEntityHealth(player, maxHealth);
            Debug.WriteLine("LFBAntiFire: Healing player to prevent fire damage!!!");
            return;
        }
        #endregion

        #region Anti HairClip Tick
        private async Task HairClippingTick()
        {
            await Delay(500);

            var currentUserRole = _permissionService.CurrentUserRole;
            var player = Game.PlayerPed.Handle;

            if (currentUserRole == null) return;
            if (currentUserRole.Branch == UserBranch.Civ) return;

            if (!_userAces.IsProDonator & !_userAces.IsDeveloper)
            {
                var hair = API.GetPedDrawableVariation(player, 2);
                if (hair != 0)
                {
                    API.SetPedComponentVariation(player, 2, 0, 0, 0);
                }
                return;
            }

            var hairStyle = API.GetPedDrawableVariation(player, 2);
            var hairColour = API.GetPedHairColor(player);
            var hairHighlights = API.GetPedHairHighlightColor(player);


            if (noHat)
            {
                hairStyle = API.GetPedDrawableVariation(player, 2);
                hairColour = API.GetPedHairColor(player);
                API.SetPedComponentVariation(player, 2, hairStyle, 0, 0);
                API.SetPedHairColor(player, hairColour, hairHighlights);
            }

            if (API.GetPedPropIndex(player, 0) == -1) // Empty
            {
                noHat = true;
            }
            else if (API.GetPedPropIndex(player, 0) == 8) // Empty
            {
                noHat = true;
            }
            else if (API.GetPedPropIndex(player, 0) == 11) // Empty
            {
                noHat = true;
            }
            else if (API.GetPedPropIndex(player, 0) == 58) // Control Headset
            {
                noHat = true;
            }
            else if (API.GetPedPropIndex(player, 0) == 0) // Ear Defenders
            {
                noHat = true;
            }
            else
            {
                noHat = false;
            }

            while (!noHat)
            {
                if (hairStyle == 0) return;

                API.SetPedComponentVariation(player, 2, hatHairStyle, 0, 0);
                API.SetPedHairColor(player, hairColour, hairHighlights);

                if (API.GetPedPropIndex(player, 0) == -1) noHat = true;
                if (API.GetPedPropIndex(player, 0) == 8) noHat = true;
                if (API.GetPedPropIndex(player, 0) == 11) noHat = true;

                await Delay(500);
            }

            if (noHat)
            {
                API.SetPedComponentVariation(player, 2, hairStyle, 0, 0);
                API.SetPedHairColor(player, hairColour, hairHighlights);
                await Delay(100);
            }
        }

        #endregion


        private async Task CustomisationTick()
        {
            if (_userAces == null)
            {
                _userAces = await _permissionService.GetUserAces();
                while (_userAces == null)
                {
                    _userAces = await _permissionService.GetUserAces();
                    await Delay(50);
                }
            }

            if (!_inCustomisation) return;
            for (int i = 0; i <= 256; i++)
            {
                if (API.PlayerId() == i) { continue; }
                API.NetworkConcealPlayer(i, true, true);
            }
        }

        private void OnCharacterAppearanceApplied()
        {
            Game.PlayerPed.SetPedOutfit(_currentOutfit);

            Game.PlayerPed.GiveDefaultEquipment(_permissionService.CurrentUserRole, _userAces);
        }

        private async Task OnFinishCustomisation(string json)
        {
            _inCustomisation = false;

            for (int i = 0; i < 500; i++)
            {
                API.NetworkConcealPlayer(i, false, false);
            }
            API.SwitchOutPlayer(API.PlayerPedId(), 0, 1);
            var characterName = await _inputService.ShowKeyboardInput("Character Name", "", 25);
            if (characterName == null)
            {
                API.SwitchInPlayer(API.PlayerPedId());
            }
            var characterData = API.GetResourceKvpString(ResourceKvp.CustomCharacters);
            var customCharacters = new List<CustomCharacter>();
            if (!string.IsNullOrEmpty(characterData))
            {
                customCharacters = JsonConvert.DeserializeObject<List<CustomCharacter>>(characterData);
            }
            var newCharacter = new CustomCharacter
            {
                Name = characterName,
                Appearance = json
            };

            customCharacters.Add(newCharacter);

            var output = JsonConvert.SerializeObject(customCharacters);
            API.SetResourceKvp(ResourceKvp.CustomCharacters, output);

            await Delay(500);

            Game.PlayerPed.SetPedOutfit(_currentOutfit);
            Game.PlayerPed.Position = _playerPosition;
            while (API.IsEntityWaitingForWorldCollision(Game.PlayerPed.Handle) && API.GetPlayerSwitchState() != 5)
            {
                await Delay(10);
            }
            API.SwitchInPlayer(API.PlayerPedId());

            Game.PlayerPed.GiveDefaultEquipment(_permissionService.CurrentUserRole, _userAces);
        }

        public void SetCharacterAppearance(CustomCharacter customCharacter)
        {
            if (customCharacter == null) return;

            _comms.ToServer(ServerEvents.SetPlayerCharacterCustomisation, customCharacter.Appearance);

            _currentOutfit = !string.IsNullOrEmpty(customCharacter.PedOutfit) ? JsonConvert.DeserializeObject<PedOutfit>(customCharacter.PedOutfit) : Game.PlayerPed.FetchCurrentPedOutfit();

            customCharacter.PedOutfit = JsonConvert.SerializeObject(_currentOutfit);

            API.SetResourceKvp(ResourceKvp.LastUsedCharacter, JsonConvert.SerializeObject(customCharacter));
        }

        public void SetCharacterAppearance(string name, string appearanceJson)
        {
            var characterString = API.GetResourceKvpString(ResourceKvp.CustomCharacters);
            if (string.IsNullOrEmpty(characterString)) return;

            var customCharacters = JsonConvert.DeserializeObject<List<CustomCharacter>>(characterString);

            var customCharacter = customCharacters.FirstOrDefault(x => x.Name == name && x.Appearance == appearanceJson);

            if (customCharacter == null) return;

            _comms.ToServer(ServerEvents.SetPlayerCharacterCustomisation, customCharacter.Appearance);

            _currentOutfit = !string.IsNullOrEmpty(customCharacter.PedOutfit) ? JsonConvert.DeserializeObject<PedOutfit>(customCharacter.PedOutfit) : Game.PlayerPed.FetchCurrentPedOutfit();

            customCharacter.PedOutfit = JsonConvert.SerializeObject(_currentOutfit);

            API.SetResourceKvp(ResourceKvp.LastUsedCharacter, JsonConvert.SerializeObject(customCharacter));
        }

        public void SaveOutfitToCharacter(string name, string appearanceJson)
        {
            var characterString = API.GetResourceKvpString(ResourceKvp.CustomCharacters);
            if (string.IsNullOrEmpty(characterString)) return;

            var customCharacters = JsonConvert.DeserializeObject<List<CustomCharacter>>(characterString);

            var customCharacter = customCharacters.FirstOrDefault(x => x.Name == name && x.Appearance == appearanceJson);

            if (customCharacter == null) return;

            customCharacters.Remove(customCharacter);

            var newCharacter = customCharacter;

            newCharacter.PedOutfit = JsonConvert.SerializeObject(Game.PlayerPed.FetchCurrentPedOutfit());

            customCharacters.Add(newCharacter);

            var json = JsonConvert.SerializeObject(customCharacters);

            API.SetResourceKvp(ResourceKvp.CustomCharacters, json);
        }

        public List<CustomCharacter> FetchCustomCharacters()
        {
            var characterData = API.GetResourceKvpString(ResourceKvp.CustomCharacters);
            return !string.IsNullOrEmpty(characterData) ? JsonConvert.DeserializeObject<List<CustomCharacter>>(characterData) : new List<CustomCharacter>();
        }

        /// <summary>
        /// Show the Character Creator
        /// </summary>
        /// <param name="playerName">Players In game Name</param>
        /// <param name="gender">-1 Select, 0 Male, 1 Female</param>
        public async Task ShowCharacterCreator(int gender)
        {
            _inCustomisation = true;
            _playerPosition = Game.PlayerPed.Position;
            _currentOutfit = Game.PlayerPed.FetchCurrentPedOutfit();
            await Delay(500);
            _logger.Debug(JsonConvert.SerializeObject(_currentOutfit));
            _comms.ToServer(ServerEvents.StartCharacterCustomisation, gender);
        }

        public void DeleteCustomCharacter(string name, string appearance)
        {
            var characterString = API.GetResourceKvpString(ResourceKvp.CustomCharacters);
            if (string.IsNullOrEmpty(characterString)) return;

            var customCharacters = JsonConvert.DeserializeObject<List<CustomCharacter>>(characterString);

            var customCharacter = customCharacters.FirstOrDefault(x => x.Name == name && x.Appearance == appearance);

            if (customCharacter == null) return;

            customCharacters.Remove(customCharacter);

            var json = JsonConvert.SerializeObject(customCharacters);

            API.SetResourceKvp(ResourceKvp.CustomCharacters, json);
        }

    }
}