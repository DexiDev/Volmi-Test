using UnityEngine;

namespace Game.Core.Handlers
{
    public class TimeScaleSetter : IHandler
    {
        [SerializeField] private float _timeScale = 0f;
        
        private float _cachedTimeScale;
        
        protected void OnEnable()
        {
            _cachedTimeScale = Time.timeScale;
            SetTimeScale(_timeScale);
        }

        protected void OnDisable()
        {
            SetTimeScale(_cachedTimeScale);
        }

        public void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }
    }
}