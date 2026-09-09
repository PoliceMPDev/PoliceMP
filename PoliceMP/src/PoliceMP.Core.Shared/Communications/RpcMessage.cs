using System;

namespace PoliceMP.Core.Shared.Communications
{
    public class RpcMessage
    {
        public Guid Id { get; set; }
        public string Event { get; set; }
        public string ReplyEvent { get; set; }
        public string[] Payload { get; set; }
    }
}