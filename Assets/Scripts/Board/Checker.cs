using Project.Player;
using UnityEngine;

namespace Project.Board
{
	public class Checker : MonoBehaviour
	{
		public bool CanGoOut { get; set; }

		public Vector3 OnBoardLocalPosition => 
			new Vector3(transform.localPosition.x, 1.37f, transform.localPosition.z);
		public Cell Cell { get; private set; }
		public PlayerColour Colour { get; private set; }

		public bool IsOnTop()
		{
			Checker checker;

			return Cell.Checkers.TryPeek(out checker) && checker == this;
		} 

		public void Initialize(Cell cell, PlayerColour owner)
		{
			Colour = owner;
			Cell = cell;

			CanGoOut = false;
		}

		public void MoveTo(Cell cell)
		{
			if (cell.Checkers.Contains(this)) return;

			Cell = cell;
			cell.Checkers.Push(this);
		}
	}

}
