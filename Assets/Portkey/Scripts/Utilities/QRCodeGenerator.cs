using Portkey.Core;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using ZXing.Unity;

namespace Portkey.Utilities
{
    public class QRCodeGenerator : IQRCodeGenerator
    {
        public Texture2D GenerateQRCode(string data, int width, int height)
        {
            var encoded = new Texture2D(width, height);
            var color32Data = GenerateQRCodeColor32(data, width, height);

            encoded.SetPixels32(color32Data.Pixels);
            encoded.Apply();

            return encoded;
        }

        private static Color32Image GenerateQRCodeColor32(string data, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            var color32Data = writer.Write(data);
            return color32Data;
        }
    }
}