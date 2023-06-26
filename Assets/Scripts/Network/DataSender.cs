using Project.Board;
using Project.Infrastructure;
using Project.Input;
using System;
using Unity.Netcode;
using UnityEngine;

public class DataSender : NetworkBehaviour
{
	private PlayerInput _input;
	private GameplayState _gameplay;

	public void Init(GameplayState gameplay, PlayerInput input)
	{
		_input = input;
		_gameplay = gameplay;
	}

	[ServerRpc(RequireOwnership = false)]
	public void StartMoveServerRpc(int firstValue, int secondValue)
	{
		_gameplay.SynchronisePlayerMove(firstValue, secondValue);
	}

	[ClientRpc]
	public void StartMoveClientRpc(int firstValue, int secondValue)
	{
		if (IsHost) return;

		_gameplay.SynchronisePlayerMove(firstValue, secondValue);
	}

	[ClientRpc]
	public void SendStartDraggingClientRpc(Vector3 mousePosition)
	{
		if (IsHost) return;
		_input.RepeatInputStart(mousePosition);
	}

	[ClientRpc]
	public void SendOnDragClientRpc(Vector3 mousePosition)
	{
		if (IsHost) return;
		_input.RepeatInputDrag(mousePosition);
	}

	[ClientRpc]
	public void SendStopDraggingClientRpc(Vector3 mousePosition)
	{
		if (IsHost) return;
		_input.RepeatInputStop(mousePosition);
	}

	[ServerRpc(RequireOwnership = false)]
	public void SendStartDraggingServerRpc(Vector3 mousePosition)
	{
		_input.RepeatInputStart(mousePosition);
	}

	[ServerRpc(RequireOwnership = false)]
	public void SendOnDragServerRpc(Vector3 mousePosition)
	{
		_input.RepeatInputDrag(mousePosition);
	}

	[ServerRpc(RequireOwnership = false)]
	public void SendStopDraggingServerRpc(Vector3 mousePosition)
	{
		_input.RepeatInputStop(mousePosition);
	}
}
