using UnityEngine;
//using NaughtyAttributes;

public class DetectHealthEnemy : MonoBehaviour
{
    public enum DetectionType
    {
        Collision,
        Trigger,
    }

    [SerializeField] private DetectionType type;
    [Tooltip("Quantidade de dano")]
    [SerializeField] private float damage;
    [Tooltip("Quantidade de cura")]
    [SerializeField] private float heal;

 


    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (type != DetectionType.Collision)
            return;

        if (collision.gameObject.TryGetComponent(out IDamageable damageable) || collision.gameObject.CompareTag("Enemy"))
        {
            //damageable.TakeDamage(transform.position, damage);
            //damageable.Heal(heal);
            DestroyObject();
        }

     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bullet"))
            return;

        if (collision.gameObject.TryGetComponent(out IDamageable damageable) || collision.gameObject.CompareTag("Enemy"))
        {
            damageable.TakeDamage(transform.position, damage);
            damageable.Heal(heal);
            DestroyObject();
        }

        
    }

    public void SetChangeDamage(float valor)
    {
        damage += valor;
    }

    private void DestroyObject()
    {
       Destroy(gameObject);
    }
}
