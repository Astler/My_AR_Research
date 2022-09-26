using SamplesResources.SceneAssets.GroundPlane.Scripts;
using UnityEngine;
using Vuforia;
using Vuforia.UnityRuntimeCompiled;

namespace Prototype.AR.VuforiaAR
{
    public class VuforiaTargetSelectView : MonoBehaviour
    {
        [SerializeField] private Transform targetPlacement;
        [SerializeField] private CameraView cameraView;
        //
        // public bool GroundPlaneHitReceived { get; private set; }
        //
        // const string GROUND_PLANE_NAME = "Emulator Ground Plane";
        // const string FLOOR_NAME = "Floor";
        //
        // string mFloorName;
        // int mAutomaticHitTestFrameCount;
        //
        // void Start()
        // {
        //     SetupFloor();
        //     Reset();
        // }
        //
        // void Update()
        // {
        //     RotateTowardsCamera(targetPlacement);
        // }
        //
        // void LateUpdate()
        // {
        //     GroundPlaneHitReceived = mAutomaticHitTestFrameCount == Time.frameCount;
        //
        //     if (!mIsPlaced)
        //     {
        //         var isVisible = VuforiaBehaviour.Instance.DevicePoseBehaviour.TargetStatus.IsTrackedOrLimited() &&
        //                         GroundPlaneHitReceived;
        //         mChairRenderer.enabled = mChairShadowRenderer.enabled = isVisible;
        //     }
        // }
        //
        // void SnapProductToMousePosition()
        // {
        //     if (TouchHandler.sIsSingleFingerDragging || VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0))
        //     {
        //         if (!UnityRuntimeCompiledFacade.Instance.IsUnityUICurrentlySelected())
        //         {
        //             var cameraToPlaneRay = mMainCamera.ScreenPointToRay(Input.mousePosition);
        //
        //             if (Physics.Raycast(cameraToPlaneRay, out var cameraToPlaneHit) &&
        //                 cameraToPlaneHit.collider.gameObject.name == mFloorName)
        //                 Chair.transform.position = cameraToPlaneHit.point;
        //         }
        //     }
        // }
        //
        // public void OnContentPlaced()
        // {
        //     Debug.Log("OnContentPlaced() called.");
        //
        //     Chair.transform.localPosition = Vector3.zero;
        //     RotateTowardsCamera(Chair);
        //
        //     mIsPlaced = true;
        // }
        //
        // public void OnAutomaticHitTest(HitTestResult result)
        // {
        //     mAutomaticHitTestFrameCount = Time.frameCount;
        //
        //     if (!mIsPlaced)
        //     {
        //         Chair.transform.position = result.Position;
        //     }
        // }
        //
        // void SetupFloor()
        // {
        //     if (VuforiaRuntimeUtilities.IsPlayMode())
        //         mFloorName = GROUND_PLANE_NAME;
        //     else
        //     {
        //         mFloorName = FLOOR_NAME;
        //         var floor = new GameObject(mFloorName, typeof(BoxCollider));
        //         floor.transform.SetParent(transform.parent);
        //         floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        //         floor.transform.localScale = Vector3.one;
        //         floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
        //     }
        // }
        //
        // void RotateTowardsCamera(GameObject augmentation)
        // {
        //     var lookAtPosition = cameraView.GetTransform().position - augmentation.transform.position;
        //     lookAtPosition.y = 0;
        //     var rotation = Quaternion.LookRotation(lookAtPosition);
        //     augmentation.transform.rotation = rotation;
        // }
    }
}