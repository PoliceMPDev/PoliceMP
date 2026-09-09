// /*
// #pragma warning disable CS0618 // Type or member is obsolete
// using System;
// using System.Linq;
// using CitizenFX.Core;
// using PoliceMP.Core.Server.Communications.Interfaces;
// using PoliceMP.Core.Server.Networking;
// using PoliceMP.Shared.Constants;
//
// namespace PoliceMP.Server.Controllers
// {
//     // ReSharper disable once UnusedType.Global
//     public class IndicatorsController : Controller
//     {
//         public IndicatorsController(ILegacyServerCommunicationsManager comms, IPlayerListAccessor playerListAccessor)
//         {
//             comms.On(ServerEvents.IndicatorsChanged, (int vehicleNetId, Tuple<bool, bool> indicators) =>
//             {
//                 if (comms == null || playerListAccessor.Players == null) return;
//
//                 var vehicle = (Vehicle)Entity.FromNetworkId(vehicleNetId);
//                 if (vehicle == null) return;
//                 foreach (var player in playerListAccessor.Players.ToArray())
//                 {
//                     if (player == null || player.Character == null) continue;
//                     if (vehicle.Position.DistanceToSquared(player.Character.Position) > 500f) continue;
//                     comms.ToClient(player, ClientEvents.IndicatorsChanged, vehicleNetId, indicators);
//                 }
//             });
//         }
//     }
// }
// #pragma warning restore CS0618 // Type or member is obsolete
// */
