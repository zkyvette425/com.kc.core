using System.Collections.Generic;

namespace KC
{
    public abstract class Component : IReference
    {
        private Dictionary<long, Component> _children;
        private Component _parent;
        protected Component(){}

        internal Dictionary<long, Component> Children => _children;
        
        public long Id { get; internal set; }

        public int ChildCount => _children?.Count ?? 0;

        public Component Parent
        {
            get => _parent;
            set
            {
                if (value == null)
                {
                    throw new CoreException($"{GetType().FullName} 设置父组件失败,原因:父组件为空");
                }
                
                if (value == this)
                {
                    throw new CoreException($"{GetType().FullName} 设置父组件失败,原因:无法将自己设置为父组件");
                }

                if (_parent != null)
                {
                    if (_parent == value)
                    {
                        throw new CoreException($"{GetType().FullName} 设置父组件失败,原因:重复设置");
                    }
                    _parent._children.Remove(Id);
                }

                _parent = value;
                _parent._children ??= new Dictionary<long, Component>();
                _parent._children.Add(Id, this);
            }
        }
        
        public TComponent GetComponent<TComponent>(long id) where TComponent : Component
        {
            if (_children == null)
            {
                return null;
            }

            if (_children.TryGetValue(id,out var component))
            {
                return (TComponent)component;
            }

            return null;
        }
        
        public void RemoveComponent(long id)
        {
            if (_children == null)
            {
                return;
            }

            if (!_children.Remove(id,out var component))
            {
                return;
            }

            ReferencePool.Release(component);
        }
        
        /// <summary>
        /// 清理引用。
        /// </summary>
        public virtual void Clear()
        {
            Id = 0;
        }
    }
}