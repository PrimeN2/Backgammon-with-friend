using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Board
{
	public class GameBoardData : MonoBehaviour
	{
		public Checker[] _whiteCheckers;
		public Checker[] _blackCheckers;

		public GameObject _whiteExit;
		public GameObject _blackExit;

		public GameObject _markerPrefab;
	}
}
