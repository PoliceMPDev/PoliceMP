using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.Manual
{
    public class Manual : Script
    {
        #region Constants

        private const float BitePoint = 0.35f;
        private const float StallRpm = 0.15f;
        private const float IdleRpm = 0.3f;
        private const float PosX = 0.5f;
        private const float PosY = 0.5f;
        private const float Scale = 0.5f;
        private const int Font = 4;
        private const float ClutchStep = 0.2f;

        private const float FinalDrive = 10.06f;
        private const float TireCircumference = 6.9f; // feet
        private const float MaxEngineRpm = 7000f;
        private const float WheelRadius = 0.75f; // meters

        private static readonly Dictionary<int, float> GearRatios = new Dictionary<int, float>
        {
            { -1, 3.642f }, // Reverse gear
            { 1, 3.785f }, // 1st gear
            { 2, 2.325f }, // 2nd gear
            { 3, 1.625f }, // 3rd gear
            { 4, 1.272f }, // 4th gear
            { 5, 1.000f }, // 5th gear
            { 6, 0.795f } // 6th gear
        };

        #endregion

        private readonly ITickManager _tickManager;
        private readonly ILegacyClientCommunicationsManager _comms;

        private bool _isManualDriving;
        private float _clutchPosition;
        private int _currentGear;
        private bool _engineStalled;

        private int CurrentGear
        {
            get => _currentGear;
            set => _currentGear = Clamp(value, -1, 6);
        }

        private static Vehicle Vehicle => Game.PlayerPed.CurrentVehicle;
        private static float VehicleSpeed => Vehicle?.Speed ?? 0f;
        private static float EngineRpm => Vehicle?.CurrentRPM ?? 0f;
        private bool IsClutchPressed => _clutchPosition > 0.8f;
        private bool IsInGear => _currentGear != 0;

        private float ClutchEngagement =>
            1 - Clamp((_clutchPosition - BitePoint) / (1 - BitePoint), 0f, 1f);

        public Manual(ITickManager tickManager, ILegacyClientCommunicationsManager comms)
        {
            _tickManager = tickManager;
            _comms = comms;

            comms.On(ClientEvents.SetManualCarMode, (bool isManual) => SetManualCarMode(isManual));
        }

        protected override Task OnStartAsync()
        {
            _tickManager.On(OnTick);
            return Task.FromResult(0);
        }

        private void SetManualCarMode(bool isManual)
        {
            _isManualDriving = isManual;
            if (!isManual)
            {
                CurrentGear = 0;
                _clutchPosition = 0f;
            }
            else
            {
                _comms.ToClient(ClientEvents.SetAutopilotState, false);
            }
        }

        private Task OnTick()
        {
            if (!_isManualDriving) return Task.FromResult(0);
            var player = Game.PlayerPed;
            if (!ValidateVehicle(player)) return Task.FromResult(0);

            Vehicle.IsHandbrakeForcedOn = false;

            HandleClutchInput();
            HandleGearShifts();
            UpdateDrivetrain();
            HandleStalling();
            EnforceGearLimits();
            DrawGearUi();
            HandleEngineDrag();
            BlockInvalidMovement();

            return Task.FromResult(0);
        }

        private bool ValidateVehicle(Ped player)
        {
            if (Vehicle == null || !Vehicle.Exists() || Vehicle.Driver != player)
            {
                CurrentGear = 0;
                _clutchPosition = 0f;
                return false;
            }

            if (Vehicle.ClassType is VehicleClass.Helicopters or VehicleClass.Planes)
            {
                _isManualDriving = false;
                return false;
            }

            return true;
        }

        private void HandleClutchInput()
        {
            if (MenuController.IsAnyMenuOpen()) return;

            DisableScrollControls();

            if (Game.IsControlJustPressed(0, Control.PrevWeapon))
            {
                _clutchPosition = Clamp(_clutchPosition + ClutchStep, 0f, 1f);
            }
            else if (Game.IsControlJustPressed(0, Control.NextWeapon))
            {
                _clutchPosition = Clamp(_clutchPosition - ClutchStep, 0f, 1f);
            }
        }

        private void DisableScrollControls()
        {
            Game.DisableControlThisFrame(0, Control.PrevWeapon);
            Game.DisableControlThisFrame(0, Control.NextWeapon);
            Game.DisableControlThisFrame(0, Control.VehicleSelectNextWeapon);
            Game.DisableControlThisFrame(0, Control.VehicleNextRadio);
            Game.DisableControlThisFrame(0, Control.WeaponWheelPrev);
            Game.DisableControlThisFrame(0, Control.WeaponWheelNext);
        }

        private void HandleGearShifts()
        {
            if (MenuController.IsAnyMenuOpen()) return;

            if (Game.IsControlJustPressed(0, Control.Aim) && IsClutchPressed && CurrentGear < 6)
            {
                int newGear = CurrentGear + 1;
                float requiredRpm = CalculateRpmForGearAndSpeed(newGear, VehicleSpeed);
                CurrentGear = newGear; // Always shift
                if (requiredRpm > 1.0f)
                {
                    ApplySevereBraking(); // Brake if over-revving
                }

                HandleRevMatch();
            }

            if (Game.IsControlJustPressed(0, Control.Attack) && IsClutchPressed)
            {
                if (CurrentGear == 0 && VehicleSpeed > 1f) return;
                int newGear = Math.Max(-1, CurrentGear - 1);
                float requiredRpm = CalculateRpmForGearAndSpeed(newGear, VehicleSpeed);
                CurrentGear = newGear; // Always shift
                if (requiredRpm > 1.0f)
                {
                    ApplySevereBraking(); // Brake if over-revving
                }

                HandleRevMatch();
            }
        }

        private void UpdateDrivetrain()
        {
            // If in neutral or clutch fully disengaged, no power
            if (CurrentGear == 0 || _clutchPosition > 0.95f)
            {
                Vehicle.EnginePowerMultiplier = 0f;
                return;
            }

            var targetRpm = CalculateTargetRpm();
            var clutchEffect = ClutchEngagement;

            // Update engine RPM based on wheel speed and clutch
            Vehicle.CurrentRPM = _engineStalled
                ? 0
                : Clamp(EngineRpm + (targetRpm - EngineRpm) * clutchEffect, 0f, 1f);

            // Apply engine braking when not accelerating
            if (clutchEffect > 0.1f && Game.GetControlNormal(0, Control.VehicleAccelerate) < 0.1f)
            {
                var engineBrakeForce = (targetRpm - EngineRpm) * 0.05f * clutchEffect;
                if (engineBrakeForce != 0)
                {
                    var newSpeed = VehicleSpeed + engineBrakeForce * Game.LastFrameTime;
                    for (var i = 0; i < API.GetVehicleNumberOfWheels(Vehicle.Handle); i += 2)
                        API.SetVehicleWheelRotationSpeed(Vehicle.Handle, i, newSpeed);
                }
            }

            // Calculate power delivery
            var throttle = Game.GetControlNormal(0, Control.VehicleAccelerate);
            var basePowerMultiplier = throttle * clutchEffect * (CurrentGear == -1 ? -1 : 1);

            // Adjust power based on expected speed
            var expectedSpeed = CalculateExpectedSpeed();
            var speedError = expectedSpeed - VehicleSpeed;
            var speedCorrection = speedError * 2.0f; // Tuning factor
            var adjustedPowerMultiplier = Clamp(basePowerMultiplier + speedCorrection, -1f, 1f);

            // Enforce redline limit: Cap power when RPM is near max
            if (Vehicle.CurrentRPM >= 0.95f && throttle > 0)
            {
                adjustedPowerMultiplier = Math.Min(adjustedPowerMultiplier, 0.1f); // Limits acceleration
            }

            // Apply the final power multiplier
            Vehicle.EnginePowerMultiplier = adjustedPowerMultiplier;
        }

        private float CalculateGearMaxSpeed()
        {
            if (CurrentGear == 0) return 0f;

            const float feetToMeters = 0.3048f;
            var ratio = GearRatios[CurrentGear] * FinalDrive;
            const float tireCircumferenceMeters = TireCircumference * feetToMeters;

            return (MaxEngineRpm * tireCircumferenceMeters) / (ratio * 60); // No 1.1f
        }

        private float CalculateTargetRpm()
        {
            if (CurrentGear == 0) return 0f;

            var wheelRpm = (VehicleSpeed * 60) / (2 * (float)Math.PI * WheelRadius);
            var drivetrainRatio = GearRatios[CurrentGear] * FinalDrive;
            return Clamp(wheelRpm * drivetrainRatio / MaxEngineRpm, 0f, 1f);
        }

        private float CalculateExpectedSpeed()
        {
            if (CurrentGear == 0) return 0f;

            const float feetToMeters = 0.3048f;
            var ratio = GearRatios[CurrentGear] * FinalDrive;
            const float tireCircumferenceMeters = TireCircumference * feetToMeters;
            float currentEngineRpm = Vehicle.CurrentRPM * MaxEngineRpm;

            return (currentEngineRpm * tireCircumferenceMeters) / (ratio * 60);
        }

        private void HandleEngineDrag()
        {
            if (CurrentGear == 1 && _clutchPosition > BitePoint && _clutchPosition < 1.0f && VehicleSpeed < 2f)
            {
                Vehicle.CurrentRPM = Math.Max(EngineRpm, IdleRpm);
                API.SetVehicleForwardSpeed(Vehicle.Handle, VehicleSpeed + 0.1f);
            }

            if (CurrentGear == 0 || _clutchPosition > BitePoint) return;

            if (CurrentGear == 1 && VehicleSpeed < 2f && ClutchEngagement > 0.2f)
                Vehicle.CurrentRPM = Math.Max(EngineRpm, IdleRpm);

            var minSpeed = CurrentGear switch
            {
                -1 => 2f,
                1 => 0.5f,
                _ => (CurrentGear - 1) * 8f / 3.6f
            };

            if (VehicleSpeed < minSpeed && Game.GetControlNormal(0, Control.VehicleAccelerate) < 0.1f)
                Vehicle.SetEngineState(false);
        }

        private void BlockInvalidMovement()
        {
            if (CurrentGear == 0 || _clutchPosition > 0.95f)
            {
                Vehicle.EnginePowerMultiplier = 0f;
                Game.DisableControlThisFrame(0, Control.VehicleAccelerate);
            }

            if (CurrentGear == -1 && VehicleSpeed > 2f)
            {
                Vehicle.EnginePowerMultiplier = 0f;
                Game.DisableControlThisFrame(0, Control.VehicleAccelerate);
            }
        }

        private void EnforceGearLimits()
        {
            if (CurrentGear <= 0) return;

            var maxSpeed = CalculateGearMaxSpeed();
            if (VehicleSpeed > maxSpeed)
            {
                Vehicle.CurrentRPM = Clamp(EngineRpm - 0.1f, 0f, 1f);
            }
        }

        private float CalculateRpmForGearAndSpeed(int gear, float speed)
        {
            if (gear == 0 || !GearRatios.ContainsKey(gear)) return 0f;

            const float feetToMeters = 0.3048f;
            var ratio = GearRatios[gear] * FinalDrive;
            const float tireCircumferenceMeters = TireCircumference * feetToMeters;

            float wheelRpm = (speed * 60) / tireCircumferenceMeters;
            float engineRpm = wheelRpm * ratio;
            return engineRpm / MaxEngineRpm;
        }

        private void ApplySevereBraking()
        {
            Vehicle.EnginePowerMultiplier = -1.0f;
            Game.PlaySound("Engine_Damage", "DLC_SM_Car_Race_Announcer_Sounds");
        }

        private void HandleRevMatch()
        {
            var targetRpm = CalculateTargetRpm() * 1.2f;
            Vehicle.CurrentRPM = Clamp(targetRpm, IdleRpm, 1f);
            Game.PlaySound("Accelerate", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        }

        private void HandleStalling()
        {
            var shouldStall = IsInGear &&
                              ClutchEngagement > 0.1f &&
                              EngineRpm < StallRpm &&
                              !Vehicle.IsInBurnout &&
                              _clutchPosition < BitePoint;

            if (CurrentGear == 1 && VehicleSpeed < 2f) shouldStall = false;

            if (shouldStall && Vehicle.IsEngineRunning)
            {
                Vehicle.IsEngineRunning = false;
                _engineStalled = true;
                Game.PlaySound("Stall", "DLC_SM_Car_Race_Announcer_Sounds");
            }
            else if (Game.IsControlJustPressed(0, Control.VehicleAccelerate) && _engineStalled)
            {
                Vehicle.IsEngineRunning = true;
                _engineStalled = false;
            }
        }

        private void DrawGearUi()
        {
            var gearText = CurrentGear switch
            {
                -1 => "R",
                0 => "N",
                _ => CurrentGear.ToString()
            };

            API.SetTextScale(Scale, Scale);
            API.SetTextFont(Font);
            API.SetTextCentre(true);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName(gearText);
            API.EndTextCommandDisplayText(PosX, PosY);

            if (_clutchPosition > 0.05f)
            {
                float barWidth = 0.1f;
                float barHeight = 0.02f;
                float clutchWidth = barWidth * _clutchPosition;

                API.DrawRect(PosX, PosY + 0.04f, barWidth, barHeight, 100, 100, 100, 150);
                API.DrawRect(
                    PosX - (barWidth / 2) + (clutchWidth / 2),
                    PosY + 0.04f,
                    clutchWidth,
                    barHeight,
                    255, 255, 255, 200
                );

                float bitePosition = barWidth * BitePoint;
                API.DrawRect(
                    PosX - (barWidth / 2) + bitePosition,
                    PosY + 0.04f,
                    0.005f,
                    barHeight * 1.5f,
                    255, 0, 0, 200
                );
            }

            var rpmWidth = 0.1f * EngineRpm;
            API.DrawRect(PosX, PosY + 0.08f, 0.1f, 0.015f, 100, 100, 100, 150);
            API.DrawRect(PosX - 0.05f + rpmWidth / 2, PosY + 0.08f, rpmWidth, 0.015f, 255, (int)(255 * (1 - EngineRpm)),
                0, 200);
        }

        private static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);
        private static float Clamp(float value, float min, float max) => Math.Min(Math.Max(value, min), max);
    }
}