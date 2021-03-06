﻿using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]

    [SerializeField] float currentHealth = 500;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.75f;
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] int enemyKillPoints = 150;
    [SerializeField] GameObject healthPack;
    [SerializeField] [Range(1, 100)] float chancesToDropPowerUp = 40f;

    [Header("Enemy Laser")]

    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] AudioClip laserSound;
    [SerializeField] [Range(0, 1)] float laserVolume = 0.5f;


    [Header("Enemy  LevelUp")]

    [SerializeField] int[] wavesClearedRequired;
    private int shipLevel;
    EnemySpawner enemySpawner;
    [SerializeField] Sprite[] enemySprites;
    [SerializeField] float maxTimeBetweenShotsBonus=-0.1f;
    [SerializeField] float healthBonus= 10f;
    [SerializeField] float projectileSpeedBonus=0.1f;
    [SerializeField] int numofEnemiesBonus=1;



    // Start is called before the first frame update
    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        CheckForWavesCleared();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if(shotCounter<=0f)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }
    private void Fire()
    {

            GameObject laser = Instantiate(laserPrefab,
                transform.position,
                Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
            AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserVolume);
    }


    private void OnTriggerEnter2D(Collider2D other)

    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);

    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        currentHealth -= damageDealer.GetDamage();
        damageDealer.Hit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        RollForPowerUp();
        FindObjectOfType<GameSession>().AddToScore(enemyKillPoints);
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
        Destroy(explosion, durationOfExplosion);
    }

    public void RollForPowerUp()
    {
        int roll = UnityEngine.Random.Range(1, 101);
        if(roll<=chancesToDropPowerUp)
        {
            GameObject newHealthPack= Instantiate(healthPack, transform.position, Quaternion.identity) as GameObject;

        }
    }

    public void CheckForWavesCleared()
    {
        int numWavesCleared = enemySpawner.numWavesCleared;
        int maxWavesClearedRequired = wavesClearedRequired[wavesClearedRequired.Length -1];
        Debug.Log(maxWavesClearedRequired);

        if (numWavesCleared >= maxWavesClearedRequired)
        {
            shipLevel =wavesClearedRequired.Length;
            Debug.Log("Enemy Ship Level is  : " + shipLevel);
            Debug.Log("health is " + currentHealth + " at ShipLevel " + shipLevel);
            HandleShipLevel(shipLevel);
        }
        else
        {
            for (int i = 0; numWavesCleared >= wavesClearedRequired[i]; i++)
            {
                shipLevel = i + 1;
                Debug.Log("Number of Waves Cleared Required Is " + wavesClearedRequired[i] + "at position " + i);
            }
            Debug.Log("Enemy Ship Level is  : " + shipLevel);
            HandleShipLevel(shipLevel);
        }
    }

    public void HandleShipLevel(int shipLevel)
    {
        if (shipLevel != 0)
        {
            maxTimeBetweenShots += maxTimeBetweenShotsBonus * shipLevel;
            projectileSpeed += projectileSpeedBonus * shipLevel;
            currentHealth = currentHealth +  healthBonus * shipLevel;
                Debug.Log("maxTimeBetweenShots is " + maxTimeBetweenShots + " at ShipLevel " + shipLevel);
            Debug.Log("projectileSpeed is " + projectileSpeed + " at ShipLevel " + shipLevel);
            Debug.Log("health is " + currentHealth + " at ShipLevel " + shipLevel);
            this.GetComponent<SpriteRenderer>().sprite = enemySprites[shipLevel - 1];
        }

       

    }
}
 