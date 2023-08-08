using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class PINProgressComponent : MonoBehaviour
    {
        [SerializeField] private List<ProgressIndicatorComponent> pinProgressIndicatorList;
        
        public void SetPINProgress(int progress)
        {
            progress = Math.Min(progress, pinProgressIndicatorList.Count);
            
            for (var i = 0; i < pinProgressIndicatorList.Count; i++)
            {
                pinProgressIndicatorList[i].SetProgress(i < progress);
            }
        }
        
        public int GetMaxPINLength()
        {
            return pinProgressIndicatorList.Count;
        }
    }
}