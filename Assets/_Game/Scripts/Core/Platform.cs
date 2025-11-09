using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] Vector2 _gap;

    [SerializeField] int       _seedSize = 10;
    [SerializeField] Transform _container;

    [Header("Piece Scale Settings")]
    [Tooltip("Scale ban đầu của piece theo trục X khi score = 0")]
    [SerializeField] float _initialPieceScaleX = 3f;

    [Tooltip("Tốc độ giảm scale theo điểm (scale giảm bao nhiêu mỗi điểm)")]
    [SerializeField] float _scaleDecreaseRate = 0.1f;

    [Tooltip("Scale tối thiểu của piece (không giảm xuống dưới giá trị này)")]
    [SerializeField] float _minPieceScale = 1f;

    Piece _piece;

    float _pieceSpeed;
    float newXPos = 0f;
    float _resetPos;
    bool  _isOver   = false;
    bool  _isFrozen = false; // Dùng cho checkpoint revival system

    bool        _isInverted = false;
    public bool InvertPos { set { _isInverted = value; } }

    ScoreManager _scoreManager;

    /// <summary>
    /// Set piece cho platform (được gọi từ PlatformSpawner)
    /// </summary>
    public void SetPiece(Piece piece) { _piece = piece; }

    List<Piece> _pieceList = new List<Piece>();

    private LevelManager _levelManager;

    private void OnEnable()
    {
        GameManager.OnGameEnd        += StopMoving;
        GameManager.OnRevive         += MoveEnabled;
        LevelManager.OnLevelChanged  += UpdateSpeed;
        GameManager.OnPlatformFreeze += FreezeMovement;
        GameManager.OnPlatformResume += ResumeMovement;
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
            _gap        = _levelManager.GetPlatformGapRange(); // Lấy gap từ level config
        }
        else
        {
            _pieceSpeed = 2f; // Fallback speed
            Debug.LogWarning("LevelManager not found! Using default speed.");
        }

        // Lấy ScoreManager để tính scale
        _scoreManager = FindFirstObjectByType<ScoreManager>();

        if (_scoreManager == null)
        {
            Debug.LogWarning("ScoreManager not found! Using default scale.");
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

    /// <summary>
    /// Freeze platform movement (khi player miss và revive)
    /// </summary>
    private void FreezeMovement()
    {
        _isFrozen = true;
        Debug.Log($"[Platform] Movement FROZEN");
    }

    /// <summary>
    /// Resume platform movement (khi player landed checkpoint)
    /// </summary>
    private void ResumeMovement()
    {
        _isFrozen = false;
        Debug.Log($"[Platform] Movement RESUMED");
    }

    /// <summary>
    /// Tính scale của piece dựa trên score hiện tại
    /// Scale = initialScale - (score * decreaseRate)
    /// Scale tối thiểu = minScale
    /// </summary>
    private float CalculatePieceScale()
    {
        if (_scoreManager == null)
        {
            return _initialPieceScaleX;
        }

        int   currentScore    = _scoreManager.Score;
        float calculatedScale = _initialPieceScaleX - (currentScore * _scaleDecreaseRate);

        // Đảm bảo scale không nhỏ hơn minScale
        float finalScale = Mathf.Max(calculatedScale, _minPieceScale);

        Debug.Log($"[Platform] Score: {currentScore}, Piece Scale: {finalScale:F2}");

        return finalScale;
    }

    void GeneratePiece()
    {
        // Tính scale dựa trên score hiện tại
        float pieceScale = CalculatePieceScale();

        for (int i = 0; i < _seedSize; i++)
        {
            Piece newPiece = Instantiate(_piece, _container);
            newPiece.transform.localPosition += Vector3.right * newXPos;

            // Apply scale theo trục X (giữ nguyên Y và Z)
            Vector3 originalScale = newPiece.transform.localScale;

            newPiece.transform.localScale = new Vector3(
                originalScale.x * pieceScale,
                originalScale.y,
                originalScale.z
            );

            // Điều chỉnh scale của pickup objects để chúng không bị ảnh hưởng bởi piece scaling
            newPiece.AdjustPickupScales(pieceScale);

            _pieceList.Add(newPiece);

            // Tính gap random
            float gap = Random.Range(_gap.x, _gap.y);

            // Tính position tiếp theo dựa trên HalfWidth đã scaled
            // HalfWidth của piece hiện tại (sau khi scale)
            float scaledHalfWidth = _piece.HalfWidth * pieceScale;

            // Position tiếp theo = position hiện tại + scaledHalfWidth + gap + scaledHalfWidth
            newXPos += scaledHalfWidth + gap + scaledHalfWidth;
        }

        _resetPos = newXPos;
    }

    private void Update() { MovePieces(); }

    private void MoveEnabled() { _isOver = false; }

    private void MovePieces()
    {
        if (_isOver || _isFrozen) return;

        foreach (var piece in _pieceList)
        {
            piece.transform.Translate(Vector3.right * _pieceSpeed * Time.deltaTime);

            if (piece.transform.localPosition.x >= _resetPos)
            {
                Vector3 newPos = piece.transform.localPosition;
                newPos.x                      = 0;
                piece.transform.localPosition = newPos;
            }
        }
    }

    private void StopMoving() { _isOver = true; }

    /// <summary>
    /// Lấy piece ở giữa platform (gần center nhất)
    /// Dùng để revive khi player chạm boundary wall
    /// </summary>
    public Transform GetCenterPiece()
    {
        if (_pieceList == null || _pieceList.Count == 0)
        {
            Debug.LogWarning("[Platform] No pieces available!");

            return null;
        }

        // Tìm piece có localPosition.x gần giữa platform nhất
        // Center của platform là _resetPos / 2
        float centerX         = _resetPos / 2f;
        Piece closestPiece    = _pieceList[0];
        float closestDistance = Mathf.Abs(closestPiece.transform.localPosition.x - centerX);

        foreach (var piece in _pieceList)
        {
            float distance = Mathf.Abs(piece.transform.localPosition.x - centerX);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPiece    = piece;
            }
        }

        Debug.Log($"[Platform] Center piece found at localX: {closestPiece.transform.localPosition.x} (center: {centerX})");

        return closestPiece.transform;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd        -= StopMoving;
        GameManager.OnRevive         -= MoveEnabled;
        LevelManager.OnLevelChanged  -= UpdateSpeed;
        GameManager.OnPlatformFreeze -= FreezeMovement;
        GameManager.OnPlatformResume -= ResumeMovement;
    }
}