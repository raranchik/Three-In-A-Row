using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Board
{
    public class GameBoardScoreController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreText;

        public static GameBoardScoreController Instance { get; private set; }

        public int Score
        {
            get => _score;
            set
            {
                string textValue = Regex.Match(scoreText.text, @"\d+").Value;
                Int32.TryParse(textValue, out var s);
                s += value;
                _score = s;
                scoreText.text = $"Счёт: {_score}";
            }
        }

        private int _score = 0;

        private void Awake()
        {
            Instance = this;
            Instance.Score = 0;
        }

    }

}