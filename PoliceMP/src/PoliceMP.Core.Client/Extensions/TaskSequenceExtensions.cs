using CitizenFX.Core;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Extensions
{
    public static class TaskSequenceExtensions
    {
        public static async Task AddRequired(this TaskSequence sequence, Ped targetPed)
        {
            if (targetPed == null) return;

            if (targetPed.IsCuffed)
            {
                await sequence.AddTask.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, AnimationFlags.Loop, 0);
            }
        }
    }
}