
using System;
using System.Reflection;
using KC;
using UnityEditor;
using UnityEngine;
using Component = KC.Component;

namespace ET
{
    [TypeDrawer]
    public class EntityRefTypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            if (type.GetGenericTypeDefinition() == typeof (ComponentRef<>))
            {
                return true;
            }

            return false;
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            FieldInfo fieldInfo = memberType.GetField("entity", BindingFlags.NonPublic | BindingFlags.Instance);
            Component entity = (Component)fieldInfo.GetValue(value);
            GameObject go = entity?.GameObject;
            EditorGUILayout.ObjectField(memberName, go, memberType, true);
            return value;
        }
    }
}
