using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1.5f;
    public int hp = 3;
    public int coinReward = 1;
    public Transform target;

    void Update()
    {
        Vector3 targetPos = target.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(coinReward);

        Destroy(gameObject);
    }
}