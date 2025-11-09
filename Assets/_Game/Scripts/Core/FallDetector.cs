namespace _Game.Scripts.Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Plane kiểm tra nếu player rơi xuống = Game Over
    /// Gắn script này vào plane ở dưới màn chơi
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FallDetector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string _playerTag = "Player";

        [SerializeField] private bool _showDebugGizmo = true;
        
        public static event Action OnPlayerFell;

        private void Awake()
        {
            // Đảm bảo collider là trigger
            Collider col = GetComponent<Collider>();

            if (col != null)
            {
                col.isTrigger = true;
            }
            else
            {
                Debug.LogError("[FallDetector] Missing Collider component!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Kiểm tra nếu là player
            if (other.CompareTag(_playerTag))
            {
                Debug.Log("[FallDetector] Player fell off the platform!");
                OnPlayerFell?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            if (!_showDebugGizmo) return;

            // Vẽ plane màu đỏ trong Scene view để dễ nhìn
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}