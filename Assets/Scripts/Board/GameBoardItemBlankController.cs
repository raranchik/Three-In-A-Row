using GameControl;
using UnityEngine;

namespace Board
{
    public class GameBoardItemBlankController : MonoBehaviour
    {
        [SerializeField]
        private float fallingSpeed = 3f;

        public GameBoardItemController TargetItem
        {
            get => _targetItem;
            private set
            {
                _targetItem = value;
                _targetPos = value.WorldPos;
            }
        }

        public int Layer { get; set; }
        public Color SpriteColor { get; set; }
        public Vector3 Size { get; set; }
        public bool IsInvocable { get; set; }

        private static int _hiddenLayerMask;
        private static int _blankLayerMask;

        private Vector2 _currentPos;
        private Vector2 _targetPos;
        private GameBoardItemController _targetItem;
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

            if (_targetItem == null || IsInvocable)
            {
                RaycastHit2D[] hitHidden = Physics2D.RaycastAll(_currentPos, Vector2.down, Mathf.Infinity, _hiddenLayerMask);

                int countHidden = hitHidden.Length;
                if (countHidden >= 2)
                {
                    _targetPos = hitHidden[1].transform.position;
                    _targetItem = null;
                    IsInvocable = false;
                }
                else if (countHidden == 1)
                {
                    GameObject i = hitHidden[0].collider.gameObject;
                    TargetItem = i.GetComponent<GameBoardItemController>();
                    i.layer = Layer;
                    CallOthers();
                }
            }

            if (_currentPos == _targetPos)
                Stop();

            _transform.position = Vector2.MoveTowards(_currentPos, _targetPos, fallingSpeed * Time.deltaTime);
        }

        private void Stop()
        {
            if (_targetItem is null)
            {
                Destroy(gameObject);
                return;
            }

            _targetItem.Image.color = SpriteColor;
            _targetItem.Button.interactable = true;
            _targetItem.IsDisable = false;

            Destroy(gameObject);
        }

        private void CallOthers()
        {
            RaycastHit2D[] hitBlank = Physics2D.RaycastAll(_currentPos, Vector2.up, Mathf.Infinity, _blankLayerMask);
            foreach (RaycastHit2D h in hitBlank)
            {
                GameBoardItemBlankController b = h.collider.gameObject.GetComponent<GameBoardItemBlankController>();
                b.IsInvocable = true;
            }
        }

    }

}
