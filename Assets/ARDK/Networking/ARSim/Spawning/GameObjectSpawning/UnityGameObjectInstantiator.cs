// Copyright 2022 Niantic, Inc. All Rights Reserved.

using UnityEngine;

namespace Niantic.ARDK.Networking.ARSim.Spawning.GameObjectSpawning
{
  /// <summary>
  /// Implementation of an IGameObjectInstantiator that uses basic UnityEngine calls.
  /// </summary>
  public sealed class UnityGameObjectInstantiator : 
    IGameObjectInstantiator
  {
    /// <inheritdoc />
    public GameObject Instantiate(GameObject original)
    {
      return Object.Instantiate(original);
    }

    /// <inheritdoc />
    public GameObject Instantiate
    (
      GameObject original,
      Transform parent,
      bool instantiateInWorldSpace = false
    )
    {
      return Object.Instantiate(original, parent, instantiateInWorldSpace);
    }

    /// <inheritdoc />
    public GameObject Instantiate
    (
      GameObject original,
      Vector3 position,
      Quaternion rotation,
      Transform parent = null
    )
    {
      return parent != null ? 
        Object.Instantiate(original, position, rotation, parent) : 
        Object.Instantiate(original, position, rotation);
    }

    /// <inheritdoc />
    public void Destroy(GameObject obj, float timeToDelay = 0.0f)
    {
      Object.Destroy(obj, timeToDelay);
    }
  }
}
