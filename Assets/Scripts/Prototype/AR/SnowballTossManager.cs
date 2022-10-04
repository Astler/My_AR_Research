using Niantic.ARDK.Extensions.Meshing;
using UnityEngine;

namespace Prototype.AR
{
    public class SnowballTossManager : MonoBehaviour
    {
        [Header("AR Mesh")]
        [SerializeField] public ARMeshManager arMeshUpdater;

        private void OnEnable()
        {
            if (Application.isEditor)
            {
                arMeshUpdater.UseInvisibleMaterial = false;
            }
        }
    }
}
