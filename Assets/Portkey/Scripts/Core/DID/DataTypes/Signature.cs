namespace Portkey.Core
{
    public class Signature
    {
        public byte[] Buffer { get; private set; }
        
        public Signature(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}