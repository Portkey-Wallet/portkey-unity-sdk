using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portkey.Core
{
    public interface ITransport
    {
        void Send(string url);
    }
}