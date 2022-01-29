using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameControl
{
    public class GameBoardControl : MonoBehaviour
    {
        [SerializeField]
        private GameObject gameBoardPref;

        public static GameBoardControl Instance { get; private set; }

        private GameBoardSettings _gameBoardSettings;

        public GameBoardSettings GameBoardSettings
        {
            get => Instance._gameBoardSettings;
            private set => Instance._gameBoardSettings = value;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            GameStart();
        }

        private void NewGame()
        {
            if (Application.CanStreamedLevelBeLoaded("Main"))
            {
                SceneManager.LoadScene("Main");
            }
        }

        private void GameStart()
        {
            _gameBoardSettings = new GameBoardSettings(16, 10, 3);
            GameObject gameBoard =  Instantiate(gameBoardPref);
            gameBoard.GetComponent<Canvas>().worldCamera = Camera.main;
            gameBoard.SetActive(true);
        }

    }

}