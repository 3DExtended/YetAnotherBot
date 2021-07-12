using System;

namespace YAB.Plugins.Injectables
{
    public class Lazy<T> where T : class
    {
        private readonly Func<T> _factory;
        private T _instance = null;

        public Lazy(Func<T> factory)
        {
            _factory = factory;
        }

        public T Value
        {
            get
            {
                if (_instance == null)
                {
                    _instance = _factory();
                }

                return _instance;
            }
        }
    }
}