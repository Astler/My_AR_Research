using UnityEngine;

public class MapObjectSizeControlView : MonoBehaviour
{
    private void Update()
    {
        transform.localScale = new Vector3(1f, 0f, 1f) * transform.localScale.x + new Vector3(0f, 15f, 0f);
    }
}
