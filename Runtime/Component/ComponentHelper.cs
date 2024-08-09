
using System;
using System.Collections.Generic;
using System.Linq;

namespace KC
{
    public static class ComponentHelper
    {
        public static TComponent AddComponent<TComponent>(this Component self) where TComponent : Component,IAwake,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self);
            child.Awake();
            return child;
        }
        
        public static TComponent AddComponent<TComponent,A>(this Component self,A a) where TComponent : Component,IAwake<A>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self);
            child.Awake(a);
            return child;
        }
        
        public static TComponent AddComponent<TComponent,A,B>(this Component self,A a,B b) where TComponent : Component,IAwake<A,B>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self);
            child.Awake(a,b);
            return child;
        }
        
        public static TComponent AddComponent<TComponent,A,B,C>(this Component self,A a,B b,C c) where TComponent : Component,IAwake<A,B,C>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self);
            child.Awake(a,b,c);
            return child;
        }
        
        public static TComponent AddComponent<TComponent,A,B,C,D>(this Component self,A a,B b,C c,D d) where TComponent : Component,IAwake<A,B,C,D>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self);
            child.Awake(a,b,c,d);
            return child;
        }
        
        public static TComponent AddComponentWithId<TComponent>(this Component self,long id) where TComponent : Component,IAwake,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self,id);
            child.Awake();
            return child;
        }
        
        public static TComponent AddComponentWithId<TComponent,A>(this Component self,long id,A a) where TComponent : Component,IAwake<A>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self,id);
            child.Awake(a);
            return child;
        }
        
        public static TComponent AddComponentWithId<TComponent,A,B>(this Component self,long id,A a,B b) where TComponent : Component,IAwake<A,B>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self,id);
            child.Awake(a,b);
            return child;
        }
        
        public static TComponent AddComponentWithId<TComponent,A,B,C>(this Component self,long id,A a,B b,C c) where TComponent : Component,IAwake<A,B,C>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self,id);
            child.Awake(a,b,c);
            return child;
        }
        
        public static TComponent AddComponentWithId<TComponent,A,B,C,D>(this Component self,long id,A a,B b,C c,D d) where TComponent : Component,IAwake<A,B,C,D>,new()
        {
            TComponent child = ReferencePool.Acquire<TComponent>();
            child.InitComponentInternal(self,id);
            child.Awake(a,b,c,d);
            return child;
        }
        
        public static List<TComponent> GetComponents<TComponent>(this Component self) where TComponent : Component
        {
            if (self.ChildCount == 0)
            {
                return null;
            }

            return self.Children.Values.OfType<TComponent>().ToList();   //这个函数的功能等于先Where再Select,但是速度和gc和API都要优秀特别多
        }

        public static List<Component> GetComponents(this Component self, Type componentType)
        {
            if (self.ChildCount == 0)
            {
                return null;
            }

            return self.Children.Values.Where(p => p.GetType() == componentType).ToList();
        }
        
        public static TComponent GetComponent<TComponent>(this Component self) where TComponent : Component
        {
            if (self.ChildCount == 0)
            {
                return null;
            }

            //return (TComponent)self.Children.Values.FirstOrDefault(p => p is TComponent);  //不知道为什么,这样会比较慢..
            return (TComponent)self.GetComponent(typeof(TComponent));                        //这个挺快的
        }
        
        public static Component GetComponent(this Component self,Type componentType)
        {
            if (self.ChildCount == 0)
            {
                return null;
            }

            return self.Children.Values.FirstOrDefault(p => p.GetType() == componentType);
        }
        
        public static void RemoveComponent(this Component self,Component component)
        {
            if (self.ChildCount == 0)
            {
                return;
            }
            
            self.RemoveComponent(component.Id);
        }
        
        public static void RemoveComponent<TComponent>(this Component self) where TComponent : Component
        {
            if (self.ChildCount == 0)
            {
                return;
            }

            var waitDeletes = self.Children.Values.OfType<TComponent>().ToArray();

            for (int i = waitDeletes.Length - 1; i >= 0; i--)
            {
                var item = waitDeletes[i];
                if (item != null)
                {
                    self.RemoveComponent(item);
                }
            }
        }
        
        public static void RemoveComponent(this Component self,Type componentType)
        {
            if (self.ChildCount == 0)
            {
                return;
            }

            var waitDeletes = self.Children.Values.Where(p => p.GetType() == componentType).ToArray();

            for (int i = waitDeletes.Length - 1; i >= 0; i--)
            {
                var item = waitDeletes[i];
                if (item != null)
                {
                    self.RemoveComponent(item);
                }
            }
        }
        
        private static void InitComponentInternal(this Component self,Component parent,long id = 0)
        {
            self.Id = id == 0 ? IdGenerator.Instance.GenerateId() : id;
            self.Parent = parent;
        }
    }
}