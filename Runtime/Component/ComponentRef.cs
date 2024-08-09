namespace KC
{
    public struct ComponentRef<TComponent> where TComponent : Component
    {
        private TComponent _component;
        private readonly long _id;

        private ComponentRef(TComponent component)
        {
            if (component == null)
            {
                _id = 0;
                _component = null;
                return;
            }
            _id = component.Id;
            _component = component;
        }

        private TComponent Unwrap
        {
            get
            {
                if (_component == null)
                {
                    return null;
                }
                if (_component.Id != _id)
                {
                    _component = null;
                }
                return _component;
            }
        }

        public static implicit operator ComponentRef<TComponent>(TComponent t)
        {
            return new ComponentRef<TComponent>(t);
        }

        public static implicit operator TComponent(ComponentRef<TComponent> v)
        {
            return v.Unwrap;
        }
    }
}