using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Infrastructure
{
	public class MainMenuState : BaseGameState
	{
		[SerializeField] private GameObject _mainMenu;
		[SerializeField] private Button _hostGameButton;
		[SerializeField] private Button _joinGameButton;

		public MainMenuState(IGameStateSwitcher stateSwitcher, GameObject mainMenu, 
			Button hostGameButton, Button joinGameButton) : 
			base(stateSwitcher)
		{
			_mainMenu = mainMenu;
			_hostGameButton = hostGameButton;
			_joinGameButton = joinGameButton;
		}

		public override void Load()
		{
			_mainMenu.SetActive(true);

			_hostGameButton.onClick.AddListener(() =>
			{
				NetworkManager.Singleton.StartHost();
				_stateSwitcher.SwitchState<GameplayState>();
			});

			_joinGameButton.onClick.AddListener(() =>
			{
				NetworkManager.Singleton.StartClient();

				NetworkManager.Singleton.OnClientConnectedCallback += StartGame;
			});
		}

		public override void Dispose()
		{
			_hostGameButton.onClick.RemoveAllListeners();
			_joinGameButton.onClick.RemoveAllListeners();

			_mainMenu.SetActive(false);
		}

		private void StartGame(ulong client)
		{
			_stateSwitcher.SwitchState<GameplayState>();
		}
	}
}
