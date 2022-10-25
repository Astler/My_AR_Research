using UnityEngine;
using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityARInterface
{
	public class ARPlaneHandler : MonoBehaviour
	{
		public static Action resetARPlane;
		public static Action<BoundedPlane> returnARPlane;
		private TrackableId? _planeId = null;
		private BoundedPlane _cachedARPlane;

		void Start()
		{
			// ARInterface.planeAdded += UpdateARPlane;
			// ARInterface.planeUpdated += UpdateARPlane;
		}

		void UpdateARPlane(BoundedPlane arPlane)
		{

			if (_planeId == null)
			{
				_planeId = arPlane.trackableId;
			}

			if (arPlane.trackableId == _planeId)
			{
				_cachedARPlane = arPlane;
			}

			returnARPlane(_cachedARPlane);
		}
	}
}
