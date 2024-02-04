using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI inventoryText;
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] Image HealthBar;
    [SerializeField] Image ExperienceBar;

    Player _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
    }

    public void UpdateLevelText()
    {
        levelText.text = "Lvl\n " + _player.GetCurrentLevel();
    }

    public void UpdateInventoryText()
    {
        inventoryText.text = "";

        foreach(string item in _player.GetInventory())
        {
            inventoryText.text += item + "\n";
        }
    }

    public void UpdateExperienceBar() 
    {
        ExperienceBar.fillAmount = (float)_player.GetCurrentExperience() / (float)_player.GetExperienceGoalForNextLevel();
    }

    public void UpdateHealthBar()
    {
        HealthBar.fillAmount = (float)_player.GetHealth() / (float)_player.GetMaxHealth();
    }

    public void SetInteractText(Vector3 position, string text)
    {
        interactText.gameObject.SetActive(true);
        interactText.text = text;
        interactText.transform.position = Camera.main.WorldToScreenPoint(position + Vector3.up);
    }

    public void DisableInteractText()
    {
        interactText.gameObject.SetActive(false);
    }
}
