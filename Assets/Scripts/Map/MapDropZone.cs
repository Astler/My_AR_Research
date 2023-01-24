using UnityEngine;

namespace Map
{
    public class MapDropZone : ClickableBehaviour
    {
        [SerializeField] private GameObject selectedAnimationGameObject;
        
        public void SetIsSelected(bool isSelected)
        {
            selectedAnimationGameObject.SetActive(isSelected);
        }
    }
}