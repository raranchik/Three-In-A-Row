using System.Collections.Generic;
using GameControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Board
{
    public class GameBoardGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject gameBoardHorizontalRow;
        [SerializeField]
        private GameObject gameBoardVerticalRow;
        [SerializeField]
        private GameObject gameBoardItem;
        [SerializeField]
        private RectOffset padding;
        [SerializeField]
        private int spacing;
        [SerializeField]
        private Color[] availableColors;
        [SerializeField]
        private BoxCollider2D breakLine;
        [SerializeField]
        private GameObject messagePanel;
        [SerializeField]
        private TextMeshProUGUI messagePanelText;
        [SerializeField, TextArea] 
        private string gameOverMessageTemplate = "Ходы закончились. Ваш счёт {0}. Поздравляем!";

        private List<GameBoardItemController> _gameBoardItems = new List<GameBoardItemController>();
        private GameBoardSettings _settings;
        private Color[] _selectedColors;
        private RectTransform _transform;
        private bool _scanning = false;

        private void Awake()
        {
            _settings = GameBoardControl.Instance.GameBoardSettings;
            _transform = GetComponent<RectTransform>();
            IdentifyColors();
            FillGameBoard();
        }

        private void Start()
        {
            InvokeRepeating(nameof(ScanGameBoard), 5f, 1.5f);
        }

        private void Update()
        {
            breakLine.size = new Vector2(_transform.rect.size.x, 1f);
        }

        private void FillGameBoard()
        {
            int x = _settings.X;
            int y = _settings.Y;
            int countColors = _settings.CountColors;
            VerticalLayoutGroup vertical = gameBoardVerticalRow.GetComponent<VerticalLayoutGroup>();
            vertical.spacing = spacing;
            vertical.padding = padding;
            RectTransform verticalRectTransform = gameBoardVerticalRow.GetComponent<RectTransform>();
            for (int i = 0; i < y; i++)
            {
                GameObject horizontal = Instantiate(gameBoardHorizontalRow, verticalRectTransform);
                horizontal.GetComponent<HorizontalLayoutGroup>().spacing = spacing;
                RectTransform horizontalRectTransform = horizontal.GetComponent<RectTransform>();
                for (int j = 0; j < x; j++)
                {
                    GameObject item = Instantiate(gameBoardItem, horizontalRectTransform);
                    Image itemImage = item.GetComponent<Image>();
                    int selectedColor = Random.Range(0, countColors);
                    itemImage.color = _selectedColors[selectedColor];
                    item.layer = LayerMask.NameToLayer("Color_" + selectedColor);
                    item.SetActive(true);
                    _gameBoardItems.Add(item.GetComponent<GameBoardItemController>());
                }
            }
        }

        private void IdentifyColors()
        {
            int countColors = _settings.CountColors;
            int availableColorsCount = availableColors.Length;

            System.Random rand = new System.Random();
            List<int> selectedColorsNums = new List<int>();
            _selectedColors = new Color[countColors];

            for (int i = 0; i < countColors; i++)
            {
                int selectedColorNum = rand.Next(0, availableColorsCount);
                while (selectedColorsNums.Contains(selectedColorNum))
                {
                    selectedColorNum = rand.Next(0, availableColorsCount);
                }
                selectedColorsNums.Add(selectedColorNum);

                _selectedColors[i] = availableColors[selectedColorNum];
            }
        }

        private void ScanGameBoard()
        {
            if (_scanning)
                return;
            if (_gameBoardItems.Count == 0)
                return;

            _scanning = true;

            List<GameBoardItemController> scannedItems = new List<GameBoardItemController>();
            foreach (GameBoardItemController i in _gameBoardItems)
            {
                if (scannedItems.Contains(i))
                    continue;

                GameBoardItemController.GetItemLayerMask(i, out int iLayerMask);
                if (iLayerMask == GameBoardControl.Instance.HiddenLayerMask)
                    continue;

                scannedItems.Add(i);
                List<GameBoardItemController> relatedItems = new List<GameBoardItemController>() { i };
                GameBoardItemController.GetRelatedItems(ref relatedItems, i, iLayerMask);

                if (relatedItems.Count < 3)
                {
                    continue;
                }
                else
                {
                    _scanning = false;
                    return;
                }
            }

            GameOver();
        }

        private void GameOver()
        {
            messagePanelText.text = string.Format(gameOverMessageTemplate, GameBoardScoreController.Instance.Score);
            CancelInvoke(nameof(ScanGameBoard));
            messagePanel.SetActive(true);
        }

    }

}
