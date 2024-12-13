using System;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Save
{
    public class SaveManager : IInitializable
    {
        private const string Extension = ".db";
        
        public static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        private IObjectResolver _objectResolver;
        private SaveConfig _config;
        
        private bool _isInitialize;

        public event Action OnSave; 
        
        [Inject]
        private void Install(IObjectResolver objectResolver, SaveConfig saveConfig)
        {
            _objectResolver = objectResolver;
            _config = saveConfig;
        }

        public void Initialize()
        {
            if (_isInitialize) return;

            _isInitialize = true;
            
            _jsonSerializerSettings.Converters?.ForEach(_objectResolver.Inject);
            
            TryUpdateToken();
        }

        private void TryUpdateToken()
        {
            var token = PlayerPrefs.GetString($"{nameof(SaveManager)}.Token{Extension}", "");

            if (!string.Equals(token, _config.SaveToken))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetString($"{nameof(SaveManager)}.Token{Extension}", _config.SaveToken);
                OnSave?.Invoke();
            }
        }

        public void AddConverter(JsonConverter jsonConverter)
        {
            _objectResolver.Inject(jsonConverter);
            
            _jsonSerializerSettings.Converters.Add(jsonConverter);
        }
        
        public void SetData(string key, object data)
        {
            if(!_isInitialize) Initialize();
            
            PlayerPrefs.SetString(key + Extension, JsonConvert.SerializeObject(data, _jsonSerializerSettings));
            
            OnSave?.Invoke();
        }
        
        public T GetData<T>(string key)
        {
            if(!_isInitialize) Initialize();
            
            var prefs = PlayerPrefs.GetString(key + Extension);
            return JsonConvert.DeserializeObject<T>(prefs, _jsonSerializerSettings);
        }

        public bool TryGetData<T>(string key, out T result)
        {
            if(!_isInitialize) Initialize();
            
            if (!PlayerPrefs.HasKey(key + Extension))
            {
                result = default;
                return false;
            }
            result = GetData<T>(key);
            
            return result != null;
        }
    }
}