using UnityEngine;
using Vuforia;
using Vuforia.UnityRuntimeCompiled;

namespace SamplesResources.SceneAssets.GroundPlane.Scripts
{
    public class ProductPlacement : MonoBehaviour
    {
        [SerializeField] private Mesh boxMesh;
        public bool GroundPlaneHitReceived { get; private set; }

        private Vector3 ProductScale
        {
            get
            {
                var augmentationScale = VuforiaRuntimeUtilities.IsPlayMode() ? 0.1f : ProductSize;
                return new Vector3(augmentationScale, augmentationScale, augmentationScale);
            }
        }

        [Header("Augmentation Object")]
        [SerializeField] GameObject Chair = null;
        [SerializeField] GameObject ChairShadow = null;

        [Header("Control Indicators")]
        [SerializeField] GameObject TranslationIndicator = null;
        [SerializeField] GameObject RotationIndicator = null;

        [Header("Augmentation Size")]
        [Range(0.1f, 2.0f)]
        [SerializeField] float ProductSize = 0.65f;

        const string RESOURCES_CHAIR_BODY = "ChairBody";
        const string RESOURCES_CHAIR_FRAME = "ChairFrame";
        const string RESOURCES_CHAIR_SHADOW = "ChairShadow";
        const string RESOURCES_CHAIR_BODY_TRANSPARENT = "ChairBodyTransparent";
        const string RESOURCES_CHAIR_FRAME_TRANSPARENT = "ChairFrameTransparent";
        const string RESOURCES_CHAIR_SHADOW_TRANSPARENT = "ChairShadowTransparent";
        const string GROUND_PLANE_NAME = "Emulator Ground Plane";
        const string FLOOR_NAME = "Floor";

        MeshRenderer mChairRenderer;
        MeshRenderer mChairShadowRenderer;
        Material[] mChairMaterials, mChairMaterialsTransparent;
        Material mChairShadowMaterial, mChairShadowMaterialTransparent;
        Camera mMainCamera;
        string mFloorName;
        Vector3 mOriginalChairScale;
        bool mIsPlaced;
        int mAutomaticHitTestFrameCount;

        void Start()
        {
            mMainCamera = VuforiaBehaviour.Instance.GetComponent<Camera>();
            mChairRenderer = Chair.GetComponent<MeshRenderer>();
            mChairShadowRenderer = ChairShadow.GetComponent<MeshRenderer>();

            SetupFloor();
        
            mOriginalChairScale = Chair.transform.localScale;
            Reset();
        }

        void Update()
        {
            if (!mIsPlaced)
                RotateTowardsCamera(Chair);
        }

        void LateUpdate()
        {
            GroundPlaneHitReceived = mAutomaticHitTestFrameCount == Time.frameCount;

            if (!mIsPlaced)
            {
                var isVisible = VuforiaBehaviour.Instance.DevicePoseBehaviour.TargetStatus.IsTrackedOrLimited() && GroundPlaneHitReceived;
                mChairRenderer.enabled = mChairShadowRenderer.enabled = isVisible;
            }
        }

        void SnapProductToMousePosition()
        {
            if (TouchHandler.sIsSingleFingerDragging || VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0))
            {
                if (!UnityRuntimeCompiledFacade.Instance.IsUnityUICurrentlySelected())
                {
                    var cameraToPlaneRay = mMainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(cameraToPlaneRay, out var cameraToPlaneHit) &&
                        cameraToPlaneHit.collider.gameObject.name == mFloorName)
                        Chair.transform.position = cameraToPlaneHit.point;
                }
            }
        }
        
        public void Reset()
        {
            Chair.transform.localPosition = Vector3.zero;
            Chair.transform.localEulerAngles = Vector3.zero;
            Chair.transform.localScale = Vector3.Scale(mOriginalChairScale, ProductScale);

            mIsPlaced = false;
        }
        
        public void OnContentPlaced()
        {
            Debug.Log("OnContentPlaced() called.");

            Chair.transform.localPosition = Vector3.zero;
            RotateTowardsCamera(Chair);

            mIsPlaced = true;
        }
        
        public void OnAutomaticHitTest(HitTestResult result)
        {
            mAutomaticHitTestFrameCount = Time.frameCount;

            if (!mIsPlaced)
            {
                Chair.transform.position = result.Position;
            }
        }
        
        void SetupFloor()
        {
            if (VuforiaRuntimeUtilities.IsPlayMode())
                mFloorName = GROUND_PLANE_NAME;
            else
            {
                mFloorName = FLOOR_NAME;
                var floor = new GameObject(mFloorName, typeof(BoxCollider));
                floor.transform.SetParent(Chair.transform.parent);
                floor.AddComponent<MeshRenderer>();
                floor.AddComponent<MeshFilter>().mesh = boxMesh;
                floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                floor.transform.localScale = new Vector3(100f, 0, 100f);
                floor.GetComponent<BoxCollider>();
            }
        }
        
        void RotateTowardsCamera(GameObject augmentation)
        {
            var lookAtPosition =  mMainCamera.transform.position - augmentation.transform.position;
            lookAtPosition.y = 0;
            var rotation = Quaternion.LookRotation(lookAtPosition);
            augmentation.transform.rotation = rotation;
        }
    }
}
