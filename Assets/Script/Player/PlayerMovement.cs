using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]  // empeche les doublons de component 
[RequireComponent(typeof(Rigidbody2D))] // si le component rigidbody est oublier pas de probleme 
[RequireComponent(typeof(PlayerStats))] // si le component playerstats est oublier pas de probleme 
public class PlayerMovement : MonoBehaviour
{
    private PlayerStats playerStats;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (playerStats.IsDead)
            return;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement.Normalize();
    }

    private void FixedUpdate()
    {
        if (playerStats.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = movement * playerStats.MoveSpeed;
    }
}