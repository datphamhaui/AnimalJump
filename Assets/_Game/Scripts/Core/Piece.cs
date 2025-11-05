using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] int _scoreValue = 1;

    [Header("Landing Zone Settings")]
    [SerializeField] Collider col;
    [Tooltip("Tỷ lệ vùng an toàn (0-1). Có thể bị override bởi LevelManager")]
    [SerializeField] float _safeLandingZoneRatio = 0.7f;

    [Header("Coin Settings")]
    [SerializeField] private GameObject _coinObject;
    [Tooltip("Xác suất xuất hiện coin (0-1). 0.2 = 20% chance")]
    [SerializeField] private float _coinSpawnChance = 0.2f;

    [Header("Heart Settings")]
    [SerializeField] private GameObject _heartObject;
    [Tooltip("Xác suất xuất hiện heart (0-1). 0.15 = 15% chance")]
    [SerializeField] private float _heartSpawnChance = 0.15f;

    private LevelManager _levelManager;
    private Coin _coin;
    private Heart _heart;

    [Header("Visual")]
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] GameData     _data;

    [field: SerializeField]
    public float HalfWidth { get; private set; }

    bool _isGameOver = false;
    bool _playerHasLanded = false; // Track nếu player đã từng landed
    bool _playerHasLeft = false;   // Track nếu player đã rời khỏi thực sự
    bool _isPlayerOnPiece = false; // Track nếu player đang trên piece (collision active)

    public static event Action          OnGameOver;
    public static event Action<Vector3> OnLastPieceExit;
    public static event Action<int>     OnGettingScore;

    private void OnEnable() { GameManager.OnRevive += Revive; }

    private void OnDisable() { GameManager.OnRevive -= Revive; }

    private void Start() 
    { 
        _renderer.material = _data.GetRandomMaterial;

        // Lấy safe landing zone từ LevelManager
        _levelManager = FindFirstObjectByType<LevelManager>();
        if (_levelManager != null)
        {
            _safeLandingZoneRatio = _levelManager.GetSafeLandingZoneRatio();
        }

        // Random spawn pickups (chỉ spawn Heart HOẶC Coin, không cả hai)
        InitializePickups();
    }

    /// <summary>
    /// Random spawn Heart hoặc Coin (ưu tiên Heart nếu cả hai đều trúng)
    /// CHỈ 1 trong 2 được hiển thị, không bao giờ cả hai cùng lúc
    /// </summary>
    private void InitializePickups()
    {
        bool heartSpawned = false;

        // Lấy components trước
        if (_heartObject != null)
        {
            _heart = _heartObject.GetComponent<Heart>();
        }
        
        if (_coinObject != null)
        {
            _coin = _coinObject.GetComponent<Coin>();
        }

        // Try spawn Heart trước (ưu tiên cao hơn)
        if (_heart != null)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            if (randomValue <= _heartSpawnChance)
            {
                // Spawn Heart → ẨN Coin
                _heart.Show();
                if (_coin != null) _coin.Hide();
                
                heartSpawned = true;
                Debug.Log($"[Piece {gameObject.name}] ❤️ Heart spawned! Coin hidden. (chance: {randomValue:F2})");
                return; // Dừng luôn, không spawn coin
            }
            else
            {
                _heart.Hide();
            }
        }

        // Chỉ đến đây nếu KHÔNG có Heart
        // Random spawn Coin
        if (_coin != null)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            if (randomValue <= _coinSpawnChance)
            {
                _coin.Show();
                Debug.Log($"[Piece {gameObject.name}] 🪙 Coin spawned! (chance: {randomValue:F2})");
            }
            else
            {
                _coin.Hide();
                Debug.Log($"[Piece {gameObject.name}] No pickup spawned");
            }
        }
        else
        {
            Debug.Log($"[Piece {gameObject.name}] No pickup spawned (no heart)");
        }
    }

    private void Revive() { _isGameOver = false; }

    private void OnCollisionEnter(Collision c)
    {
        // Đánh dấu player đã landed và đang trên piece
        _playerHasLanded = true;
        _isPlayerOnPiece = true;

        Debug.Log($"[Piece {gameObject.name}] ========== COLLISION ENTER ==========");
        Debug.Log($"[Piece {gameObject.name}] Player landed! _playerHasLanded={_playerHasLanded}, _isPlayerOnPiece={_isPlayerOnPiece}");

        // Tính khoảng cách từ player đến tâm của piece
        float xDistanceToCenter = Mathf.Abs(c.transform.position.x - transform.position.x);

        // Tính vùng an toàn (giữa piece)
        float safeZoneWidth = col.bounds.size.x * _safeLandingZoneRatio * 0.5f;

        Debug.Log($"[Piece {gameObject.name}] Distance to center: {xDistanceToCenter:F2} | Safe zone: {safeZoneWidth:F2}");

        // Nếu đáp xa tâm (ngoài vùng an toàn) = đáp lệch mép → Mất health
        if (xDistanceToCenter > safeZoneWidth)
        {
            Debug.Log($"[Piece {gameObject.name}] ⚠️ Landed on edge - Player loses health!");

            // Trigger event để GameManager xử lý (mất health)
            OnGameOver?.Invoke();

            // CHỈ rơi nếu hết health (GameManager sẽ xử lý việc này)
            // Không detach/rơi ngay ở đây nữa
        }
        else
        {
            // Đáp chính xác vào vùng an toàn = được điểm
            OnGettingScore?.Invoke(_scoreValue);
            LeanTween.moveLocalY(gameObject, -.3f, .2f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1);

            // Spawn floating score
            FloatingScore floatingScore = Instantiate(_data.GetFloatingScore);
            floatingScore.Spawn(transform.position, _scoreValue);
            Destroy(floatingScore.gameObject, 1f);

            Debug.Log($"[Piece {gameObject.name}] ✅ Success! Score +{_scoreValue}");
        }

        SoundController.GetInstance().PlayAudio(AudioType.LANDING);
        Debug.Log($"[Piece {gameObject.name}] ========================================\n");
    }

    private void OnCollisionStay(Collision c)
    {
        // Player vẫn đang chạm piece
        _isPlayerOnPiece = true;
        Debug.Log($"[Piece {gameObject.name}] 🔄 COLLISION STAY - _isPlayerOnPiece={_isPlayerOnPiece}");
    }

    private void OnCollisionExit(Collision c)
    {
        Debug.Log($"[Piece {gameObject.name}] ========== COLLISION EXIT ==========");
        Debug.Log($"[Piece {gameObject.name}] Before exit - _isPlayerOnPiece={_isPlayerOnPiece}, _playerHasLanded={_playerHasLanded}, _playerHasLeft={_playerHasLeft}");
        
        // Đánh dấu player không còn chạm piece
        _isPlayerOnPiece = false;

        // Chỉ cho platform rơi nếu:
        // 1. Không phải game over
        // 2. Player đã từng landed trên piece này
        // 3. Chưa rơi trước đó
        if (!_isGameOver && _playerHasLanded && !_playerHasLeft)
        {
            _playerHasLeft = true;
            
            Debug.Log($"[Piece {gameObject.name}] 🚀 Player exit detected - Starting CheckAndFallPlatform coroutine");
            
            // Delay để đảm bảo player đã nhảy xa thực sự
            StartCoroutine(CheckAndFallPlatform());

            OnLastPieceExit?.Invoke(transform.position);
        }
        else
        {
            Debug.Log($"[Piece {gameObject.name}] ⚠️ Exit ignored - _isGameOver={_isGameOver}, _playerHasLanded={_playerHasLanded}, _playerHasLeft={_playerHasLeft}");
        }
        
        Debug.Log($"[Piece {gameObject.name}] After exit - _isPlayerOnPiece={_isPlayerOnPiece}");
        Debug.Log($"[Piece {gameObject.name}] ========================================\n");
    }

    IEnumerator CheckAndFallPlatform()
    {
        Debug.Log($"[Piece {gameObject.name}] ⏱️ CheckAndFallPlatform started - Waiting 0.1s...");
        
        // Tìm PlayerMovement để check xem player có đang jump không
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        
        // Đợi 0.1s để check xem collision có quay lại không (do unparent)
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"[Piece {gameObject.name}] ⏱️ After 0.1s - _isPlayerOnPiece={_isPlayerOnPiece}");
        
        // KIỂM TRA: Player có thực sự đang jumping không?
        bool playerIsJumping = playerMovement != null && playerMovement.IsJumping;
        Debug.Log($"[Piece {gameObject.name}] 🎯 Player IsJumping={playerIsJumping}");

        // Nếu sau 0.1s player vẫn không chạm lại VÀ đang jumping = đã nhảy xa thực sự
        if (!_isPlayerOnPiece && playerIsJumping)
        {
            Debug.Log($"[Piece {gameObject.name}] 💥 Player has LEFT! Platform will fall in 0.4s...");
            
            yield return new WaitForSeconds(0.4f); // Delay thêm trước khi rơi
            
            Debug.Log($"[Piece {gameObject.name}] 🌊 PLATFORM FALLING NOW!");
            transform.parent.parent.gameObject.AddComponent<Rigidbody>();
            Destroy(transform.parent.parent.gameObject, 2f);
        }
        else
        {
            Debug.Log($"[Piece {gameObject.name}] ⚠️ FALSE ALARM - Player still on piece or not jumping! Resetting _playerHasLeft");
            _playerHasLeft = false; // Reset flag
        }
    }

}
