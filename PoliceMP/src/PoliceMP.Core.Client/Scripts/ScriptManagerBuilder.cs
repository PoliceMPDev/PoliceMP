using System;
using System.Collections.Generic;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Shared;

namespace PoliceMP.Core.Client.Scripts
{
    public class ScriptManagerBuilder : IScriptManagerBuilder
    {

        private readonly IServiceContainerBuilder _services;
        private readonly Dictionary<Type, Type> _scriptTypes = new Dictionary<Type, Type>();

        public ScriptManagerBuilder(IServiceContainerBuilder services)
        {
            _services = services;

            _services.Add<IScriptManager>(s => new ScriptManager(
                s.GetRequired<ILogger<ScriptManager>>(),
                s,
                _scriptTypes,
                s.GetRequired<ILegacyClientCommunicationsManager>(),
                s.GetRequired<ITickManager>()));
        }

        public IScriptManagerBuilder Add<T>() where T : Script
            => Add<T, T>();

        public IScriptManagerBuilder Add<T, TImpl>() where TImpl : Script, T
        {
            _services.Add<T, TImpl>();
            _scriptTypes.Add(typeof(T), typeof(TImpl));
            return this;
        }
    }
}