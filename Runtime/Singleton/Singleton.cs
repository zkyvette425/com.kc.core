using System;

namespace KC
{
    public abstract class Singleton<T> : ISingleton  where T : Singleton<T>
    {
        private bool _isDisposed;
        private static T _instance;

        public static T Instance => _instance;

        public bool IsDisposed => _isDisposed;

        protected Singleton()
        {
            
        }
        
        public virtual void Destroy(){}

        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }

            _isDisposed = true;
            
            Destroy();

            _instance = null;
        }

        public void Register()
        {
            _instance = (T)this;
        }
    }
}