using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Board;
using UnityEngine;

public class GameBoardItemBlankController : MonoBehaviour
{
    [SerializeField]
    private float fallingSpeed = 3f;

    public GameBoardItemController TargetItem
    {
        get => _targetItem;
        set
        {
            _targetPosition = value.WorldPos;
            _targetItem = value;
        }
    }

    public Color SpriteColor
    {
        get => _spriteRend.color;
        set => _spriteRend.color = value;
    }

    public Vector3 Size
    {
        get => _transform.localScale;
        set => _transform.localScale = value;
    }

    private Vector2 _targetPosition;
    private GameBoardItemController _targetItem;
    private SpriteRenderer _spriteRend;
    private Transform _transform;
    private Color _spriteColor;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _spriteRend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 pos = _transform.position;
        _transform.position = Vector2.MoveTowards(pos, _targetPosition, fallingSpeed * Time.deltaTime);
    }

}
