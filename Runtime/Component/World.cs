using System.Collections.Generic;

namespace KC
{
    public static class World
    {
        private static List<Root> _zones = new();
        
        public static Root CreateRoot(int sceneType)
        {
            var root = new Root();
            _zones.Add(root);
            return root;
        }
        
        
    }
}