using System.Collections.Generic;
using System.Linq;
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
    }
}