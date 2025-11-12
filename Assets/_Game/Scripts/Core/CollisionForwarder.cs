using UnityEngine;

public class CollisionForwarder : MonoBehaviour
{
    private Piece _parentPiece;

    private void Awake()
    {
        _parentPiece = GetComponentInParent<Piece>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Forward va chạm lên cha
        if (_parentPiece != null)
        {
            _parentPiece.OnCollisionEnterFromChild(collision);
        }
    }
}