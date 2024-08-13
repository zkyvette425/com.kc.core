
using System;
using KC;
using UnityEditor;
using Component = KC.Component;

namespace ET
{
    [TypeDrawer]
    public class ISceneTypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof (IRoot);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            Component iScene = (Component)value;
            EditorGUILayout.ObjectField(memberName, iScene.GameObject, memberType, true);
            return value;
        }
    }
}
