using UnityEngine;

namespace Portkey.Core
{
    public interface IQRCodeGenerator
    {
        Texture2D GenerateQRCode(string data, int width, int height);
    }
}