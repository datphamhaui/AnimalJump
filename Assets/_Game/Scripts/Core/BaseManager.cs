namespace _Game.Scripts.Core
{
    using UnityEngine;

    public class BaseManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] _pieceBase;

        public void InitializeBasePieces(int selectedIndex)
        {
            for (int i = 0; i < _pieceBase.Length; i++)
            {
                if (i == selectedIndex)
                {
                    _pieceBase[i].SetActive(true);
                    Debug.Log($"[BaseManager] Activated base piece: {_pieceBase[i].name}");
                }
                else
                {
                    _pieceBase[i].SetActive(false);
                }
            }
        }
    }
}