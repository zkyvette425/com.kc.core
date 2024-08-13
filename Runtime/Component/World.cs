using System.Collections.Generic;
using System.Linq;

namespace KC
{
    public static class World
    {
        private static Queue<ComponentRef<Root>> _zoneQueue = new();
        private static List<ComponentRef<Root>> _zones = new();
        
        public static Root CreateRoot(int rootType)
        {
            var root = new Root { Id = IdGenerator.Instance.GenerateId() };
            root.Awake(rootType);
            _zoneQueue.Enqueue(root);
            _zones.Add(root);
            return root;
        }

        // public static Root GetRoot(long id)
        // {
        //     return _zones.FirstOrDefault(p => p.Id == id);
        // }
        //
        // public static Root[] GetRoots(int rootType)
        // {
        //     return _zones.Where(p => p.RootType == rootType).ToArray();
        // }

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