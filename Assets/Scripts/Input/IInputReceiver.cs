using UnityEngine;

namespace Project.Input
{
	public interface IInputReceiver
	{
		void StartDragging(Vector3 mousePosition, Camera activeCamera);
		void StopDragging(Vector3 mousePosition, Camera activeCamera);
		void OnDrag(Vector3 mousePosition, Camera activeCamera);
	}
}
