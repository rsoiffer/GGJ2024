using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Helpers
{
    public static class DebugHelpers
    {
        [PublicAPI]
        public static string FullName(this GameObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var fullName = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                fullName = obj.name + "/" + fullName;
            }

            return fullName;
        }

        [PublicAPI]
        public static string FullName<T>(this T component) where T : Component
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            return component.gameObject.FullName() + "." + typeof(T).Name;
        }
    }
}