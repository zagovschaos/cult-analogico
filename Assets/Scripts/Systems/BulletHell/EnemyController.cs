using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    //[SerializeField] private GameObject bulletPrefab;
    [SerializeField] Transform Player;

    public event Action<IDamageable> OnDie;
    [SerializeField] float speed;
    [SerializeField] private float distanceToPlayer;
    private bool canHit = true;
    [SerializeField] private bool isLookingLeft;

    [SerializeField] private float damage = 1;
    [SerializeField] private float timeToAttack = 0.5f;


   
    public void Attack()
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - transform.position;
    }



    private void Update()
    {
        Flip();

        if (Vector3.Distance(transform.position, Player.position) < distanceToPlayer)
        {
            //perto da arvore
            if (canHit)
            {
                PlayerController.Instance.TakeDamage(transform.position, damage);
                canHit = false;
                //StartCoroutine(WaitForHit());
            }

            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, Player.position, speed * Time.deltaTime);
    }


    //private IEnumerator WaitForHit()
   // {
    //    yield return new WaitForSeconds(timeToAttack);
   //     canHit = true;
  //  }

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



}

