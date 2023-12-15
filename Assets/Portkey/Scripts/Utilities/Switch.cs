using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.Utilities
{

    public class Switch : MonoBehaviour

    {
        public Image On;
        public Image Off;
        public Image img;
        internal int index;

        void Start()
        {
        }
        
        public void Initialize(int initialState)
        {
            index = initialState;
            if (index == 1)
            {
                ON();
            }
            if (index == 0)
            {
                OFF();
            }
        }

        public int Update()
        {
            return index;
        }

        public void ON()
        {
            index = 1;
            Off.gameObject.SetActive(true);
            On.gameObject.SetActive(false);
        }

        public void OFF()
        {
            index = 0;
            On.gameObject.SetActive(true);
            Off.gameObject.SetActive(false);
        }
        


    }
}