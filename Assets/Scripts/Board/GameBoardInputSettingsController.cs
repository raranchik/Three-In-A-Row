using System;
using GameControl;
using TMPro;
using UnityEngine;

namespace Board
{
    public class GameBoardInputSettingsController : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inpX;
        [SerializeField]
        private TMP_InputField inpY;
        [SerializeField]
        private TMP_InputField inpCountColors;
        [SerializeField]
        private GameObject messagePanel;
        [SerializeField]
        private TextMeshProUGUI messagePanelText;
        [SerializeField, TextArea] 
        private string validFailMessageTemplate =
            "Введённые данные некорректны.\n" +
            "Ширина от {0} до {1}.\n" +
            "Высота от {2} до {3}.\n" +
            "Цвета от {4} до {5}.";

        private int _x;
        private int _y;
        private int _countColors;

        public void StartNewGame()
        {
            if (!InputIsCorrect())
                return;

            GameBoardSettings settings = new GameBoardSettings(_x, _y, _countColors);

            bool settingsIsValid =  GameBoardSettings.ValidSettings(settings);
            if (!settingsIsValid)
            {
                ThrowFailMessage();
                return;
            }

            GameBoardControl.NewGame(settings);
        }

        private bool InputIsCorrect()
        {
            string input;

            input = inpX.text;
            if (!Int32.TryParse(input, out _x))
            {
                ThrowFailMessage();
                return false;
            }

            input = inpY.text;
            if (!Int32.TryParse(input, out _y))
            {
                ThrowFailMessage();
                return false;
            }

            input = inpCountColors.text;
            if (!Int32.TryParse(input, out _countColors))
            {
                ThrowFailMessage();
                return false;
            }

            return true;
        }

        private void ThrowFailMessage()
        {
            messagePanelText.text = string.Format(
                validFailMessageTemplate,
                GameBoardSettings.MinX, GameBoardSettings.MaxX,
                GameBoardSettings.MinY, GameBoardSettings.MaxY,
                GameBoardSettings.MinCountColors, GameBoardSettings.MaxCountColors
                );

            messagePanel.SetActive(true);
        }

    }

}