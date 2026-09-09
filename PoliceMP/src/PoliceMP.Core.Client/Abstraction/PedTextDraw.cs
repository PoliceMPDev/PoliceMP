using CitizenFX.Core;
using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Core.Client.Abstraction
{
    public class PedTextDraw : TextDraw
    {
        private Ped _ped;

        public PedTextDraw(Ped ped, string text, Color color = null) : base(ped.Position, text, color)
        {
            _ped = ped;
        }

        public override void Draw()
        {
            _position = _ped.Bones[Bone.SKEL_Head].Position;
            _position.Z += 1f;
            base.Draw();
        }
    }
}