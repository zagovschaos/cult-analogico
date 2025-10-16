using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;

public class EnemyController : MonoBehaviour, IDamageable
{
    
    private Transform Player;

    [field: SerializeField] public float CurrentHealth { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }
    public bool IsDie { get; set; }

    public event Action<IDamageable> OnDie;
    public event Action<float, float> OnChangeHealth;
    public event Action<Vector3> OnTakeDamage;
    public event Action OnHeal;

    [SerializeField] float speed;
    [SerializeField] private float distanceToPlayer;
    private bool canHit = true;
    [SerializeField] private bool isLookingLeft;

    [SerializeField] private float damage = 1;
    [SerializeField] private float timeToAttack = 0.5f;

    [Header("XP Drop Settings")]
    [SerializeField] private GameObject xpOrbPrefab;
    [SerializeField] private int minXP = 1;
    [SerializeField] private int maxXP = 3;
    [SerializeField] private int xpOrbCount = 1; // Number of orbs to drop


    private void Start()
    {
        
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player = playerObject.transform;
        }
        
    }


    public void Attack()
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - transform.position;
    }



    private void Update()
    {
        Flip();

        if (Vector3.Distance(transform.position, Player.position) < distanceToPlayer)
        {
            
            if (canHit)
            {
                PlayerController.Instance.TakeDamage(transform.position, damage);
                canHit = false;
                
            }

            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, Player.position, speed * Time.deltaTime);
    }



    private void Flip()
    {
        if (transform.position.x > Player.position.x && isLookingLeft)
        {
            isLookingLeft = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (transform.position.x < Player.position.x && !isLookingLeft)
        {
            isLookingLeft = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    }

    public void TakeDamage(Vector3 direction, float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
        {
            Die();
            return;
        }

        OnTakeDamage?.Invoke(direction);
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
    }

    public void Heal(float amount)
    {
        return;
    }

    public void Die()
    {
        DropXP();
        EnemySpawner.Instance.OnEnemyDeath();
        Destroy(gameObject);
    }

    private void DropXP()
    {
        for (int i = 0; i < xpOrbCount; i++)
        {
            // Calculate random XP value for this orb
            int xpValue = UnityEngine.Random.Range(minXP, maxXP + 1);

            // Create XP orb
            GameObject xpOrb = Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);

            // Set XP value
            XPDrop orbComponent = xpOrb.GetComponent<XPDrop>();
            if (orbComponent != null)
            {
                // If you want to set XP value dynamically, you'd need to modify XPOrb script
                // For now, the XP value is set in the prefab
            }

            // Add slight random offset for multiple orbs
            if (xpOrbCount > 1)
            {
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-0.5f, 0.5f),
                    UnityEngine.Random.Range(-0.5f, 0.5f),
                    0
                );
                xpOrb.transform.position += randomOffset;
            }
        }
    }
}

