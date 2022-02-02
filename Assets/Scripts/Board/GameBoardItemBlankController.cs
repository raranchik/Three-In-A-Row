using GameControl;
using UnityEngine;

namespace Board
{
    public class GameBoardItemBlankController : MonoBehaviour
    {
        [SerializeField]
        private float fallingSpeed = 15f;

        public int Layer { get; set; }
        public Color SpriteColor { get; set; }
        public Vector3 Size { get; set; }
        protected bool IsInvocable { get; set; }
        protected Vector2 TargetPos { get; set; }
        protected GameBoardItemController TargetItem { get; set; }

        private static int _hiddenLayerMask;
        private static int _blankLayerMask;

        private Vector2 _currentPos;
        private SpriteRenderer _spriteRend;
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _spriteRend = GetComponent<SpriteRenderer>();
            _transform.localScale = Size;
            _spriteRend.color = SpriteColor;
            _hiddenLayerMask = GameBoardControl.Instance.HiddenLayerMask;
            _blankLayerMask = GameBoardControl.Instance.BlankLayerMask;
        }

        private void Update()
        {
            Fall();
        }

        private void Fall()
        {
            _currentPos = _transform.position;

            if (TargetItem == null && !IsInvocable)
            {
                RaycastHit2D[] hitHidden = Physics2D.RaycastAll(_currentPos, Vector2.down, Mathf.Infinity, _hiddenLayerMask);
                int countHidden = hitHidden.Length;
                if (countHidden > 1)
                {
                    TargetPos = hitHidden[1].transform.position;
                }
                else if (countHidden == 1)
                {
                    RaycastHit2D h = hitHidden[0];
                    TargetItem = h.collider.GetComponent<GameBoardItemController>();
                    TargetItem.gameObject.layer = Layer;
                    TargetPos = h.transform.position;
                    CallOthers();
                }
            }

            if (_currentPos == TargetPos && TargetItem != null)
                Stop();
            else if (_currentPos == TargetPos && IsInvocable)
            {
                IsInvocable = false;
            }

            _transform.position = Vector2.MoveTowards(_currentPos, TargetPos, fallingSpeed * Time.deltaTime);
        }

        private void Stop()
        {
            TargetItem.Image.color = SpriteColor;
            TargetItem.Button.interactable = true;
            TargetItem.IsDisable = false;

            Destroy(gameObject);
        }

        private void CallOthers()
        {
            RaycastHit2D[] hitBlank = Physics2D.RaycastAll(_currentPos, Vector2.up, Mathf.Infinity, _blankLayerMask);
            if (hitBlank.Length < 2)
                return;

            RaycastHit2D[] hitHidden = Physics2D.RaycastAll(_currentPos, Vector2.up, Mathf.Infinity, _hiddenLayerMask);
            for (int i = 1; i < hitBlank.Length; i++)
            {
                GameBoardItemBlankController b = hitBlank[i].collider.GetComponent<GameBoardItemBlankController>();
                b.TargetPos = hitHidden[i - 1].transform.position;
                b.IsInvocable = true;
            }
        }

    }

}
