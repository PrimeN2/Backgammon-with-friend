using Project.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Board
{
	public class Cell
	{
		public readonly int Index;
		public Vector3 AvailableLocalSlot => 
			_startPosition - (int)_defaultColour * Checkers.Count * _checkerZOffset;
		public bool IsHead(PlayerColour colour) => Index == 1 && colour == PlayerColour.White ||
			Index == 13 && colour == PlayerColour.Black;
			
		public Stack<Checker> Checkers { get; private set; }

		private readonly PlayerColour _defaultColour;

		private Vector3 _startPosition;

		private Vector3 _checkerZOffset;

		private GameObject _marker;

		public Cell(GameObject markerPrefab, Transform parent, PlayerColour defaultCheckerColour,
			int checkerAmount, Vector3 startPosition, float checkerZOffset, int cellNumber,
			Checker[] checkers = null)
		{
			Index = cellNumber;
			_startPosition = startPosition;
			_checkerZOffset = new Vector3(0, 0, checkerZOffset);

			Checkers = new Stack<Checker>();
			_defaultColour = defaultCheckerColour;

			InitStack(checkers, parent, checkerAmount);
			CreateActiveSlotMarker(markerPrefab, parent);
		}

		private void CreateActiveSlotMarker(GameObject markerPrefab, Transform parent)
		{
			_marker = Object.Instantiate(markerPrefab, parent);
			_marker.SetActive(false);
		}

		private void InitStack(Checker[] checkers, Transform parent, int checkerAmount)
		{
			if (checkers == null) return;

			foreach(var checker in checkers)
			{
				checker.transform.SetParent(parent);
				checker.Initialize(this, _defaultColour);

				Checkers.Push(checker);
			}
		}

		public void HighlightActiveSlot()
		{
			_marker.transform.localPosition = AvailableLocalSlot;
			_marker.SetActive(true);
		}

		public void TurnOffActiveSlot()
		{
			_marker.SetActive(false);
		}

		public bool CanPlaceCheckerWith(PlayerColour currentCheckerColour)
		{
			Checkers.TryPeek(out Checker checker);

			return checker == null || checker.Colour == currentCheckerColour;
		}
	}
}
