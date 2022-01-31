using System.Collections.Generic;
using GameControl;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    public class GameBoardItemController : MonoBehaviour
    {
        [SerializeField]
        private GameObject itemBlankPref;

        public static readonly Vector2[] AvailableRayDirections =
        {
            Vector2.right, Vector2.down, Vector2.left, Vector2.up
        };

        public Vector2 WorldPos { get; private set; }
        public Vector2 ItemSize { get; private set; } = Vector2.zero;
        public Vector2 ScaleItemSize { get; private set; } = Vector2.zero;
        public BoxCollider2D Collider { get; private set; }
        public Image Image { get; private set; }
        public Button Button { get; private set; }
        public bool IsDisable { get; set; }

        private RectTransform _rectTransform;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
            _rectTransform = GetComponent<RectTransform>();
            Button = GetComponent<Button>();
            Image = GetComponent<Image>();
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
            int layer = gameObject.layer;
            string layerName = LayerMask.LayerToName(layer);
            int layerMask = LayerMask.GetMask(layerName);

            GetRelatedItems(ref relatedItems, this, layerMask);
            if (relatedItems.Count < 3)
                return;

            foreach (GameBoardItemController i in relatedItems)
            {
                i.IsDisable = true;
            }

            ExecuteFall(relatedItems);
        }

        private void ExecuteFall(in List<GameBoardItemController> items)
        {
            List<GameBoardItemBlankController> blanks = new List<GameBoardItemBlankController>();

            foreach (GameBoardItemController i in items)
            {
                List<GameBoardItemController> itemsFromXAxis = GetAllItemsFromXAxis(i);
                foreach (GameBoardItemController j in itemsFromXAxis)
                {
                    if (j.IsDisable)
                    {
                        ConfigureItem(j, false);
                        continue;
                    }

                    GameObject b = Instantiate(itemBlankPref, j.WorldPos, Quaternion.identity);
                    GameBoardItemBlankController bController = b.GetComponent<GameBoardItemBlankController>();
                    bController.Layer = j.gameObject.layer;
                    bController.Size = j.ScaleItemSize;
                    bController.SpriteColor = j.Image.color;
                    blanks.Add(bController);

                    ConfigureItem(j, false);
                }
            }

            foreach (GameBoardItemBlankController b in blanks)
            {
                b.gameObject.SetActive(true);
            }

            GameBoardScoreController.Instance.Score = items.Count;
        }

        private bool ExitMoreDisabled(in GameBoardItemController item, in Vector2 dir)
        {
            int seeLayerMask = GameBoardControl.Instance.SeeLayerMask;
            RaycastHit2D[] hits = Physics2D.RaycastAll(item.WorldPos, dir, Mathf.Infinity, seeLayerMask);
            foreach (RaycastHit2D h in hits)
            {
                GameObject i = h.collider.gameObject;
                GameBoardItemController iController = i.GetComponent<GameBoardItemController>();
                if (iController.Equals(item))
                    continue;

                if (iController.IsDisable)
                    return true;
            }

            return false;
        }

        private List<GameBoardItemController> GetAllItemsFromXAxis(in GameBoardItemController item)
        {
            List<GameBoardItemController> items = new List<GameBoardItemController>();
            int seeLayerMask = GameBoardControl.Instance.SeeLayerMask;

            Vector2 dir = GameBoardItemController.AvailableRayDirections[1]; // Down dir
            RaycastHit2D[] hits = Physics2D.RaycastAll(item.WorldPos, dir, Mathf.Infinity, seeLayerMask);
            foreach (RaycastHit2D h in hits)
            {
                GameObject i = h.collider.gameObject;

                GameBoardItemController iController = i.GetComponent<GameBoardItemController>();
                if (iController.Equals(item))
                    continue;

                if (iController.IsDisable)
                    items.Add(iController);

                if (ExitMoreDisabled(iController, dir))
                {
                    if (!iController.IsDisable)
                        items.Add(iController);
                }
                else break;
            }

            items.Add(item); // Middle item

            dir = GameBoardItemController.AvailableRayDirections[3]; // Up dir
            hits = Physics2D.RaycastAll(item.WorldPos, dir, Mathf.Infinity, seeLayerMask);
            foreach (RaycastHit2D h in hits)
            {
                GameObject i = h.collider.gameObject;
                GameBoardItemController iController = i.GetComponent<GameBoardItemController>();
                if (iController.Equals(item))
                    continue;

                items.Add(iController);
            }

            return items;
        }

        private void ConfigureItem(in GameBoardItemController item, in bool enable)
        {
            item.gameObject.layer = LayerMask.NameToLayer(GameBoardControl.NameLayerHidden);
            item.Button.interactable = enable;
            if (!enable)
                item.Image.color = new Color(0f, 0f, 0f, 0f);
        }

        private void GetRelatedItems(ref List<GameBoardItemController> relatedItems, in GameBoardItemController item, in int layerMask)
        {
            Vector2[] dirs = GameBoardItemController.AvailableRayDirections;
            for (int i = 1; i <= dirs.Length; i++)
            {
                Vector2 dir = dirs[i - 1];
                float dist = i % 2 == 1 ? item.ScaleItemSize.x : item.ScaleItemSize.y;
                RaycastHit2D[] hit = Physics2D.RaycastAll(item.WorldPos, dir, dist, layerMask);

                if (hit.Length < 2)
                    continue;

                GameObject rItem = hit[1].collider.gameObject;
                GameBoardItemController rIController = rItem.GetComponent<GameBoardItemController>();
                if (relatedItems.Contains(rIController))
                    continue;

                relatedItems.Add(rIController);
                GetRelatedItems(ref relatedItems, rIController, layerMask);
            }
        }

    }

}