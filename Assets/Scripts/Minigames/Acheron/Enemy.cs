using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private protected Player player;

    [SerializeField] private protected int damage;
    [SerializeField] private protected float moveSpeed;

    [SerializeField] private protected SpriteRenderer spriteRenderer;
    [SerializeField] private protected Rigidbody2D rb2d;
}
