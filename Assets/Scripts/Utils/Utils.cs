using System.Collections.Generic;
using System.Linq;
using Plugins.Honeti.I18N.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public static class Utils
    {
        public static bool IsPointerOverUIObject() {
            PointerEventData eventDataCurrentPosition = new(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
        {
            IEnumerable<T> enumerable1 = enumerable as T[] ?? enumerable.ToArray();
            return enumerable1.ElementAt(Random.Range(0, enumerable1.Count()));
        }
        
        public static string GetTranslation(this string key, params object[] parameters)
        {
            if (!key.StartsWith("^"))
            {
                key = "^" + key;
            }
            
            return I18N.instance.GetValue(key, parameters);
        }
    }
}