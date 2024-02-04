using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int health = 10;
    [SerializeField] int maxHealth = 10;
    [SerializeField] float movementSpeed = 1.5f;
    [SerializeField] int experienceToGive = 10;

    [Header("Target")]
    [SerializeField] LayerMask layerMask;
    [SerializeField] float chaseRange = 5f;
    Vector2 targetDirection;
    Player _player;

    [Header("Attack")]
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackRate = 1f;
    float lastAttackTime;

    Rigidbody2D _rb;
    SpriteRenderer _sr;
    ParticleSystem _hitEffect;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _player = FindAnyObjectByType<Player>();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();

        if ( _player == null )
        {
            Debug.LogError("A player is required");
        }

        if (_hitEffect == null )
        {
            Debug.LogError("A hit effect is required");
        }
    }

    private void Update()
    {
        float playerDistance = Vector2.Distance(transform.position, _player.transform.position);

        if (playerDistance <= attackRange) 
        {
            AttackPlayer();
            _rb.velocity = Vector2.zero;
        } 
        else if (playerDistance <= chaseRange)
        {
            ChaseTarget();
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }

    void ChaseTarget()
    {
        targetDirection = (_player.transform.position - transform.position).normalized;

        _rb.velocity = targetDirection * movementSpeed;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Ouch!");
        _hitEffect.Play();

        health -= damage;

        if (health <= 0)
        {
            _player.AddExperience(experienceToGive);
            Destroy(gameObject);
        }
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackRate)
        {
            lastAttackTime = Time.time;
            _player.TakeDamage(attackDamage);
        }
    }
}
