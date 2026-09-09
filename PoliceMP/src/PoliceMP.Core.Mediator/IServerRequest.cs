
namespace PoliceMP.Core.Mediator
{
    /// <summary>
    /// Defines a fire and forget request with no return type - used for commands
    /// </summary>
    public interface IServerRequest : IBaseRequest
    {
    }

    /// <summary>
    /// Defines an event type that should only have one handler defined on the server.
    /// </summary>
    public interface IServerRequest<out TResponse> : IBaseRequest
    {
    }
}