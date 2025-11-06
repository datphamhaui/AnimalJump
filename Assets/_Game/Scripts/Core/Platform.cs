using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] Vector2 _gap;
    [SerializeField] int _seedSize = 10;
    [SerializeField] Transform _container;

    Piece _piece;

    float _pieceSpeed;
    float newXPos = 0f;
    float _resetPos;
    bool _isOver = false;

    bool _isInverted = false;
    public bool InvertPos { set { _isInverted = value; } }

    /// <summary>
    /// Set piece cho platform (được gọi từ PlatformSpawner)
    /// </summary>
    public void SetPiece(Piece piece)
    {
        _piece = piece;
    }

    List<Piece> _pieceList = new List<Piece>();

    private LevelManager _levelManager;

    private void OnEnable()
    {
        GameManager.OnGameEnd += StopMoving;
        GameManager.OnRevive += MoveEnabled;
        LevelManager.OnLevelChanged += UpdateSpeed;
    }

    private void Start()
    {
        if (_isInverted)
        {
            transform.eulerAngles = Vector3.up * 180;
        }

        // Lấy config từ LevelManager
        _levelManager = FindFirstObjectByType<LevelManager>();
        if (_levelManager != null)
        {
            _pieceSpeed = _levelManager.GetCurrentSpeed();
            _gap = _levelManager.GetPlatformGapRange(); // Lấy gap từ level config
        }
        else
        {
            _pieceSpeed = 2f; // Fallback speed
            Debug.LogWarning("LevelManager not found! Using default speed.");
        }

        if (_piece != null)
        {
            GeneratePiece();
        }
    }

    /// <summary>
    /// Cập nhật tốc độ khi level thay đổi
    /// </summary>
    private void UpdateSpeed(int level, float newSpeed)
    {
        _pieceSpeed = newSpeed;
        Debug.Log($"Platform speed updated to: {_pieceSpeed} (Level {level})");
    }

    void GeneratePiece()
    {
        for (int i = 0; i < _seedSize; i++)
        {
            Piece newPiece = Instantiate(_piece, _container);
            newPiece.transform.localPosition += Vector3.right * newXPos;
            _pieceList.Add(newPiece);

            float gap = Random.Range(_gap.x, _gap.y);
            newXPos += _piece.HalfWidth + _piece.HalfWidth + gap;
        }

        _resetPos = newXPos;
    }

    private void Update()
    {
        MovePieces();
    }

    private void MoveEnabled()
    {
        _isOver = false;
    }

    private void MovePieces()
    {
        if (_isOver) return;

        foreach (var piece in _pieceList)
        {
            piece.transform.Translate(Vector3.right * _pieceSpeed * Time.deltaTime);

            if (piece.transform.localPosition.x >= _resetPos)
            {
                Vector3 newPos = piece.transform.localPosition;
                newPos.x = 0;
                piece.transform.localPosition = newPos;
            }
        }
    }

    private void StopMoving()
    {
        _isOver = true;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd -= StopMoving;
        GameManager.OnRevive -= MoveEnabled;
        LevelManager.OnLevelChanged -= UpdateSpeed;
    }
}
