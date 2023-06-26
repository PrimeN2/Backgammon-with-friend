using UnityEngine;

namespace Project.Dice
{
	public class Dice : MonoBehaviour
	{
		public DiceSide[] diceSides;

		public int GetDiceNumber()
		{
			return GetHighestSide(diceSides);
		}

		private int GetHighestSide(DiceSide[] diceSides)
		{
			int highestSideValue = 0;
			float highestYPosition = float.MinValue;

			foreach (DiceSide diceSide in diceSides)
			{
				if (diceSide.transform.position.y > highestYPosition)
				{
					highestYPosition = diceSide.transform.position.y;
					highestSideValue = diceSide.sideValue;
				}
			}

			return highestSideValue;
		}
	}
}
