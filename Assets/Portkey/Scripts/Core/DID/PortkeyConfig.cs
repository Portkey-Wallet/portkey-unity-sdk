using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// Portkey configuration object. Contains only config data.
    /// </summary>
    [CreateAssetMenu(fileName = "PortkeyConfig", menuName = "Portkey/PortkeyConfig", order = 1)]
    public class PortkeyConfig : ScriptableObject
    {
        public string apiBaseUrl;
    }
}
