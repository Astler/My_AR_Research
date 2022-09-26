using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Outline))]
public class WorldBoxView : MonoBehaviour
{
    private Outline _outline;

    public void SetIsSelected(bool isSelected)
    {
        _outline.enabled = isSelected;
    }

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }
}