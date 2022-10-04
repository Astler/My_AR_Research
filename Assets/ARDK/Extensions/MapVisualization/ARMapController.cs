// Copyright 2022 Niantic, Inc. All Rights Reserved.

using Niantic.ARDK.AR.SLAM;
using Niantic.ARDK.Utilities;
using UnityEngine;

namespace Niantic.ARDK.Extensions.MapVisualization {
  /// Controller for map visualization prefab for AR localization
  public class ARMapController : MonoBehaviour, IMapVisualizationController {
    private MeshRenderer _meshRenderer;
    private Color _color;
    private bool _visibility = true;

    /// <inheritdoc />
    public void VisualizeMap(IARMap map) {
      if (_meshRenderer == null) {
        _meshRenderer = GetComponent<MeshRenderer>();
        _color = Random.ColorHSV(0.5f, 1f, 0.5f, 1f, 0.5f, 1f, 1f, 1f);
      }

      transform.position = map.Transform.ToPosition();
      transform.rotation = map.Transform.ToRotation();
      transform.localScale = new Vector3(0.05f,0.05f,0.05f);
      _meshRenderer.material.color = _color;
    }

    /// <inheritdoc />
    public void SetVisibility(bool visibility) {
      if (_visibility == visibility) {
        // Visibility did not change, do nothing
        return;
      }

      _visibility = visibility;
      transform.gameObject.SetActive(_visibility);
    }
  }
}