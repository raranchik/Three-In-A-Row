using System.Collections.Generic;
using GameControl;
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

        private GameBoardSettings _settings;
        private Color[] _selectedColors;
        private RectTransform _transform;

        private void Awake()
        {
            _settings = GameBoardControl.Instance.GameBoardSettings;
            _transform = GetComponent<RectTransform>();
            IdentifyColors();
            FillGameBoard();
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

    }

}
