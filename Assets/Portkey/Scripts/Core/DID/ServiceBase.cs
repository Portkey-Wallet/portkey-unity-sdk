namespace Portkey.Core
{
    /// <summary>
    /// Service base class for all services to inherit from.
    /// </summary>
    /// <typeparam name="T">Any IHttp implementation for requesting through HTTP to an API.</typeparam>
    public abstract class ServiceBase<T> where T : IHttp
    {
        protected T Http { private set; get; }

        protected ServiceBase(T http)
        {
            Http = http;
        }
    }
}
