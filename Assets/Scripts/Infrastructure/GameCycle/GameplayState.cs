using Project.Board;
using Project.Dice;
using Project.Input;
using Project.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Infrastructure
{
	public class GameplayState : BaseGameState
	{
		private DataSender _sender;
		private GameBoardData _boardData;

		private PlayerData _whitePlayerSettings;
		private PlayerData _blackPlayerSettings;

		private CheckerMover _whitePlayer;
		private CheckerMover _blackPlayer;

		private DiceThrower _thrower;

		private PlayerTurn _playerTurn;
		private GameBoard _board;

		private PlayerInput _input;

		private PlayerCameraDefiner _cameraDefiner;

		public GameplayState(
			IGameStateSwitcher stateSwitcher, PlayerInput input,
			DiceThrower thrower, CheckerMover whitePlayer, CheckerMover blackPlayer, 
			PlayerCameraDefiner cameraDefiner, DataSender sender, GameBoardData boardData)
			: base(stateSwitcher)
		{
			_sender = sender;
			_boardData = boardData;
			_whitePlayer = whitePlayer;
			_blackPlayer = blackPlayer;
			_cameraDefiner = cameraDefiner;

			_input = input;
			_thrower = thrower;
		}

		public override void Load()
		{
			_board = new GameBoard(_boardData);
			_playerTurn = new PlayerTurn(_board, _input);

			_input.Construct(_sender);
			_input.Init(1, _thrower);
			_input.CurrentPlayerID = 0;
			_input.CurrentActiveCamera = _cameraDefiner.WhitePlayerCamera;

			_thrower.Initialize(_input);
			_whitePlayer.Initialize(_board, _playerTurn);
			_blackPlayer.Initialize(_board, _playerTurn);


			_whitePlayerSettings = 
				new PlayerData(_whitePlayer, (19, 24), PlayerColour.White, _cameraDefiner.WhitePlayerCamera);
			_blackPlayerSettings = new PlayerData(
				_blackPlayer, (7, 12), PlayerColour.Black, _cameraDefiner.BlackPlayerCamera);

			_playerTurn.SetSettings(_whitePlayerSettings);

			_cameraDefiner.SetCameras();
			_sender.Init(this, _input);

			_thrower.OnDiceLanded += StartPlayerMove;
			_playerTurn.PlayerEndsMove += PassTheTurn;
			_whitePlayer.OnCheckerLeftTheGame += IncreaseLeftCheckersAmount;
			_blackPlayer.OnCheckerLeftTheGame += IncreaseLeftCheckersAmount;
		}

		public override void Dispose()
		{
			_thrower.OnDiceLanded -= StartPlayerMove;
			_playerTurn.PlayerEndsMove -= PassTheTurn;
			_whitePlayer.OnCheckerLeftTheGame -= IncreaseLeftCheckersAmount;
			_blackPlayer.OnCheckerLeftTheGame -= IncreaseLeftCheckersAmount;
		}

		private void IncreaseLeftCheckersAmount(PlayerData player)
		{
			if (++player.Score > 15)
			{
				SceneManager.LoadScene(0);
			}
		}

		private void PassTheTurn(PlayerData previousPlayer)
		{
			_input.Init(1, _thrower);

			if (previousPlayer.Colour == PlayerColour.White)
			{
				_playerTurn.SetSettings(_blackPlayerSettings);
				_input.CurrentActiveCamera = _cameraDefiner.BlackPlayerCamera;
				_input.CurrentPlayerID = 1;
			}
			else
			{
				_playerTurn.SetSettings(_whitePlayerSettings);
				_input.CurrentActiveCamera = _cameraDefiner.WhitePlayerCamera;
				_input.CurrentPlayerID = 0;
			}
		}

		private void StartPlayerMove(int firstValue, int secondValue)
		{
			_playerTurn.SetValues(firstValue, secondValue);
			_input.Init(0, _playerTurn.CurrentPlayer.Mover);
			
			if (_input.IsHost)
				_sender.StartMoveClientRpc(firstValue, secondValue);
			else
				_sender.StartMoveServerRpc(firstValue, secondValue);
		}

		public void SynchronisePlayerMove(int firstValue, int secondValue)
		{
			_playerTurn.SetValues(firstValue, secondValue);
			_input.Init(0, _playerTurn.CurrentPlayer.Mover);
		}

		public void SynchroniseData(GameBoard board, PlayerTurn turnData,
			PlayerData whitePlayerData, PlayerData blackPlayerData, PlayerData previousPlayer)
		{
			//_board.ApplyPositionChanges(Stack<int>[])

			_whitePlayerSettings = whitePlayerData;
			_blackPlayerSettings = blackPlayerData;

			_input.Init(1, _thrower);

			if (previousPlayer.Colour == PlayerColour.White)
			{
				_playerTurn.SetSettings(_blackPlayerSettings);
				_input.CurrentActiveCamera = _cameraDefiner.BlackPlayerCamera;
				_input.CurrentPlayerID = 1;
			}
			else
			{
				_playerTurn.SetSettings(_whitePlayerSettings);
				_input.CurrentActiveCamera = _cameraDefiner.WhitePlayerCamera;
				_input.CurrentPlayerID = 0;
			}

			_thrower.Initialize(_input);
			_whitePlayer.Initialize(_board, _playerTurn);
			_blackPlayer.Initialize(_board, _playerTurn);
		}
	}
}
