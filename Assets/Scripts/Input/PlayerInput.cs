using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

namespace Project.Input
{
	public class PlayerInput : NetworkBehaviour
	{
		public Camera CurrentActiveCamera { get; set; }

		public ulong CurrentPlayerID { get; set; }

		private Coroutine _inputHandleProcess;
		private DataSender _sender;

		private IInputReceiver[] _inputReceivers;
		private int _maxDragsAmount;
		private int _dragsAmount;

		private bool _controlLock;

		public void Construct(DataSender sender)
		{
			_sender = sender;
		}

		public void Init(int maxDragsAmount = 0, params IInputReceiver[] inputReceivers)
		{
			_inputReceivers = inputReceivers;
			_maxDragsAmount = maxDragsAmount;

			_controlLock = false;
			_dragsAmount = 0;

			if (_inputHandleProcess != null)
				StopCoroutine(_inputHandleProcess);
			_inputHandleProcess = StartCoroutine(HandleInput());
		}

		public void UnlockControl()
		{
			_controlLock = false;
		}

		public void LockControl()
		{
			_controlLock = true;
		}

		private IEnumerator HandleInput()
		{
			yield return new WaitForSeconds(0.1f);

			while (true)
			{
				if (CurrentPlayerID == 0)
				{
					if (IsHost)
					{
						Handle();
					}
				}

				else
				{
					if (!IsHost)
					{
						Handle();
					}
				}


				yield return null;
			}
		}

		private void Handle()
		{
			if (_maxDragsAmount > 0)
			{
				if (_dragsAmount < 1)
					SendInputToReceivers();
			}
			else
				SendInputToReceivers();
		}

		private void SendInputToReceivers()
		{
			if (UnityEngine.Input.GetMouseButtonDown(0) && !_controlLock)
			{
				foreach (var receiver in _inputReceivers)
				{
					receiver.StartDragging(UnityEngine.Input.mousePosition, CurrentActiveCamera);

					if(_maxDragsAmount < 1)
					{
						if (IsHost)
						{
							_sender.SendStartDraggingClientRpc(UnityEngine.Input.mousePosition);
						}
						else
						{
							_sender.SendStartDraggingServerRpc(UnityEngine.Input.mousePosition);
						}
					}
				}
			}


			else if (UnityEngine.Input.GetMouseButton(0))
			{
				foreach (var receiver in _inputReceivers)
				{
					receiver.OnDrag(UnityEngine.Input.mousePosition, CurrentActiveCamera);

					if (_maxDragsAmount < 1)
					{
						if (IsHost)
							_sender.SendOnDragClientRpc(UnityEngine.Input.mousePosition);
						else
						{
							_sender.SendOnDragServerRpc(UnityEngine.Input.mousePosition);
						}
					}
				}
			}

			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				_dragsAmount++;

				foreach (var receiver in _inputReceivers)
				{
					if (_maxDragsAmount < 1)
					{
						if (IsHost)
						{
							_sender.SendStopDraggingClientRpc(UnityEngine.Input.mousePosition);
						}
						else
						{
							_sender.SendStopDraggingServerRpc(UnityEngine.Input.mousePosition);
						}
					}

					receiver.StopDragging(UnityEngine.Input.mousePosition, CurrentActiveCamera);
				}
			}
		}

		public void RepeatInputStart(Vector3 mousePosition)
		{
			foreach (var receiver in _inputReceivers)
			{
				receiver.StartDragging(mousePosition, CurrentActiveCamera);
			}
		}

		public void RepeatInputDrag(Vector3 mousePosition)
		{
			foreach (var receiver in _inputReceivers)
			{
				receiver.OnDrag(mousePosition, CurrentActiveCamera);
			}
		}

		public void RepeatInputStop(Vector3 mousePosition)
		{
			foreach (var receiver in _inputReceivers)
			{
				receiver.StopDragging(mousePosition, CurrentActiveCamera);
			}
		}
	}
}

