using System.Collections;
using UnityEngine;

namespace Portkey.Utilities
{
    /// <summary>
    /// A static helper class for running coroutines.
    /// This allows classes that are not MonoBehaviours to run coroutines,
    /// thereby allowing abstract class to be inherited without it being a MonoBehaviour.
    /// Example usage: Rest API call classes that do not have side effects and only provides a service.
    /// </summary>
    public static class StaticCoroutine
    {
        private class CoroutineHolder : MonoBehaviour { }

        private static CoroutineHolder _runner;
        private static CoroutineHolder Runner
        {
            get
            {
                if (_runner == null)
                {
                    _runner = new GameObject("Portkey Static Coroutine").AddComponent<CoroutineHolder>();
                    Object.DontDestroyOnLoad(_runner);
                }
                return _runner;
            }
        }

        /// <summary>
        /// Start running a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to run.</param>
        /// <returns>Coroutine returned from running StartCoroutine.</returns>
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return Runner.StartCoroutine(coroutine);
        }
    }
}