using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Client.Interface;

namespace PoliceMP.Core.Client.Abstraction
{
    public delegate Task ChangeBagChangeHandler<T>(T oldValue, T newValue, bool replicated);
        
    public abstract class StateBagProxy<T>
    {
        private readonly StateBag _stateBag;
        private readonly bool _replicated;
        private readonly string _key;
        private readonly ConcurrentDictionary<ChangeBagChangeHandler<T>, int> _cookieDictionary =
            new ConcurrentDictionary<ChangeBagChangeHandler<T>, int>();

        public T Value
        {
            get => Get();
            set => Set(value);
        }

        protected StateBagProxy(StateBag bag, string key, bool replicated = true, T defaultValue = default)
        {
            _stateBag = bag;
            _replicated = replicated;
            _key = key;

            if (_stateBag.Get(_key) is null)
            {
                Set(defaultValue);
            }
        }

        private T Get()
        {
            var value = _stateBag.Get(_key);
            if (value is null)
            {
                return default;
            }
            
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonSerializationException)
            {
                return value;
            }
        }

        public void Set(T value, bool? replicated = null)
        {
            _stateBag.Set(_key, JsonConvert.SerializeObject(value, Formatting.None), replicated ?? _replicated);
        }
        
        public abstract void AddStateBagChangeHandler(ChangeBagChangeHandler<T> handler, bool ignoreIfNoValueChange = false);
        protected void AddStateBagChangeHandler(string bagName, ChangeBagChangeHandler<T> handler, bool ignoreIfNoValueChange = false)
        {
            var cookie = API.AddStateBagChangeHandler(this._key, bagName, new Func<string, string, dynamic, int, bool, Task>(
                async (s, s1, v, reserved, replicated) =>
                {
                    var oldValue = Value;
                    T newValue = v is null ? null : JsonConvert.DeserializeObject<T>(v);

                    if (ignoreIfNoValueChange && EqualityComparer<T>.Default.Equals(newValue,oldValue))
                    {
                        return;
                    }
                    
                    await handler(Value, newValue, replicated);
                }));

            if (!_cookieDictionary.TryAdd(handler, cookie))
            {
                throw new Exception("Cannot add cookie handler!");
            }
        }
        
        public void RemoveStateBagChangeHandler(ChangeBagChangeHandler<T> handler)
        {
            if (_cookieDictionary.TryRemove(handler, out var cookie))
            {
                API.RemoveStateBagChangeHandler(cookie);
            }
        }
    }

    public class GlobalStateBagProxy<T> : StateBagProxy<T>
    {
        private const string BagName = "global";
        public GlobalStateBagProxy(IGlobalStateAccessor globalAccessor, string key, bool replicated = true, T defaultValue = default) 
            : base(globalAccessor.GlobalState, key, replicated, defaultValue)
        {
        }

        public override void AddStateBagChangeHandler(ChangeBagChangeHandler<T> handler, bool ignoreIfNoValueChange = true)
        {
            base.AddStateBagChangeHandler(BagName, handler, ignoreIfNoValueChange);
        }
    }

    public class EntityStateBagProxy<T> : StateBagProxy<T>
    {
        private readonly Entity _entity;
        private string BagName =>
            API.NetworkGetEntityIsNetworked(_entity.Handle)
                ? $"entity:{_entity.NetworkId}"
                : $"localEntity:{_entity.Handle}";
        

        public EntityStateBagProxy(Entity entity, string key, bool replicated = true, T defaultValue = default) : 
            base(entity.State, key, replicated, defaultValue)
        {
            _entity = entity;
        }

        public override void AddStateBagChangeHandler(ChangeBagChangeHandler<T> handler, bool ignoreIfNoValueChange = true)
        {
            base.AddStateBagChangeHandler(BagName, handler, ignoreIfNoValueChange);
        }
    }

    public class PlayerStateBagProxy<T> : StateBagProxy<T>
    {
        private readonly string _bagName;

        private readonly ConcurrentDictionary<ChangeBagChangeHandler<T>, int> _cookieDictionary =
            new ConcurrentDictionary<ChangeBagChangeHandler<T>, int>();
        
        public PlayerStateBagProxy(Player player, string key, bool replicated = true, T defaultValue = default) 
            : base(player.State, key, replicated, defaultValue)
        {
            _bagName = $"player:{player.ServerId}";
        }

        public override void AddStateBagChangeHandler(ChangeBagChangeHandler<T> handler, bool ignoreIfNoValueChange = true)
        {
            base.AddStateBagChangeHandler(_bagName, handler, ignoreIfNoValueChange);
        }
    }
}
