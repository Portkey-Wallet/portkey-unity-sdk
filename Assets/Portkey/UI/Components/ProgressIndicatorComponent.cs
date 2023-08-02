using UnityEngine;

namespace Portkey.UI
{
    public class ProgressIndicatorComponent : MonoBehaviour
    {
        [SerializeField] private GameObject complete;
        [SerializeField] private GameObject progress;

        public void SetProgress(bool isComplete)
        {
            complete.SetActive(isComplete);
            progress.SetActive(!isComplete);
        }
    }
}