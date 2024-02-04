using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int health = 50;
    [SerializeField] int maxHealth = 50;
    [SerializeField] int meleeDamage = 5;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float interactRange;
    List<string> inventory = new List<string>();

    [Header("Combat")]
    [SerializeField] float attackRange;
    [SerializeField] float attackRate;
    float lastAttackTime;

    // This is just an example, using a animation controller is much better.
    [Header("Sprites")]
    [SerializeField] Sprite leftSprite;
    [SerializeField] Sprite rightSprite;
    [SerializeField] Sprite upSprite;
    [SerializeField] Sprite downSprite;

    [Header("Experience")]
    [SerializeField] int currentLevel = 0;
    [SerializeField] int currentExperience = 0;
    [SerializeField] int experienceGoalForNextLevel = 5;
    [SerializeField] float experienceModifier = 1.5f;

    float xAxisInput;
    float yAxisInput;
    Vector2 direction;

    Rigidbody2D _rb;
    SpriteRenderer _sr;
    ParticleSystem _hitEffect;
    PlayerUI _ui;
    bool _isFire1InUse;
    bool _isFire2InUse;

    public int GetCurrentLevel() { return currentLevel; }
    public int GetCurrentExperience() { return currentExperience; }
    public int GetExperienceGoalForNextLevel() { return experienceGoalForNextLevel; }
    public int GetHealth() { return health; }
    public int GetMaxHealth() { return maxHealth; }
    public List<string> GetInventory() { return inventory; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        _ui = FindAnyObjectByType<PlayerUI>();

        if (_hitEffect == null)
        {
            Debug.LogError("A hit effect is required");
        }
    }

    private void Start()
    {
        _ui.UpdateLevelText();
        _ui.UpdateInventoryText();
        _ui.UpdateHealthBar();
        _ui.UpdateExperienceBar();
    }

    private void Update()
    {
        ReadInput();
        CheckInteract();
    }

    private void ReadInput()
    {
        xAxisInput = Input.GetAxisRaw("Horizontal");
        yAxisInput = Input.GetAxisRaw("Vertical");

        if (Input.GetAxisRaw("Fire1") != 0)
        {
            if (_isFire1InUse == false)
            {
                _isFire1InUse = true;
                AttackEnemyWithinRange();
            }
        } 
        else
        {
            _isFire1InUse = false;
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 velocity = new Vector2(xAxisInput, yAxisInput);

        if (velocity.magnitude != 0)
        {
            direction = velocity.normalized;
        }

        UpdateSpriteDirection();

        _rb.velocity = velocity.normalized * movementSpeed;
    }

    // This is just an example, using a animation controller is much better
    void UpdateSpriteDirection()
    {
        if (xAxisInput < 0)
        {
            _sr.sprite = leftSprite;
        } 
        else if (xAxisInput > 0)
        {
            _sr.sprite = rightSprite;
        } 
        else if (yAxisInput > 0)
        {
            _sr.sprite = upSprite;
        } 
        else if (yAxisInput < 0)
        {
            _sr.sprite = downSprite;
        }
    }

    void AttackEnemyWithinRange()
    {
        if (Time.time - lastAttackTime >= attackRate)
        {
            lastAttackTime = Time.time;
            LayerMask layerMask = LayerMask.GetMask("Enemy");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, layerMask);
            Debug.DrawRay(transform.position, direction * attackRange, Color.red, 1f);


            if (hit.collider != null)
            {
                hit.collider.GetComponent<Enemy>().TakeDamage(meleeDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Ouch!");
        _hitEffect.Play();

        health -= damage;

        if (health <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }

        _ui.UpdateHealthBar();
    }

    public void AddExperience(int experience)
    {
        currentExperience += experience;
        

        if (currentExperience >= experienceGoalForNextLevel)
        {
            LevelUp();
        }

        _ui.UpdateExperienceBar();
    }

    public void LevelUp() 
    {
        currentLevel++;
        currentExperience -= experienceGoalForNextLevel;
        experienceGoalForNextLevel = Mathf.RoundToInt((float)experienceGoalForNextLevel * experienceModifier);
        _ui.UpdateLevelText();
    }

    void CheckInteract() 
    {
        LayerMask layerMask = LayerMask.GetMask("Interactable");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, interactRange, layerMask);

        if (hit.collider != null)
        {
            Interactable item = hit.collider.GetComponent<Interactable>();
            _ui.SetInteractText(item.gameObject.transform.position, item.InteractDescription());

            if (Input.GetKeyDown(KeyCode.E))
            {
                item.Interact();
            }

        } else
        {
            _ui.DisableInteractText();
        }
    }

    public void AddToInventory(string itemName)
    {
        inventory.Add(itemName);
        _ui.UpdateInventoryText();
    }

    public void RemoveFromInventory(string itemName)
    {
        foreach (string item in inventory)
        {
            if (item.Equals(itemName))
            { 
                inventory.Remove(item); 
            }
        }

        _ui.UpdateInventoryText();
    }
}
