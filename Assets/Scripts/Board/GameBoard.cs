using Project.Player;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Project.Board
{
	public class GameBoard
	{
		public Cell[] Cells { get; private set; }

		private List<Cell> _cellsToHighlight;
		private GameBoardData _data;

		private float _xOffset = 2.74f;
		private float _zOffset = 1.66f;

		private float _cellY = 1.37f;

		public GameBoard(GameBoardData data)
		{
			_data = data;

			GenerateBoardCells();
		}

		public int GetHeadIndexForColour(PlayerColour colour)
		{
			if (colour == PlayerColour.White) return 1;
			else return 13;
		}

		public void HighlightAvailableCells(List<Cell> cellsToHighlight)
		{
			_cellsToHighlight = cellsToHighlight;

			foreach (Cell cell in _cellsToHighlight) 
				cell.HighlightActiveSlot();
		}

		public void TurnOffHighlights()
		{
			foreach (Cell cell in _cellsToHighlight)
				cell.TurnOffActiveSlot();

			_cellsToHighlight = new List<Cell>();
		}

		private void GenerateBoardCells()
		{
			Cells = new Cell[24];

			for (int i = 0; i < 24; i++)
			{
				if (i < 6)
				{
					if (i == 0)
						Cells[i] = new Cell(_data._markerPrefab, _data.transform, PlayerColour.White, 15,
							new Vector3(26.22f - _xOffset * i, _cellY, 17.36f), _zOffset, i + 1, _data._whiteCheckers);
					else
						Cells[i] = new Cell(_data._markerPrefab, _data.transform, PlayerColour.White, 0,
							new Vector3(26.22f - _xOffset * i, _cellY, 17.36f), _zOffset, i + 1);
				}

				else if (i < 12)
					Cells[i] = new Cell(_data._markerPrefab, _data.transform, PlayerColour.White, 0,
						new Vector3(6.62f - _xOffset * (i - 6), _cellY, 17.36f), _zOffset, i + 1);
				else if (i < 18)
				{
					if (i == 12)
					{
						Cells[i] = new Cell(_data._markerPrefab, _data.transform, PlayerColour.Black, 15,
							new Vector3(-7.11f + _xOffset * (i - 12), _cellY, -15.96f), _zOffset, i + 1, _data._blackCheckers);
					}
					else
						Cells[i] = new Cell(_data._markerPrefab, _data.transform, PlayerColour.Black, 0,
							new Vector3(-7.11f + _xOffset * (i - 12), _cellY, -15.96f), _zOffset, i + 1);
				}

				else
					Cells[i] = new Cell(_data._markerPrefab, _data.transform, PlayerColour.Black, 0,
						new Vector3(12.46f + _xOffset * (i - 18), _cellY, -15.96f), _zOffset, i + 1);
			}
		}

		public void HighlightExit(PlayerColour colour)
		{
			if (colour == PlayerColour.White)
				_data._whiteExit.SetActive(true);
			else
				_data._blackExit.SetActive(true);
		}

		public void TurnOffExitHighlight(PlayerColour colour)
		{
			if (colour == PlayerColour.White)
				_data._whiteExit.SetActive(false);
			else
				_data._blackExit.SetActive(false);
		}
	}
}

