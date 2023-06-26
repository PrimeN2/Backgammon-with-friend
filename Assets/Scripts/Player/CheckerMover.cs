using Porject.Board;
using Project.Board;
using Project.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
	public class CheckerMover : MonoBehaviour, IInputReceiver
	{
		public Action<PlayerData> OnCheckerLeftTheGame;

		[SerializeField] private AudioClip _moveSound;
		[SerializeField] private float _followSpeed = 15;

		private List<Cell> _availableCells;

		private Camera _camera;
		private PlayerTurn _turn;
		private Checker _selectedChecker;
		private GameBoard _gameBoard;

		public void Initialize(GameBoard gameBoard, PlayerTurn turn)
		{
			_gameBoard = gameBoard;
			_turn = turn;

			_selectedChecker = null;
		}

		public void StartDragging(Vector3 mousePosition, Camera camera)
		{
			_camera = camera;

			if (Physics.Raycast(_camera.ScreenPointToRay(mousePosition),
				out RaycastHit hit, 10, LayerMask.GetMask("Checkers")))
			{
				if (hit.collider.TryGetComponent(out _selectedChecker))
				{
					_availableCells = _turn.GetAvailableCellsFor(_selectedChecker);
					if (RightColour() && 
						_turn.HasPlaceToGo(_selectedChecker) && 
						_selectedChecker.IsOnTop())
					{
						_selectedChecker.Cell.Checkers.Pop();
						_selectedChecker.transform.localScale *= 1.2f;

						_turn.LockInput();

						HighlightPoints();
					}
					else
					{
						_selectedChecker = null;
					}
				}
			}
		}

		private void HighlightPoints()
		{
			if (_selectedChecker.CanGoOut)
				_gameBoard.HighlightExit(_selectedChecker.Colour);
			_gameBoard.HighlightAvailableCells(_availableCells);
		}

		private bool RightColour() => _selectedChecker.Colour == _turn.CurrentPlayer.Colour;

		public void OnDrag(Vector3 mousePosition, Camera camera)
		{
			_camera = camera;

			if (!_selectedChecker) return;

			if (Physics.Raycast(_camera.ScreenPointToRay(mousePosition), 
				out RaycastHit hit, 10, LayerMask.GetMask("Board")))
			{
				if (hit.collider.TryGetComponent<BoardPlane>(out _))
				{
					_selectedChecker.transform.position =
						Vector3.Lerp(_selectedChecker.transform.position,
						new Vector3(hit.point.x, -1.75f, hit.point.z), Time.deltaTime * _followSpeed);
				}
			}
		}

		private bool EnoughDistanceToCell(Cell cell)
		{
			return Vector3.Distance(_selectedChecker.OnBoardLocalPosition, cell.AvailableLocalSlot) < 4f;
		}

		public void StopDragging(Vector3 mousePosition, Camera camera)
		{
			_camera = camera;
			if (!_selectedChecker) return;

			Cell startingCell = _selectedChecker.Cell;

			foreach (var cell in _availableCells)
			{
				if (EnoughDistanceToCell(cell))
				{
					StartCoroutine(MoveCheckerTo(cell));
					_selectedChecker.MoveTo(cell);
					_turn.MakeMove(
						_selectedChecker, startingCell.Index, cell.Index, 
						cell.Index - startingCell.Index);
					AudioManager.Instance.MakeSound(_moveSound);

					SetupForNextMove();

					return;
				}
			}

			if (_selectedChecker.CanGoOut &&
				Physics.Raycast(_camera.ScreenPointToRay(mousePosition),
				out RaycastHit hit, 10, LayerMask.GetMask("Finish")))
			{
				_selectedChecker.gameObject.SetActive(false);

				OnCheckerLeftTheGame?.Invoke(_turn.CurrentPlayer);

				SetupForNextMove();

				return;
			}

			StartCoroutine(MoveCheckerTo(startingCell));
			_selectedChecker.MoveTo(startingCell);

			SetupForNextMove();
		}

		private void SetupForNextMove()
		{
			_selectedChecker.transform.localScale /= 1.2f;

			_gameBoard.TurnOffExitHighlight(_selectedChecker.Colour);
			_gameBoard.TurnOffHighlights();
			_turn.UnlockInput();

			_selectedChecker = null;
		}

		private IEnumerator MoveCheckerTo(Cell cell)
		{
			Transform checkerTransform = _selectedChecker.transform;
			Vector3 targetPosition = cell.AvailableLocalSlot;

			_turn.LockInput();

			while (checkerTransform.position != targetPosition)
			{
				checkerTransform.localPosition = 
					Vector3.MoveTowards(
						checkerTransform.localPosition, targetPosition, _followSpeed * Time.deltaTime);
				if (Vector3.Distance(targetPosition, checkerTransform.localPosition) < 0.01f)
					yield break;	
				yield return null;
			}
			_turn.UnlockInput();
		}
	}
}
