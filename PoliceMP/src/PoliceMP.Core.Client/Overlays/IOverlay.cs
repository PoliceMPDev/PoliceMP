namespace PoliceMP.Core.Client.Overlays
{
    public interface IOverlay
    {
        bool Enabled { get; }
        void Enable();
        void Disable();
    }
}