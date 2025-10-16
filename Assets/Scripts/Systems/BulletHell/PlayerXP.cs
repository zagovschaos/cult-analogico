using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class PlayerXP : MonoBehaviour
{
    [Header("XP Settings")]
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel = 100;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float xpMultiplier = 1.2f; // XP required multiplier per level

    [Header("UI References")]
    [SerializeField] private Image xpFillImage; // Image component with Image Type = Filled
    [SerializeField] private Text levelText; // Optional: if you want to show level text separatel

    [Header("Events")]
    public UnityEvent<int> OnXPAdded; // Parameter: XP amount
    public UnityEvent<int> OnLevelUp; // Parameter: new level

    void Start()
    {
        UpdateUI();
    }

    public void AddXP(int xpAmount)
    {
        currentXP += xpAmount;

        // Trigger XP added event
        OnXPAdded?.Invoke(xpAmount);

        // Check for level up
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;

        // Increase XP required for next level
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);

        // Trigger level up event
        OnLevelUp?.Invoke(currentLevel);

        Debug.Log($"Level Up! Now level {currentLevel}. Next level in {xpToNextLevel} XP");

        // Here you can add level up benefits like:
        // - Increase player stats
        // - Unlock new abilities
        // - Heal player
    }

    private void UpdateUI()
    {
        // Update XP fill amount (0 to 1)
        if (xpFillImage != null)
        {
            xpFillImage.fillAmount = (float)currentXP / xpToNextLevel;
        }

        // Update level text if assigned
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
        }
    }

    // For saving/loading game state
    public void SetXP(int xp, int level, int nextLevelXP)
    {
        currentXP = xp;
        currentLevel = level;
        xpToNextLevel = nextLevelXP;
        UpdateUI();
    }

    // Getters for other scripts
    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentXP() => currentXP;
    public int GetXPToNextLevel() => xpToNextLevel;

    public float GetXPFillPercentage()
    {
        return (float)currentXP / xpToNextLevel;
    }
}