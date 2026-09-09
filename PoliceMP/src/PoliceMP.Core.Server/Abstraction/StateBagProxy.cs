using CitizenFX.Core;
using Newtonsoft.Json;

namespace PoliceMP.Core.Server.Abstraction
{
    public class StateBagProxy<T>
    {
        private readonly StateBag _stateBag;
        private readonly bool _replicated;
        private readonly string _key;

        public bool HasValue => _stateBag.Get(_key) != null;

        public T Value
        {
            get => Get();
            set => Set(value);
        }

        public StateBagProxy(StateBag bag, string key, bool replicated = true, T defaultValue = default)
        {
            _stateBag = bag;
            _replicated = replicated;
            _key = key;

            if (!HasValue)
            {
                Value = defaultValue;
            }
        }

        private T Get()
        {
            var value = _stateBag.Get(_key);
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
            => _stateBag.Set(_key, JsonConvert.SerializeObject(value, Formatting.None), replicated ?? _replicated);
    }
}
