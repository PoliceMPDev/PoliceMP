// using System;
// using CitizenFX.Core;
// using CitizenFX.Core.Native;
// using PoliceMP.Core.Server.Communications.Interfaces;
// using PoliceMP.Core.Server.Interfaces.Factories;
// using PoliceMP.Core.Server.Interfaces.Services;
// using PoliceMP.Server.Services.Interfaces;
//
// namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.JointResponse
// {
//     public class DerailedTrainCallout : BaseCallout, IAiCallout
//     {
//         private readonly IBehaviorService _behaviors;
//         private readonly IServerCommunicationsManager _comms;
//         private readonly IPedInfoService _pedInfo;
//         private readonly IPedInfoFactory _pedInfoFactory;
//
//         private Vector3 _location = new(2311.51f, 2665.96f, 45.50f);
//
//
//         public DerailedTrainCallout(IBehaviorService behaviors, IServerCommunicationsManager _comms,
//             IPedInfoService _pedInfo, IPedInfoFactory _pedInfoFactory)
//         {
//             _behaviors = behaviors;
//             this._comms = _comms;
//             this._pedInfo = _pedInfo;
//             this._pedInfoFactory = _pedInfoFactory;
//         }
//
//
//         private int _pedId;
//
//         public override string Title()
//         {
//             return "999 - A Train has Derailed";
//         }
//
//         public override string Subtitle()
//         {
//             return "";
//         }
//
//         public override string Body()
//         {
//             return "A train has derailed!";
//         }
//
//         public override void OnCreate()
//         {
//             // _pedId = API.CreatePed(
//             //     0,
//             //     (uint)PedHash.Tramp01AMM,
//             //     _location.X,
//             //     _location.Y,
//             //     _location.Z,
//             //     0,
//             //     true,
//             //     false
//             // );
//             // Ped ped = Entity.FromHandle(_pedId) as Ped;
//             // var pedInfo = _pedInfoFactory.Random(ped.NetworkId, Gender.Male);
//             // pedInfo.IsOnCannabis = true;
//             // pedInfo.QuestionList = "DerailedTrainQuestions";
//             // _pedInfo.AddOrUpdate(pedInfo);
//         }
//
//         public override bool CanBeStarted()
//         {
//             if (!HasAttendees())
//             {
//                 return false;
//             }
//
//             // Check if any of the current attached players are close enough to the Ped
//             Vector3 pedPos = API.GetEntityCoords(_pedId);
//             return CheckIfAnyAttachedPlayerIsCloseEnough(_attendees, pedPos, 100);
//         }
//
//         public override void OnStart()
//         {
//             Ped ped = Entity.FromHandle(_pedId) as Ped;
//             API.CreateVehicle(1030400667, 2311.51f, 2665.96f, 45.50f, 1.0f, true, true);
//             API.CreateVehicle(920453016, 2295.16f, 2669.64f, 46.24f, 1.0f, true, true);
//
//
//             // _behaviors.SetPedBehavior<StealACarBehaviour>(ped);        
//         }
//
//         public override bool CanBeResolved()
//         {
//             if (!API.DoesEntityExist(_pedId))
//             {
//                 return true;
//             }
//
//             Ped ped = Ped.FromHandle(_pedId) as Ped;
//             if (API.IsEntityVisible(_pedId) == false)
//             {
//                 return true;
//             }
//
//             if (ped == null)
//             {
//                 return true;
//             }
//
//             return isPedDead(_pedId);
//         }
//
//         public override IAiCallout OnResolve()
//         {
//             return null;
//         }
//
//         public override bool CanBeCompleted()
//         {
//             if (HasAttendees())
//             {
//                 return false;
//             }
//
//             if (!API.DoesEntityExist(_pedId))
//             {
//                 return true;
//             }
//
//             Vector3 pedPos = API.GetEntityCoords(_pedId);
//             return !CheckIfAnyAttachedPlayerIsCloseEnough(_originalAttendees, pedPos, 30);
//         }
//
//         public override void OnComplete()
//         {
//             try
//             {
//                 API.DeleteEntity(_pedId);
//             }
//             catch (Exception)
//             {
//                 // Ignore, it will be cleaned up by garbage collection anyway
//             }
//         }
//         public new int CalloutBlipIcon()
//         {
//             return 795;
//         }
//
//         public new Vector2 CalloutBlipLocation()
//         {
//             return new Vector2(_location.X, _location.Y);
//         }
//     }
// }

