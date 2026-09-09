using System;
using System.Runtime.Serialization;

namespace PoliceMP.Client.Services.Overlays
{
    [Serializable]
    public class OverlayManagerException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public OverlayManagerException()
        {
        }

        public OverlayManagerException(string message) : base(message)
        {
        }

        public OverlayManagerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OverlayManagerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}