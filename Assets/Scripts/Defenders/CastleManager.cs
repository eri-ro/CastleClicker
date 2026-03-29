using System;
using UnityEngine;
// Castle hit points; fires game over when health reaches zero.
public class CastleManager : MonoBehaviour
{
    public static CastleManager Instance { get; private set; }

    public event Action OnCastleHealthChanged;
    public event Action OnGameOver;

    [Header("Castle Health")]
    public int maxCastleHealth = 100;
    public int currentCastleHealth;
    public bool gameOver;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int bonus = LegacyManager.Instance.GetStartingCastleHealthBonus();
        maxCastleHealth = 100 + bonus;
        currentCastleHealth = maxCastleHealth;
        gameOver = false;
    }

    public void DamageCastle(int amount)
    {
        if (gameOver)
            return;

        currentCastleHealth -= amount;
        currentCastleHealth = Mathf.Max(0, currentCastleHealth);

        OnCastleHealthChanged?.Invoke();

        if (currentCastleHealth <= 0)
            GameOver();
    }

    void GameOver()
    {
        gameOver = true;
        OnGameOver?.Invoke();
        Debug.Log("Game Over");
    }

    public void ResetForNewGame()
    {
        int bonus = LegacyManager.Instance.GetStartingCastleHealthBonus();
        maxCastleHealth = 100 + bonus;
        currentCastleHealth = maxCastleHealth;
        gameOver = false;
        OnCastleHealthChanged?.Invoke();
    }
}
