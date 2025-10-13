using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerGuns : MonoBehaviour
{
    enum SpawnerType { Straight, Spin, Everywhere }

    //[SerializeField] GameObject Target;

    [Header("Bullet Attributes")]
    public GameObject bullet;
    public float bulletLife = 1f;
    public float speed = 1f;


    [Header("Spawner Attributes")]
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private float firingRate = 1f;


    private GameObject spawnedBullet;
    private float timer = 0f;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
       
    }


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (spawnerType == SpawnerType.Straight)
        {
            AimAtMouse();
        }

        if (spawnerType == SpawnerType.Spin) transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + 1f);

        //if (spawnerType == SpawnerType.Everywhere) transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + 1f);
        if (timer >= firingRate)
        {
            Fire();
            timer = 0;
        }
    }

    private void AimAtMouse()
    {
        if (mainCamera == null) return;

        // Get mouse position in world coordinates
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Set z to 0 for 2D

        // Calculate direction and angle
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation to face the mouse
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Fire()
    {
        if (bullet)
        {
            spawnedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            spawnedBullet.GetComponent<Bullet>().speed = speed;
            spawnedBullet.GetComponent<Bullet>().bulletLife = bulletLife;
            spawnedBullet.transform.rotation = transform.rotation;
        }
    }
}
