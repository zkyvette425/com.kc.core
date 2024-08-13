using System.Collections.Generic;
// ReSharper disable SuspiciousTypeConversion.Global

namespace KC
{
    public class Root : Component,IAwake<int>,IRoot
    {
        private readonly Queue<ComponentRef<Component>> _updateLoops = new();
        private readonly Queue<ComponentRef<Component>> _lateUpdateLoops = new();
        
        internal Root(){}
        
        internal Root(Component component){}
        
        public Root ParentRoot { get; set; }
        
        public int RootType { get;internal set; }
        
        public void Awake(int a)
        {
            RootType = a;
            IRoot = this;
        }

        internal void Register(Component component)
        {
            if (component is IUpdate)
            {
                _updateLoops.Enqueue(component);
            }
            if (component is ILateUpdate)
            {
                _lateUpdateLoops.Enqueue(component);
            }
        }

        public void Update()
        {
            int count = _updateLoops.Count;
            while (count-- > 0)
            {
                Component component = _updateLoops.Dequeue();
                if (component == null)
                {
                    continue;
                }
                (component as IUpdate)!.Update();
                _updateLoops.Enqueue(component);
            }
        }

        public void LateUpdate()
        {
            int count = _lateUpdateLoops.Count;
            while (count-- > 0)
            {
                Component component = _lateUpdateLoops.Dequeue();
                if (component == null)
                {
                    continue;
                }
                (component as ILateUpdate)!.LateUpdate();
                _lateUpdateLoops.Enqueue(component);
            }
        }


    }
}