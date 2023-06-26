using Project.Board;
using Project.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
	public class PlayerTurn
	{
		public Action<PlayerData> PlayerEndsMove;

		public PlayerData CurrentPlayer { get; private set; }

		private List<Cell> _availableCells;
		private GameBoard _board;
		private PlayerInput _input;

		private List<int> _availableMoves;

		private bool _hasMovedFromHead;

		public PlayerTurn(GameBoard board, PlayerInput input)
		{
			_board = board;
			_input = input;

			_availableCells = new List<Cell>();

			_hasMovedFromHead = false;
		}

		public List<Cell> GetAvailableCellsFor(Checker checker)
		{
			_availableCells = new List<Cell>();
			int firstMove = _availableMoves[0];
			int secondMove = _availableMoves.Count > 1 ? _availableMoves[1] : 0;

			if (checker.Colour == PlayerColour.White)
			{
				CalculateAvaliableCellsForWhite(
					checker, firstMove, secondMove);
			}
			else
			{
				CalculateAvaliableCellsForBlack(
					checker, firstMove, secondMove);
			}

			return _availableCells;
		}

		private bool IsCheckerFromHead(Checker checker)
		{
			return checker.Cell.Index == _board.GetHeadIndexForColour(checker.Colour);
		}

		public void SetSettings(PlayerData settings)
		{
			CurrentPlayer = settings;
		}

		public void SetValues(int firstValue, int secondValue)
		{
			_availableMoves = firstValue == secondValue ?
				new List<int>() { firstValue, firstValue, firstValue, firstValue }
				: new List<int>() { firstValue, secondValue };

			_hasMovedFromHead = false;
		}


		private void CalculateAvaliableCellsForWhite(Checker checker, int firstValue, int secondValue)
		{
			int number = checker.Cell.Index;

			AddCellInBorders(checker, number + firstValue, 24);
			AddCellInBorders(checker, secondValue != 0 ? number + secondValue : 0, 24);
		}

		private void CalculateAvaliableCellsForBlack(Checker checker, int firstValue, int secondValue)
		{
			int number = checker.Cell.Index;
			int indexAfterFirstMove = number + firstValue;
			int indexAfterSecondMove = secondValue != 0 ? number + secondValue : 0;

			if (number >= 13)
			{
				if (indexAfterFirstMove > 24) 
					TryAddCellBy(indexAfterFirstMove - 24, checker);
				else 
					TryAddCellBy(indexAfterFirstMove, checker);
				if (indexAfterSecondMove > 24)
					TryAddCellBy(indexAfterSecondMove - 24, checker);
				else
					TryAddCellBy(indexAfterSecondMove, checker);
			}
			else
			{
				AddCellInBorders(checker, indexAfterFirstMove, 12);
				AddCellInBorders(checker, indexAfterSecondMove, 12);
			}
		}
		private void AddCellInBorders(Checker checker, int cellIndex, int limit)
		{
			if (cellIndex <= limit)
				TryAddCellBy(cellIndex, checker);
			else if(AllCheckersAtHome())
				checker.CanGoOut = true;
		}

		private bool AllCheckersAtHome()
		{
			return CurrentPlayer.CheckersAtHome.Count == 15;
		}

		private bool TryAddCellBy(int index, Checker checker)
		{
			if (index < 1) return false;

			var cell = _board.Cells[index - 1];
			if (!cell.CanPlaceCheckerWith(checker.Colour) || 
				_availableCells.Contains(cell) ||
				TryMoveFromHeadTwice(checker))
			{
				return false;
			}

			_availableCells.Add(_board.Cells[index - 1]);

			return true;
		}

		private bool TryMoveFromHeadTwice(Checker checker)
		{
			if (IsCheckerFromHead(checker) && _hasMovedFromHead)
				return true;
			if (CurrentPlayer.FirstTurnMovesAmount < 2 && IsPassingThroughEnemyHeadValues())
				return false;
			return false;
		}

		private bool IsPassingThroughEnemyHeadValues()
		{
			if (_availableMoves.Count > 1 && !(_availableMoves[0] == _availableMoves[1]))
				return false;
			return _availableMoves[0] == 3 || _availableMoves[0] == 4 || _availableMoves[0] == 6;
		}

		public void MakeMove(Checker checker, int previousCellIndex, int newCellIndex, int moveLength)
		{
			_availableMoves.Remove(moveLength);
			CurrentPlayer.FirstTurnMovesAmount++;

			if (previousCellIndex == _board.GetHeadIndexForColour(checker.Colour))
				_hasMovedFromHead = true;

			if (!CurrentPlayer.CheckersAtHome.Contains(checker) && InPlayersHome(newCellIndex))
				CurrentPlayer.CheckersAtHome.Add(checker);

			if (_availableMoves.Count == 0 || !HasAvailableMovesForAnyCheckerWithColour(checker.Colour))
			{
				PlayerEndsMove?.Invoke(CurrentPlayer);
			}
		}

		private bool HasAvailableMovesForAnyCheckerWithColour(PlayerColour colour)
		{
			foreach (var cell in _board.Cells)
			{
				if (cell.Checkers.TryPeek(out Checker checker) && checker.Colour == colour)
				{
					GetAvailableCellsFor(checker);

					if (HasPlaceToGo(checker))
						return true;
				}
			}

			return false;
		}

		private bool InPlayersHome(int index)
		{
			return CurrentPlayer.HomeLimits.start <= index && index <= CurrentPlayer.HomeLimits.end;
		}

		public void UnlockInput()
		{
			_input.UnlockControl();
		}

		public void LockInput()
		{
			_input.LockControl();
		}

		public bool HasPlaceToGo(Checker checker)
		{
			return checker.CanGoOut || _availableCells.Count > 0;
		}
	}
}
