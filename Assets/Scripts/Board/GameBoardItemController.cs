using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using GameControl;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    public class GameBoardItemController : MonoBehaviour
    {
        [SerializeField]
        private float findIntervalTime = 1f;
        [SerializeField]
        private GameObject itemBlank;

        public static readonly List<Vector2> AvailableRayDirections = new List<Vector2>()
            { Vector2.right, Vector2.down, Vector2.left, Vector2.up };

        public Vector2 WorldPos { get; private set; }
        public Vector2 ItemSize { get; private set; } = Vector2.zero;
        public Vector2 ScaleItemSize { get; private set; } = Vector2.zero;
        public BoxCollider2D Collider { get; private set; }
        public Image Image { get; private set; }
        public Button Button { get; private set; }
        public int Layer { get; private set; }

        private RectTransform _rectTransform;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
            _rectTransform = GetComponent<RectTransform>();
            Button = GetComponent<Button>();
            Image = GetComponent<Image>();
            Layer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
        }

        private void Start()
        {
            // InvokeRepeating(nameof(FindIdenticalGameBoardItems), 0f, findIntervalTime);
        }

        private void Update()
        {
            ItemSize = _rectTransform.rect.size;
            if (ItemSize != Vector2.zero)
            {
                WorldPos = _rectTransform.position;
                Collider.size = ItemSize;
                ScaleItemSize = ItemSize / (_rectTransform.localScale * 100);
            }
        }

        public void FindAndBreakRelatedItems()
        {
            List<GameBoardItemController> relatedItems = new List<GameBoardItemController> { this };
            GetRelatedItems(ref relatedItems, this);
            if (relatedItems.Count < 3)
                return;

            SetUpItems(relatedItems, false);
            // ExcludeXAxis(relatedItems);
            ExecuteFall(relatedItems);
            Debug.Log("DESTROY THIS");
        }

        private void ExcludeXAxis(in List<GameBoardItemController> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                if (items[i].WorldPos.x == items[i + 1].WorldPos.x)
                    items.Remove(items[i]);
            }
        }

        private void ExecuteFall(in List<GameBoardItemController> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                List<GameBoardItemController> upperItems = GetUpperItems(items[i]);
                for (int j = 0; j < upperItems.Count; j++)
                {
                    GameObject blank = Instantiate(itemBlank, upperItems[j].WorldPos, Quaternion.identity);
                    GameBoardItemBlankController blankController = blank.GetComponent<GameBoardItemBlankController>();
                    blankController.SpriteColor = upperItems[j].Image.color;
                    blankController.Size = upperItems[j].ScaleItemSize;
                    // blankController.TargetItem = i;
                }
                SetUpItems(upperItems, false);
            }
        }

        private List<GameBoardItemController> GetUpperItems(GameBoardItemController item)
        {
            List<GameBoardItemController> upperItems = new List<GameBoardItemController>();
            Vector2 upDir = Vector2.up;
            RaycastHit2D[] hitsUp = Physics2D.RaycastAll(item.WorldPos, upDir);
            foreach (RaycastHit2D hit in hitsUp)
            {
                GameBoardItemController itemController =
                    hit.collider.gameObject.GetComponent<GameBoardItemController>();
                upperItems.Add(itemController);
            }

            return upperItems;
        }

        private void SetUpItems(in List<GameBoardItemController> items, in bool enable)
        {
            foreach (GameBoardItemController item in items)
            {
                item.Collider.enabled = enable;
                item.Button.interactable = false;
                if (!enable)
                    item.Image.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        private void GetRelatedItems(ref List<GameBoardItemController> relatedItems, in GameBoardItemController item)
        {
            List<Vector2> directions = GameBoardItemController.AvailableRayDirections;
            for (int i = 1; i <= directions.Count; i++)
            {
                Vector2 dir = directions[i - 1];
                float distance = i % 2 == 1 ? item.ScaleItemSize.x : item.ScaleItemSize.y;
                RaycastHit2D hit = Physics2D.Raycast(item.WorldPos, dir, distance, item.Layer);

                if (hit.collider == null)
                    continue;

                GameObject anotherItem = hit.collider.gameObject;
                GameBoardItemController anotherItemController = anotherItem.GetComponent<GameBoardItemController>();
                if (relatedItems.Contains(anotherItemController))
                    continue;

                relatedItems.Add(anotherItemController);
                GetRelatedItems(ref relatedItems, anotherItemController);
            }
        }

    }

}