using SamplesResources.SceneAssets.GroundPlane.Scripts;
using UnityEngine;
using Vuforia;
using Vuforia.UnityRuntimeCompiled;

namespace Prototype.AR.VuforiaAR
{
    public class VuforiaARController : MonoBehaviour, IARController
    {
        [SerializeField] private CameraView cameraView;
        public bool GroundPlaneHitReceived { get; private set; }

        const string GROUND_PLANE_NAME = "Emulator Ground Plane";
        const string FLOOR_NAME = "Floor";

        string mFloorName;
        int mAutomaticHitTestFrameCount;
        private Vector3 _lastPointerPosition;

        void Start()
        {
            SetupFloor();
        }

        void Update()
        {
            // if (!mIsPlaced)
            //     RotateTowardsCamera(Chair);
        }

        void LateUpdate()
        {
            GroundPlaneHitReceived = mAutomaticHitTestFrameCount == Time.frameCount;
            var isVisible = VuforiaBehaviour.Instance.DevicePoseBehaviour.TargetStatus.IsTrackedOrLimited() &&
                            GroundPlaneHitReceived;
        }

        void SetupFloor()
        {
            if (VuforiaRuntimeUtilities.IsPlayMode())
                mFloorName = GROUND_PLANE_NAME;
            else
            {
                mFloorName = FLOOR_NAME;
                var floor = new GameObject(mFloorName, typeof(BoxCollider));
                floor.transform.SetParent(transform);
                floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                floor.transform.localScale = Vector3.one;
                floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
            }
        }

        // void RotateTowardsCamera(GameObject augmentation)
        // {
        //     var lookAtPosition = mMainCamera.transform.position - augmentation.transform.position;
        //     lookAtPosition.y = 0;
        //     var rotation = Quaternion.LookRotation(lookAtPosition);
        //     augmentation.transform.rotation = rotation;
        // }

        public (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition)
        {
            if (TouchHandler.sIsSingleFingerDragging || VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0))
            {
                if (!UnityRuntimeCompiledFacade.Instance.IsUnityUICurrentlySelected())
                {
                    RaycastHit hit = cameraView.FirstHitByMousePosition(Input.mousePosition);

                    if (hit.collider.gameObject.name == mFloorName)
                        return (true, new Pose(hit.point, Quaternion.identity));
                }
            }

            return (false, null);
        }

        public void SetLastPointerPosition(HitTestResult result)
        {
            _lastPointerPosition = result.Position;
        }

        public CameraView GetCamera() => cameraView;
        
        public Vector3 GetPointerPosition() => _lastPointerPosition;

        public void Reset()
        {
        }
    }
}