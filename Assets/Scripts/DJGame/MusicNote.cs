using UnityEngine;

public class MusicNote : MonoBehaviour
{
    [SerializeField]SpriteRenderer spriteRenderer;
    private DJMiniGame minigame;
    float moveSpeed;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        rb.MovePosition(transform.position + Vector3.left * (moveSpeed * Time.deltaTime));
    }

    public void Initialize(float noteSpeed, Color color,DJMiniGame game)
    {
        minigame= game;
        spriteRenderer.color = color;
        moveSpeed = noteSpeed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.ObstacleTag))
        {
            minigame.MusiNoteMissed();
            Destroy(gameObject);
        }
    }
}
