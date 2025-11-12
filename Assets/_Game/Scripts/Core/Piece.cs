using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] int _scoreValue = 1;

    [Header("Landing Zone Settings")]
    [SerializeField] Collider col;

    [Tooltip("Tỷ lệ vùng an toàn (0-1). Có thể bị override bởi LevelManager")]
    [SerializeField] float _safeLandingZoneRatio = 0.7f;

    [Header("Cube ")]
    [SerializeField] public GameObject _cubeObject;

    [Header("Coin Settings")]
    [SerializeField] private GameObject _coinObject;

    [Tooltip("Xác suất xuất hiện coin (0-1). 0.2 = 20% chance")]
    [SerializeField] private float _coinSpawnChance = 0.2f;

    [Header("Heart Settings")]
    [SerializeField] private GameObject _heartObject;

    [Tooltip("Xác suất xuất hiện heart (0-1). 0.15 = 15% chance")]
    [SerializeField] private float _heartSpawnChance = 0.15f;

    [Header("Trap Settings")]
    [SerializeField] private GameObject _trapObject;

    [Tooltip("Xác suất xuất hiện trap/bẫy (0-1). 0.1 = 10% chance")]
    [SerializeField] private float _trapSpawnChance = 0.1f;

    public  bool         isIgnoreTriggerSetScore = false;
    private LevelManager _levelManager;
    private Coin         _coin;
    private Heart        _heart;
    private Trap         _trap;

    // Lưu original scales của pickup objects
    private Vector3 _originalCoinScale;
    private Vector3 _originalHeartScale;
    private Vector3 _originalTrapScale;
    private bool    _originalScalesSaved = false;

    [Header("Visual")]
    [SerializeField] MeshRenderer _renderer;

    [SerializeField] GameData _data;

    [field: SerializeField]
    public float HalfWidth { get; private set; }

    bool _isGameOver      = false;
    bool _playerHasLanded = false; // Track nếu player đã từng landed
    bool _playerHasLeft   = false; // Track nếu player đã rời khỏi thực sự
    bool _isPlayerOnPiece = false; // Track nếu player đang trên piece (collision active)

    // Checkpoint system
    public static bool      IsReviving = false; // Flag để disable scoring khi revive
    private       Coroutine _fallCoroutine; // Lưu reference coroutine để có thể cancel khi revive

    public static event Action            OnGameOver;
    public static event Action<Vector3>   OnLastPieceExit;
    public static event Action<int>       OnGettingScore;
    public static event Action<Transform> OnSafeLanding; // Trigger khi player đáp đúng vùng an toàn → set checkpoint

    private void OnEnable()
    {
        GameManager.OnRevive         += Revive;
        GameManager.OnPlatformFreeze += StopFalling; // Stop coroutine khi freeze
    }

    private void OnDisable()
    {
        GameManager.OnRevive         -= Revive;
        GameManager.OnPlatformFreeze -= StopFalling;
    }

    private void Start()
    {
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
    /// Random spawn Heart, Trap hoặc Coin
    /// Priority: Heart > Trap > Coin (CHỈ 1 trong 3 được spawn)
    /// </summary>
    private void InitializePickups()
    {
        // Lấy components và lưu original scales trước (chỉ lần đầu)
        if (!_originalScalesSaved)
        {
            if (_heartObject != null)
            {
                _heart              = _heartObject.GetComponent<Heart>();
                _originalHeartScale = _heartObject.transform.localScale;
            }

            if (_trapObject != null)
            {
                _trap              = _trapObject.GetComponent<Trap>();
                _originalTrapScale = _trapObject.transform.localScale;
            }

            if (_coinObject != null)
            {
                _coin              = _coinObject.GetComponent<Coin>();
                _originalCoinScale = _coinObject.transform.localScale;
            }

            _originalScalesSaved = true;
        }
        else
        {
            // Nếu đã lưu scales rồi, chỉ lấy components
            if (_heartObject != null && _heart == null)
            {
                _heart = _heartObject.GetComponent<Heart>();
            }

            if (_trapObject != null && _trap == null)
            {
                _trap = _trapObject.GetComponent<Trap>();
            }

            if (_coinObject != null && _coin == null)
            {
                _coin = _coinObject.GetComponent<Coin>();
            }
        }

        // 1. Try spawn Heart trước (priority cao nhất)
        if (_heart != null)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);

            if (randomValue <= _heartSpawnChance)
            {
                // Spawn Heart → Ẩn Trap và Coin
                _heart.Show();
                if (_trap != null) _trap.Hide();
                if (_coin != null) _coin.Hide();

                return; // Dừng luôn
            }
            else
            {
                _heart.Hide();
            }
        }

        // 2. Chỉ đến đây nếu KHÔNG spawn Heart
        // Try spawn Trap (priority trung bình)
        if (_trap != null)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);

            if (randomValue <= _trapSpawnChance)
            {
                // Spawn Trap → Ẩn Coin
                _trap.Show();
                if (_coin != null) _coin.Hide();

                return; // Dừng luôn
            }
            else
            {
                _trap.Hide();
            }
        }

        // 3. Chỉ đến đây nếu KHÔNG có Heart và KHÔNG có Trap
        // Random spawn Coin (priority thấp nhất)
        if (_coin != null)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);

            if (randomValue <= _coinSpawnChance)
            {
                _coin.Show();
            }
            else
            {
                _coin.Hide();
            }
        }
    }

    private void Revive() { _isGameOver = false; }

    /// <summary>
    /// Điều chỉnh scale của pickup objects để chúng scale đều theo tỷ lệ với piece
    /// Pickup objects sẽ scale đều theo tất cả các chiều (X, Y, Z) theo tỷ lệ piece scale
    /// Được gọi từ Platform sau khi piece được scale
    /// </summary>
    public void AdjustPickupScales(float pieceScaleX)
    {
        // Scale pickup objects đều theo tất cả các chiều với tỷ lệ piece scale
        if (_coinObject != null)
        {
            _coinObject.transform.localScale = Vector3.one * pieceScaleX;
        }

        if (_heartObject != null)
        {
            _heartObject.transform.localScale = Vector3.one * pieceScaleX;
        }

        if (_trapObject != null)
        {
            _trapObject.transform.localScale = Vector3.one * pieceScaleX;
        }

        Debug.Log($"[Piece] Scaled pickup objects uniformly by {pieceScaleX:F2} (Original * {pieceScaleX:F2})");
    }

    /// <summary>
    /// Stop platform fall coroutine khi freeze (revive system)
    /// Tránh platform rơi khi player revive về piece này
    /// </summary>
    private void StopFalling()
    {
        if (_fallCoroutine != null)
        {
            StopCoroutine(_fallCoroutine);
            _fallCoroutine = null;

            // Reset flags để có thể reuse piece
            _playerHasLeft = false;
        }
    }

    public void OnCollisionEnterFromChild(Collision c)
    {
        // Gọi lại đúng logic bạn đang có
        HandleCollision(c);
        if (this._coin.gameObject.activeSelf) this._coin.CollectCoin();
        if (this._heart.gameObject.activeSelf) this._heart.CollectHeart();
        if (this._trap.gameObject.activeSelf) this._trap.TriggerTrap();
        

    }

    private void HandleCollision(Collision c)
    {
        // Đánh dấu player đã landed và đang trên piece
        _playerHasLanded = true;
        _isPlayerOnPiece = true;

        // Nếu đang reviving → Resume platforms và skip scoring
        if (IsReviving)
        {
            IsReviving = false;

            // Resume platforms thông qua GameManager
            GameManager gameManager = FindFirstObjectByType<GameManager>();

            if (gameManager != null)
            {
                gameManager.ResumePlatformsFromRevival();
            }

            SoundController.GetInstance().PlayAudio(AudioType.LANDING);

            return; // Skip tất cả logic scoring/coin/heart
        }

        // Tính khoảng cách từ player đến tâm của piece
        float xDistanceToCenter = Mathf.Abs(c.transform.position.x - col.bounds.center.x);

        // Tính vùng an toàn (giữa piece)
        float safeZoneWidth = col.bounds.size.x * 0.5f;

        // Nếu đáp xa tâm (ngoài vùng an toàn) = đáp lệch mép → Mất health
        if (xDistanceToCenter > safeZoneWidth)
        {
            // Trigger event để GameManager xử lý (mất health)
            OnGameOver?.Invoke();
        }
        else
        {
            // Đáp chính xác vào vùng an toàn = set checkpoint + được điểm
            Debug.Log("[Piece] Safe landing!");
            OnSafeLanding?.Invoke(transform);

            if (!isIgnoreTriggerSetScore)
            {
                // Cộng điểm
                OnGettingScore?.Invoke(_scoreValue);
                LeanTween.moveLocalY(gameObject, -.3f, .2f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1);

                // Spawn floating score
                FloatingScore floatingScore = Instantiate(_data.GetFloatingScore);
                floatingScore.Spawn(transform.position, _scoreValue);
                Destroy(floatingScore.gameObject, 1f);
            }
        }

        SoundController.GetInstance().PlayAudio(AudioType.LANDING);
    }
    
    public void OnCollisionStayFromChild(Collision c)
    {
        HandleCollisionStay(c);
    }

    private void HandleCollisionStay(Collision c)
    {
        // Player vẫn đang chạm piece
        _isPlayerOnPiece = true;
    }

    public void OnCollisionExitFromChild(Collision c)
    {
        HandleCollisionExit(c);
    }

    private void HandleCollisionExit(Collision c)
    {
        // Đánh dấu player không còn chạm piece
        _isPlayerOnPiece = false;

        // Chỉ cho platform rơi nếu:
        // 1. Không phải game over
        // 2. Player đã từng landed trên piece này
        // 3. Chưa rơi trước đó
        if (!_isGameOver && _playerHasLanded && !_playerHasLeft)
        {
            _playerHasLeft = true;

            // Delay để đảm bảo player đã nhảy xa thực sự
            _fallCoroutine = StartCoroutine(CheckAndFallPlatform());

            OnLastPieceExit?.Invoke(transform.position);
        }
    }

    IEnumerator CheckAndFallPlatform()
    {
        // Tìm PlayerMovement để check xem player có đang jump không
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

        // Đợi 0.1s để check xem collision có quay lại không (do unparent)
        yield return new WaitForSeconds(0.1f);

        // KIỂM TRA: Player có thực sự đang jumping không?
        bool playerIsJumping = playerMovement != null && playerMovement.IsJumping;

        // Nếu sau 0.1s player vẫn không chạm lại VÀ đang jumping = đã nhảy xa thực sự
        if (!_isPlayerOnPiece && playerIsJumping)
        {
            yield return new WaitForSeconds(0.4f); // Delay thêm trước khi rơi
            yield return new WaitUntil(() => CheckpointManager.GetInstance().CheckpointPiece != transform); // Đợi đến khi checkpoint không phải piece này

            if (transform.gameObject.name != "Base")
            {
                transform.parent.parent.gameObject.AddComponent<Rigidbody>();
                Destroy(transform.parent.parent.gameObject, 2f);
            }

            _fallCoroutine = null; // Cleanup coroutine reference
        }
        else
        {
            _playerHasLeft = false; // Reset flag
            _fallCoroutine = null; // Cleanup coroutine reference
        }
    }
}