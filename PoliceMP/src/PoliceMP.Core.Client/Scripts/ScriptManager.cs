using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Core.Shared.Scripts;
using PoliceMP.Shared.Constants.Events;

namespace PoliceMP.Core.Client.Scripts
{
    
    public class ScriptManager : IScriptManager
    {
        private readonly ILogger<ScriptManager> _log;
        private readonly IServiceContainer _services;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ITickManager _ticks;
        private readonly Dictionary<string, Type> _scriptTypeMapping;
        private readonly Dictionary<string, Type> _scriptImplementingTypeMap;
        private readonly Dictionary<Type, Type> _scriptImplToServiceTypeMap;
        private List<Script> _scripts = new List<Script>();


        public ScriptManager(ILogger<ScriptManager> log,
            IServiceContainer services,
            Dictionary<Type, Type> scriptTypes,
            ILegacyClientCommunicationsManager comms,
            ITickManager ticks)
        {
            _log = log;
            _services = services;
            _comms = comms;
            _ticks = ticks;
            _scriptTypeMapping = scriptTypes.ToDictionary(s => s.Value.Name.ToLowerInvariant(), s => s.Key);

            foreach (var script in scriptTypes)
            {
                _log.Debug($"Enabled required script: {script.Value.Name}");
                SetScriptEnabled(script.Value.Name, true);
            }

            _scriptTypeMapping = scriptTypes.ToDictionary(s => s.Value.Name.ToLowerInvariant(), s => s.Key);
            _scriptImplementingTypeMap = scriptTypes.ToDictionary(s => s.Key.Name.ToLowerInvariant(), s => s.Value);
            _scriptImplToServiceTypeMap = scriptTypes.ToDictionary(s => s.Value, s => s.Key); // key: TImpl, value: T
        }


        private void SetScriptEnabled(string name, bool enabled)
        {
            lock (_scripts)
            {
                var script = _scripts.SingleOrDefault(s =>
                    s.GetType().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (enabled && script == null)
                {
                    if (!_scriptTypeMapping.TryGetValue(name.ToLowerInvariant(), out var scriptType)
                        && !_scriptImplementingTypeMap.TryGetValue(name.ToLowerInvariant(), out scriptType))
                    {
                        _log.Error($"Failed to find script {name}. Did you add this to the script manager in client.cs?");
                        _comms.ToServer(ScriptManagerEvents.Server.FailedToStart, name, "Not found in ScriptManager.");
                        return;
                    }

                    _log.Debug($"Creating script environment for {name}...");
                    script = (Script)_services.Get(scriptType);
                    if (script == null)
                    {
                        _log.Error($"Failed to find script {scriptType.FullName} in ServiceContainer. Did you add this to the script manager in client.cs?");
                        _comms.ToServer(ScriptManagerEvents.Server.FailedToStart, name, "Not found in ServiceContainer.");
                        return;
                    }

                    _log.Debug($"Starting script {script.GetType().Name}...");
                    var _ = script.StartAsync();

                    _scripts.Add(script);
                    //_comms.ToServer(ScriptManagerEvents.Server.SuccessfullyStarted, name);
                }
            }
        }
    }
}