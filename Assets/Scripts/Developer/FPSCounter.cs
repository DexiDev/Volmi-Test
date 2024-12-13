using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.Developer
{
    [DisallowMultipleComponent]
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _fpsText;

        private int _currentFPS;
        private WaitForSeconds _waitSeconds = new WaitForSeconds(1);

        private void OnEnable()
        {
            StartCoroutine(CountFPS());
        }

        private void OnDisable()
        {
            StopCoroutine(CountFPS());
            StopCoroutine(SecondsCounter());
        }

        private IEnumerator CountFPS()
        {
            StartCoroutine(SecondsCounter());
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _currentFPS++;
            }
        }

        private IEnumerator SecondsCounter()
        {
            while (true)
            {
                yield return _waitSeconds;
                _fpsText.text = _currentFPS.ToString();
                _currentFPS = 0;
            }
        }

    }
}