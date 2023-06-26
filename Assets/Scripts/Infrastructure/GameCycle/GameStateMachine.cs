using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Input;
using Project.Board;
using Project.Dice;
using Project.Player;
using UnityEngine.UI;

namespace Project.Infrastructure
{
	public class GameStateMachine : MonoBehaviour, IGameStateSwitcher
	{
		[Header("UI")]
		[SerializeField] private GameObject _mainMenu;
		[SerializeField] private Button _hostGameButton;
		[SerializeField] private Button _joinGameButton;

		[SerializeField] private DataSender _sender;

		[SerializeField] private CheckerMover _whitePlayer;
		[SerializeField] private CheckerMover _blackPlayer;

		[SerializeField] private DiceThrower _thrower;

		[SerializeField] private GameBoardData _boardData;
		[SerializeField] private PlayerInput _inputHandler;
		[SerializeField] private PlayerCameraDefiner _cameraDefiner;

		public BaseGameState _currentState { get; private set; }
		private List<BaseGameState> _allStates;

		private void Start()
		{
			_allStates = new List<BaseGameState>()
			{
				new MainMenuState(this, _mainMenu, _hostGameButton, _joinGameButton),
				new GameplayState(this, _inputHandler, _thrower, 
				_whitePlayer, _blackPlayer, _cameraDefiner,
				_sender, _boardData)
			};

			_currentState = _allStates[0];

			_currentState.Load();
		}

		public void SwitchState<T>() where T : BaseGameState
		{
			var state = _allStates.FirstOrDefault(s => s is T);

			_currentState.Dispose();
			_currentState = state;
			_currentState.Load();
		}

		private void OnDisable()
		{
			_currentState.Dispose();
		}
	}
}
