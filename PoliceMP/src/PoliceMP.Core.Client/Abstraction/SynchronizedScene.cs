using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PoliceMP.Core.Client.Abstraction
{
    public class NetworkSynchronizedScene
    {
        private struct AnimationDetails
        {
            public string AnimDict { get; set; }
            public string AnimName { get; set; }

            public AnimationDetails(string animDict, string animName)
            {
                AnimDict = animDict;
                AnimName = animName;
            }
        }

        public int NetSceneHandle { get; private set; }
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }
        public float Heading { get; }
        public int AnimTime { get; }

        private List<Entity> _entities = new List<Entity>();
        private List<Ped> _peds = new List<Ped>();
        private ConcurrentDictionary<Ped, AnimationDetails> _startingAnimations = new ConcurrentDictionary<Ped, AnimationDetails>();

        public NetworkSynchronizedScene(
            Vector3 position, 
            Vector3 rotation, 
            int rotationOrder = 2, 
            bool holdLastFrame = false, 
            bool looped = false, 
            int animTime = 0)
        {
            Position = position;
            Rotation = rotation;
            Heading = GameMath.DirectionToHeading(rotation);
            AnimTime = animTime;

            NetSceneHandle = API.NetworkCreateSynchronisedScene(
                position.X,
                position.Y,
                position.Z,
                rotation.X,
                rotation.Y,
                rotation.Z,
                rotationOrder,
                holdLastFrame,
                looped,
                1065353216,
                0f,
                1065353216);
        }

        public void AddEntityToScene(
            Entity entity, 
            string animDict, 
            string animName, 
            float blendInSpeed = 1000f, 
            float blendOutSpeed = -1000f, 
            int flags = 0)
        {
            API.NetworkAddEntityToSynchronisedScene(
                entity.Handle, 
                NetSceneHandle, 
                animDict, 
                animName, 
                blendInSpeed, 
                blendOutSpeed,
                flags);

            _entities.Add(entity);
        }

        public void AddPedToScene(Ped ped, string animDict, string animName, float blendInSpeed = 1000f, float blendOutSpeed = -1000f, int duration = 0, int flags = 0, float playbackRate = 1000f)
        {
            _startingAnimations.TryAdd(ped, new AnimationDetails(animDict, animName));

            API.NetworkAddPedToSynchronisedScene(
                ped.Handle,
                NetSceneHandle,
                animDict,
                animName,
                blendInSpeed,
                blendOutSpeed,
                duration,
                flags,
                playbackRate,
                0);

            _peds.Add(ped);
        }

        public void AttachSceneToEntity(Entity entity, int bone = -1)
        {
            API.NetworkAttachSynchronisedSceneToEntity(NetSceneHandle, entity.Handle, bone);
        }

        public void SetLocalCameraAnim(string animDict, string animName)
        {
            API.NetworkAddSynchronisedSceneCamera(NetSceneHandle, animDict, animName);
        }

        private async Task GetInPosition(bool moveToPosition, bool faceAnimationStart, float moveToThreshold)
        {
            var pedSequences = new Dictionary<Ped, TaskSequence>();
            foreach (var ped in _peds)
            {
                if (_startingAnimations.TryGetValue(ped, out var animationDetails))
                {
                    var sequence = new TaskSequence();

                    if (moveToPosition)
                    {
                        var startingPos = API.GetAnimInitialOffsetPosition(
                            animationDetails.AnimDict,
                            animationDetails.AnimName,
                            Position.X,
                            Position.Y,
                            Position.Z,
                            Rotation.X,
                            Rotation.Y,
                            Rotation.Z,
                            0f,
                            2
                        );

                        if (World.GetDistance(ped.Position, startingPos) > moveToThreshold)
                        {
                            sequence.AddTask.GoTo(startingPos, true, 1500);
                        }
                    }

                    if (faceAnimationStart)
                    {
                        var startingRotation = API.GetAnimInitialOffsetRotation(
                            animationDetails.AnimDict,
                            animationDetails.AnimName,
                            Position.X,
                            Position.Y,
                            Position.Z,
                            Rotation.X,
                            Rotation.Y,
                            Rotation.Z,
                            0f,
                            2
                        );
                        sequence.AddTask.AchieveHeading(startingRotation.Z, 1500);
                    }
                    sequence.AddTask.StandStill(-1);
                    sequence.Close();

                    ped.Task.PerformSequence(sequence);
                    pedSequences.Add(ped, sequence);
                }
            }

            foreach (var ped in _peds)
            {
                var sequence = pedSequences[ped];
                while (ped.TaskSequenceProgress == -1)
                    await BaseScript.Delay(0);

                while (ped.TaskSequenceProgress < sequence.Count - 1) // deliberately don't count the last one as it's stand still
                {
                    await BaseScript.Delay(0);
                }
            }
        }

        public async Task Start(bool moveToPosition = false, float moveToDistance = 0f, bool faceAnimationStart = false, CancellationToken ct = default, int timeoutMs = 10000)
        {
            //TODO: Request control of entities
            //TODO: Freeze entities
            
            await GetInPosition(moveToPosition, faceAnimationStart, moveToDistance);
            
            _peds.ForEach(p =>
            {
                p.HasGravity = false;
                API.FreezeEntityPosition(p.Handle, true);
            });

            API.NetworkStartSynchronisedScene(NetSceneHandle);

            int sceneId = -1;
            while (sceneId == -1)
            {
                sceneId = API.NetworkConvertSynchronisedSceneToSynchronizedScene(NetSceneHandle);
                await BaseScript.Delay(0);
            }

            if (AnimTime == 0)
            {
                while (API.IsSynchronizedSceneRunning(sceneId) && !ct.IsCancellationRequested)
                    await BaseScript.Delay(0);
            }
            else
            {
                await BaseScript.Delay(AnimTime);
            }

            API.NetworkStopSynchronisedScene(NetSceneHandle);
            await BaseScript.Delay(0);

            _peds.ForEach(p =>
            {
                p.HasGravity = true;
                API.FreezeEntityPosition(p.Handle, false);
            });
        }
    }
}
