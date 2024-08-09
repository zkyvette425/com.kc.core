using System;
using System.Collections.Generic;

namespace KC
{
    public class Root : Component,IRoot
    {
        private readonly Dictionary<Type, Queue<ComponentRef<Component>>> _dictionary = new();
        private readonly Queue<ComponentRef<Component>> _loops = new();
        
        internal Root(){}
        
        internal Root(Component component){}
        
        public Root ParentRoot { get; set; }
        
        public int RootType { get; set; }

        internal void Register(Component component)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (component is IUpdate a || component is ILateUpdate b)
            {
                _loops.Enqueue(component);
            }
        }

        public void Update()
        {
            
        }
    }
}