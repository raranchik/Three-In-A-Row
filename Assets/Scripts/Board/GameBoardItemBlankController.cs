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
        }

        private void Update()
        {
            _currentPos = _transform.position;
            Vector2 moveTo = _targetItem == null
                ? new Vector2(_currentPos.x, _currentPos.y - Size.y)
                : _targetPos;
            _transform.position = Vector2.MoveTowards(_currentPos, moveTo, fallingSpeed * Time.deltaTime);

            Debug.DrawLine(_currentPos, moveTo, Color.white);
            if (NeedFindStopPoint())
            {
                SetStopPoint();
            }

            if (_currentPos == _targetPos)
            {
                _targetItem.Image.color = SpriteColor;
                _targetItem.Collider.enabled = true;
                _targetItem.Button.interactable = true;
                _targetItem.IsDisable = false;
                _targetItem.gameObject.layer = Layer;

                Destroy(gameObject);
            }
        }

        public void SetStopPoint()
        {
            int hiddenLayerMask = GameBoardControl.Instance.HiddenLayerMask;
            RaycastHit2D[] hitHidden = Physics2D.RaycastAll(_currentPos, Vector2.down, 0.01f, hiddenLayerMask);

            if (hitHidden.Length != 0)
            {
                GameObject i = hitHidden[0].collider.gameObject;
                TargetItem = i.GetComponent<GameBoardItemController>();
                InvokeStop();
            }
        }

        private bool NeedFindStopPoint()
        {
            int seeLayerMask = GameBoardControl.Instance.SeeLayerMask;
            int breakLineLayerMask = GameBoardControl.Instance.BreakLineLayerMask;
            RaycastHit2D[] hitSee = Physics2D.RaycastAll(_currentPos, Vector2.down , Size.y, seeLayerMask + breakLineLayerMask);
            
            if (hitSee.Length == 0)
            {
                _targetItem = null;
                _targetPos = Vector2.zero;
                return false;
            }

            return true;
        }

        private void InvokeStop()
        {
            int blankLayerMask = GameBoardControl.Instance.BlankLayerMask;
            RaycastHit2D[] hitBlank = Physics2D.RaycastAll(_currentPos, Vector2.up, Size.y, blankLayerMask);
            for (int i = 1; i < hitBlank.Length; i++)
            {
                GameObject b = hitBlank[i].collider.gameObject;
                GameBoardItemBlankController bController = b.GetComponent<GameBoardItemBlankController>();
                bController.IsInvocable = true;
                bController.SetStopPoint();
            }
        }

    }

}
