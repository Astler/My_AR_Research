using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Builder : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;

    [SerializeField] private GameObject[] boxes;
    [SerializeField] private LayerMask mask;

    [SerializeField] private Button buildButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button[] boxMenuButtons;

    private WorldBoxView _selectedWorldBlock;
    private int _selectedBox;

    private void Awake()
    {
        buildButton.onClick.AddListener(OnBuild);
        deleteButton.onClick.AddListener(OnDestroy);

        int i = 0;

        foreach (Button boxMenuButton in boxMenuButtons)
        {
            int localIndex = i;

            boxMenuButton.onClick.AddListener(delegate { _selectedBox = localIndex; });

            i += 1;
        }
    }

    private void OnDestroy()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(.5f, .5f));

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, mask))
        {
            Destroy(hit.collider.gameObject);
        }
    }

    private void OnBuild()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector2(.5f, .5f));

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, mask))
        {
            BuildBlock(hit.transform.position + hit.normal, hit.transform.rotation);
        }
        else
        {
            List<ARRaycastHit> arHitResults = new();
            raycastManager.Raycast(ray, arHitResults, TrackableType.Planes);

            if (arHitResults.Count > 0)
            {
                Vector3 hitPose = arHitResults[0].pose.position;
                Quaternion hitRotation = arHitResults[0].pose.rotation;
                Vector3 targetPosition =
                    new Vector3(Mathf.Round(hitPose.x), Mathf.Round(hitPose.y), Mathf.Round(hitPose.z));

                BuildBlock(targetPosition, hitRotation);
            }
        }
    }

    private void BuildBlock(Vector3 position, Quaternion rotation)
    {
        Instantiate(boxes[_selectedBox], position, rotation);
    }

    private void Update()
    {
        if (_selectedWorldBlock)
        {
            _selectedWorldBlock.SetIsSelected(false);
        }

        var ray = Camera.main.ViewportPointToRay(new Vector2(.5f, .5f));

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, mask))
        {
            var worldBlock = hit.collider.gameObject.GetComponent<WorldBoxView>();

            if (worldBlock)
            {
                _selectedWorldBlock = worldBlock;
                worldBlock.SetIsSelected(true);
            }
        }
    }
}