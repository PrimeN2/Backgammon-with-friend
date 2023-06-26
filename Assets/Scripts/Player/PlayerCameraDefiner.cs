using System;
using Unity.Netcode;
using UnityEngine;

namespace Project.Player
{
	public class PlayerCameraDefiner : NetworkBehaviour
	{
		public Camera WhitePlayerCamera;
		public Camera BlackPlayerCamera;

		public void SetCameras()
		{
			WhitePlayerCamera.gameObject.SetActive(true);
			BlackPlayerCamera.gameObject.SetActive(true);

			if (IsClient)
			{
				WhitePlayerCamera.depth = -2;
			}

			if (IsHost)
			{
				WhitePlayerCamera.depth = 0;
			}
		}
	}
}
