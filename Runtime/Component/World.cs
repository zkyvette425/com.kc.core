using System.Collections.Generic;

namespace KC
{
    public static class World
    {
        private static Queue<ComponentRef<Root>> _zoneQueue = new();
        
        public static Root CreateRoot(int rootType)
        {
            var root = new Root { Id = IdGenerator.Instance.GenerateId() };
            root.Awake(rootType);
            _zoneQueue.Enqueue(root);
            return root;
        }
        
        public static void Update()
        {
            int count = _zoneQueue.Count;
            while (count-- > 0)
            {
                Root root = _zoneQueue.Dequeue();
                if (root == null)
                {
                    continue;
                }
                root.Update();
                _zoneQueue.Enqueue(root);
            }
        }
        
        public static void LateUpdate()
        {
            int count = _zoneQueue.Count;
            while (count-- > 0)
            {
                Root root = _zoneQueue.Dequeue();
                if (root == null)
                {
                    continue;
                }
                root.LateUpdate();
                _zoneQueue.Enqueue(root);
            }
        }
    }
}