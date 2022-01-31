using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameControl
{
    public class GameBoardControl : MonoBehaviour
    {
        [SerializeField]
        private GameObject gameBoardPref;

        public static readonly string[] NameLayerSee =
        {
            "Color_0", "Color_1", "Color_2", "Color_3", "Color_4"
        };
        public static readonly string NameLayerHidden = "Hidden";
        public static readonly string NameLayerBreakLine = "Break";
        public static readonly string NameLayerBlank = "Blank";

        public static GameBoardControl Instance { get; private set; }

        private GameBoardSettings _gameBoardSettings;

        public GameBoardSettings GameBoardSettings
        {
            get => Instance._gameBoardSettings;
            private set => Instance._gameBoardSettings = value;
        }

        public int HiddenLayerMask { get; private set; }
        public int SeeLayerMask { get; private set; }
        public int BreakLineLayerMask { get; private set; }
        public int BlankLayerMask { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _gameBoardSettings = GameBoardSettings.GetDefaultSettings();
                DontDestroyOnLoad(gameObject);
            }

            GameStart();
        }

        public static void NewGame(in GameBoardSettings settings)
        {
            if (Application.CanStreamedLevelBeLoaded("Main"))
            {
                GameBoardControl.Instance._gameBoardSettings = settings;
                SceneManager.LoadScene("Main");
            }
        }

        private void GameStart()
        {
            HiddenLayerMask = LayerMask.GetMask(NameLayerHidden);
            SeeLayerMask = LayerMask.GetMask(NameLayerSee);
            BreakLineLayerMask = LayerMask.GetMask(NameLayerBreakLine);
            BlankLayerMask = LayerMask.GetMask(NameLayerBlank);

            GameObject gameBoard =  Instantiate(gameBoardPref);
            gameBoard.GetComponent<Canvas>().worldCamera = Camera.main;
            gameBoard.SetActive(true);
        }

    }

}