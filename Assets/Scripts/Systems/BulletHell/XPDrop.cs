using UnityEngine;
using System.Collections;

public class XPDrop : MonoBehaviour
{
    [Header("XP Settings")]
    [SerializeField] private int xpValue = 1;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float collectionRange = 2f;
    [SerializeField] private float lifetime = 10f;

    [Header("Visual Effects")]
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatFrequency = 2f;

    private SpriteRenderer spriteRenderer;
    private Transform player;
    private bool canMoveToPlayer = false;
    private Vector3 startPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        // Find player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
        
       // Start floating animation
        StartCoroutine(FloatAnimation());
                
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Check if player is in range to start moving
        if (player != null && !canMoveToPlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= collectionRange)
            {
                canMoveToPlayer = true;
            }
        }

        // Move towards player if allowed
        if (canMoveToPlayer && player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    IEnumerator FloatAnimation()
    {
        while (true)
        {
            float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = startPosition + new Vector3(0, yOffset, 0);
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectXP(collision.gameObject);
        }
    }

    private void CollectXP(GameObject playerObject)
    {
        // Add XP to player
        PlayerXP playerXP = playerObject.GetComponent<PlayerXP>();
        if (playerXP != null)
        {
            playerXP.AddXP(xpValue);
        }
        else
        {
            Debug.LogWarning("PlayerXP component not found on player!");
        }

        // Visual/audio effects can go here
        Debug.Log($"Collected {xpValue} XP!");

        // Destroy the orb
        Destroy(gameObject);
    }

    // Visualize collection range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}