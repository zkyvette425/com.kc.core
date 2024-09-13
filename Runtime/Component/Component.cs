using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
#endif


namespace KC
{
    public abstract class Component : IReference
    {
        private Dictionary<long, Component> _children;
        private Component _parent;
        private IRoot _iRoot;

#if UNITY_EDITOR
        public GameObject GameObject;
#endif
        
        protected Component(){}

        protected virtual string GameObjectName => GetType().FullName;

        internal Dictionary<long, Component> Children => _children;
        
        public long Id { get; internal set; }

        public int ChildCount => _children?.Count ?? 0;

        public IRoot IRoot
        {
            get => _iRoot;
            set
            {
                if (value == null)
                {
                    throw new CoreException($"{GetType().FullName} 设置Root错误失败,原因:Root为空");
                }

                if (_iRoot == value)
                {
                    return;
                }

                if (_iRoot != null)
                {
                    _iRoot = value;
                    return;
                }

                _iRoot = value;
                
                ((Root)_iRoot).Register(this);

#if UNITY_EDITOR
                GameObject = new GameObject(GameObjectName);
                GameObject.AddComponent<ComponentView>().Component = this;
                GameObject.transform.SetParent(Parent == null
                    ? GameObject.Find("World/Roots").transform
                    : Parent.GameObject.transform);
#endif
            }
        }

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

                if (value.IRoot == null)
                {
                    throw new CoreException($"{GetType().FullName} 设置父组件失败,原因:父组件:{value.GetType().FullName} Root为空");
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

                if (this is IRoot root)
                {
                    IRoot = root;
                }
                else
                {
                    IRoot = _parent.IRoot;
                }

#if UNITY_EDITOR
                GameObject.GetComponent<ComponentView>().Component = this;
                GameObject.transform.SetParent(Parent == null ? GameObject.Find("World").transform : Parent.GameObject.transform);

                if (_children == null)
                {
                    return;
                }
                
                foreach (var children in Children.Values)
                {
                    children.GameObject.transform.SetParent(GameObject.transform);
                }
#endif
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

            if (_children.Count == 0)
            {
                _children = null;
            }
            
            component.DestroyThis();
            ReferencePool.Release(component);
        }
        

        internal void DestroyThis()
        {
#if UNITY_EDITOR
            Object.Destroy(GameObject);
#endif
            Id = 0;
            
            if (_children != null)
            {
                foreach (var children in _children.Values)
                {
                    children.DestroyThis();
                    ReferencePool.Release(children);
                }
                
                _children = null;
            }

            if (this is IDestroy a)
            {
                a.Destroy();
            }

            _iRoot = null;
            _parent = null;
        }
        
        /// <summary>
        /// 清理引用。
        /// </summary>
        public virtual void OnRecycle()
        {
            Id = 0;
            
#if UNITY_EDITOR
            GameObject = null;
#endif
        }
    }
}