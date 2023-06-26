using Project.Board;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Project.Player
{
	public class PlayerData
	{
		public int FirstTurnMovesAmount;
		public int Score;

		public readonly CheckerMover Mover;
		public readonly Camera Camera;
		public readonly PlayerColour Colour;
		public readonly (int start, int end) HomeLimits;

		public List<Checker> CheckersAtHome;

		public PlayerData(
			CheckerMover checkerMover, (int start, int end) homeLimits, PlayerColour colour, Camera camera)
		{
			CheckersAtHome = new List<Checker>();

			Score = 0;
			FirstTurnMovesAmount = 0;

			Mover = checkerMover;
			HomeLimits = homeLimits;
			Colour = colour;
			Camera = camera;
		}
	}
	public enum PlayerColour
	{
		White = 1,
		Black = -1
	}

}