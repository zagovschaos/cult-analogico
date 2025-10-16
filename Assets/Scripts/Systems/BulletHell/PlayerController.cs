using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance { get; private set; }

    [Header("Vida e XP")]
    [SerializeField] private Image xpBar;
    public int xpBarMax;
    public int xpBarCurrent;
    [SerializeField] private Image lifeBar;

    public event Action<float, float> OnChangeHealth;
    public event Action<Vector3> OnTakeDamage;
    public event Action OnHeal;
    public event Action<IDamageable> OnDie;

    [field: SerializeField] public float CurrentHealth { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }
    public bool IsDie { get; set; }


    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float collisionOffset = 0.05f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask collisionLayer;

    private Vector2 movement;
    private bool isLookingLeft = false;

    [SerializeField] private Text lifeText;

    // Add this method to initialize the Singleton
    private void Awake()
    {
         Instance = this;
       
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {

        if (xpBar != null)
        {
            CurrentHealth = MaxHealth;
            SetlifeBar(CurrentHealth, MaxHealth);
        }

    }

    private void OnEnable()
    {

        OnChangeHealth += SetlifeBar;
    }

    private void OnDisable()
    {

        OnChangeHealth -= SetlifeBar;
    }

    public void SetAddXp(int xp)
    {

        xpBarCurrent += xp;
        if (xpBarCurrent >= xpBarMax)
        {
            //GameManager.Instance.PowerUP();
            xpBarCurrent = xpBarMax;
        }

        xpBar.fillAmount = (float)xpBarCurrent / xpBarMax;
    }

    public void SetlifeBar(float currrentHealth, float maxHealth)
    {
        lifeBar.fillAmount = currrentHealth / maxHealth;
        lifeText.text = currrentHealth.ToString();
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

    public void AddMaxHP(float value)
    {
        MaxHealth += value;
        CurrentHealth = MaxHealth;
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
        OnHeal?.Invoke();
    }

    public void Heal(float amount)
    {

        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;

        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
        OnHeal?.Invoke();
    }

    public void Die()
    {
        IsDie = true;
        OnDie?.Invoke(this);
        //GameManager.Instance.GameOver();
    }
    void Update()
    {
        // movimento
        movement.x = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;
        movement.y = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;

       
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        
        if (movement.x < 0 && !isLookingLeft)
        {
            FlipCharacter(true);
        }
        else if (movement.x > 0 && isLookingLeft)
        {
            FlipCharacter(false);
        }
    }

    void FixedUpdate()
    {
 
        if (movement != Vector2.zero)
        {
            Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

            
            if (!CheckCollision(newPosition))
            {
                rb.MovePosition(newPosition);
            }
            else
            {
               
                Vector2 horizontalMove = new Vector2(movement.x, 0);
                if (horizontalMove.magnitude > 0 && !CheckCollision(rb.position + horizontalMove * moveSpeed * Time.fixedDeltaTime))
                {
                    rb.MovePosition(rb.position + horizontalMove * moveSpeed * Time.fixedDeltaTime);
                }

               
                Vector2 verticalMove = new Vector2(0, movement.y);
                if (verticalMove.magnitude > 0 && !CheckCollision(rb.position + verticalMove * moveSpeed * Time.fixedDeltaTime))
                {
                    rb.MovePosition(rb.position + verticalMove * moveSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }

    private bool CheckCollision(Vector2 targetPosition)
    {
       
        RaycastHit2D hit = Physics2D.CircleCast(targetPosition, GetComponent<Collider2D>().bounds.extents.x * 0.8f, Vector2.zero, 0f, collisionLayer);
        return hit.collider != null;
    }

    private void FlipCharacter(bool lookLeft)
    {
        isLookingLeft = lookLeft;
        transform.rotation = Quaternion.Euler(0f, lookLeft ? 180f : 0f, 0f);
    }

   
}