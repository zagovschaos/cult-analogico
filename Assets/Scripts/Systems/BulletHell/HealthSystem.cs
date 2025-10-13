using System;
using System.ComponentModel;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    //[field:] serve pra deixar variaveis que tem esse { get; set; } no final, visiveis no editor;
    [field: SerializeField] public float CurrentHealth { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }
    public bool IsDead { get; set; }

    //diz que esse carinha alterou a vida (dano ou cura), e passa a vida atual
    public event Action<float, float> OnChangeHealth;
    //avisa que o gameObject foi morto
    public event Action<IDamageable> OnDie;
    public event Action<Vector3> OnTakeDamage;
    public event Action OnHeal;

    [SerializeField] private bool destroyOnDie;

    [SerializeField] private int enemyXp;
    public Collider2D collider2d;


   
    void Start()
    {
        collider2d = GetComponent<Collider2D>();
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
    }

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
        collider2d.enabled = true;
    }

    public void TakeDamage(Vector3 direction, float damage)
    {
        if (damage <= 0)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth < 0)
        {
            Die();
            
            return;
        }

        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
        OnTakeDamage?.Invoke(direction);
    }


    public void Die()
    {
        if (IsDead)
            return;
        IsDead = true;
        collider2d.enabled = false;
        PlayerController.Instance.SetAddXp(enemyXp);
        //OnDie?.Invoke(this);


       // if (destroyOnDie)
        //{
       // }//evita que o player seja destruido
         //gameObject.SetActive(false);

       
    }

    public void Heal(float amount)
    {
        if (amount <= 0)
            return;

        CurrentHealth += amount;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;

        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);
        OnHeal?.Invoke();
    }
}
