using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;

namespace PoliceMP.Core.Client.Scripts
{
    public abstract class Script : IDisposable
    {
        public static ILogger<Script> _log = new Logger<Script>();

        public bool IsStarted { get; private set; }

        protected virtual void OnStart()
        {
        }

        protected virtual Task OnStartAsync()
        {
            return Task.FromResult(0);
        }

        public async Task StartAsync()
        {
            // ReSharper disable once MethodHasAsyncOverload
            OnStart();
            await OnStartAsync();
            IsStarted = true;
        }

        protected virtual void OnStop()
        {
        }

        protected virtual Task OnStopAsync()
        {
            return Task.FromResult(0);
        }

        public async Task StopAsync()
        {
            // ReSharper disable once MethodHasAsyncOverload
            OnStop();
            await OnStopAsync();
            IsStarted = false;
        }

        public static async Task Delay(int ms)
        {
            var stackTrace = new StackTrace();
            var callee = stackTrace.GetFrame(3);
            var method = callee.GetMethod();
            var name =
                $"{method.DeclaringType}.{method.Name}({method.DeclaringType?.FullName}:{callee.GetFileLineNumber()}";

            // _log.Debug($"{name} is yielding for {ms}ms");
            await BaseScript.Delay(ms);
            // _log.Debug($"Reactivating {name}...");
        }

        protected async Task Delay(TimeSpan delay)
        {
            await BaseScript.Delay((int)delay.TotalMilliseconds);
        }

        protected void RegisterDecor(string name, DecorType type)
        {
            API.DecorRegister(name, (int)type);
        }

        public void Dispose()
        {
        }
    }
}