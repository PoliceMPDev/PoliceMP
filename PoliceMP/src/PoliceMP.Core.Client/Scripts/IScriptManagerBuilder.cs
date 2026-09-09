namespace PoliceMP.Core.Client.Scripts
{
    public interface IScriptManagerBuilder
    {
        IScriptManagerBuilder Add<T>() where T : Script;
        IScriptManagerBuilder Add<T, TImpl>() where TImpl : Script, T;
    }
}