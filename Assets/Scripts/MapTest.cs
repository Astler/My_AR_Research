using Prototype.World;
using UnityEngine;
using UnityEngine.UI;

public class MapTest : MonoBehaviour
{
    [SerializeField] private LocationController locationController;
    [SerializeField] private Button toMeButton;
    [SerializeField] private Button toNearestPortal;

    private void Awake()
    {
        toMeButton.onClick.AddListener(OnToMeClicked);
        toNearestPortal.onClick.AddListener(OnNearestClicked);
    }
    
    private void OnNearestClicked()
    {
        OnlineMaps.instance.position = locationController.SelectedPortalZone.Value.GetPosition();
    }

    private void OnToMeClicked()
    {
        OnlineMaps.instance.position = LocationController.GetPlayerPosition();
    }
}