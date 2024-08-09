using System;
using System.Collections.Generic;

namespace KC
{
    public class SingleManager : IDisposable
    {
        private static SingleManager _instance;

        public static SingleManager Instance => _instance ??= new SingleManager();
        
        private SingleManager(){ }
        
        private readonly Stack<ISingleton> _singletons = new();


        public void Dispose()
        {
            _instance = null;

            lock (this)
            {
                while (_singletons.Count > 0)
                {
                    var singleton = _singletons.Pop();
                    singleton.Dispose();
                }
            }
        }
        
        public T AddSingleton<T>() where T : ISingleton, ISingletonAwake
        {
            T singleton = Activator.CreateInstance<T>();
            singleton.Awake();

            AddSingleton(singleton);
            return singleton;
        }
        
        public T AddSingleton<T, A>(A a) where T : ISingleton, ISingletonAwake<A>
        {
            T singleton = Activator.CreateInstance<T>();
            singleton.Awake(a);

            AddSingleton(singleton);
            return singleton;
        }
        
        public T AddSingleton<T, A, B>(A a, B b) where T : ISingleton, ISingletonAwake<A, B>
        {
            T singleton = Activator.CreateInstance<T>();
            singleton.Awake(a, b);

            AddSingleton(singleton);
            return singleton;
        }
        
        public T AddSingleton<T, A, B, C>(A a, B b, C c) where T : ISingleton, ISingletonAwake<A, B, C>
        {
            T singleton = Activator.CreateInstance<T>();
            singleton.Awake(a, b, c);

            AddSingleton(singleton);
            return singleton;
        }

        public void AddSingleton(ISingleton singleton)
        {
            lock (this)
            {
                _singletons.Push(singleton);
            }
            singleton.Register();
        }
    }
}