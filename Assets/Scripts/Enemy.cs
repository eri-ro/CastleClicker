using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Basic,
        FastFlying,
        Tank
    }

    [Header("Type")]
    public EnemyType enemyType = EnemyType.Basic;

    [Header("Movement")]
    public float speed = 1.5f;
    public Transform target;

    [Header("Combat")]
    public int hp = 3;
    public double coinReward = 1;
    public int castleDamage = 1;

    [Header("Status Effects")]
    public float burnDps = 0f;

    [Header("Traits")]
    public bool ignoresMoatSlow = false;
    public bool ignoresMoatBurn = false;

    [Header("Wave")]
    public bool isMassiveWaveEnemy = false;
    public double massiveWaveBonusReward = 2;

    float baseSpeed;
    float speedMultiplier = 1f;
    float burnAccumulator = 0f;

    SpriteRenderer sr;
    Color defaultColor = Color.white;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        baseSpeed = speed;
        ApplyTypeDefaults();
    }

    void Update()
    {
        HandleBehaviorByType();
        HandleBurn();
        UpdateVisuals();
    }

    void ApplyTypeDefaults()
    {
        switch (enemyType)
        {
            case EnemyType.Basic:
                ignoresMoatSlow = false;
                ignoresMoatBurn = false;
                break;

            case EnemyType.FastFlying:
                ignoresMoatSlow = true;
                ignoresMoatBurn = true;
                break;

            case EnemyType.Tank:
                ignoresMoatSlow = false;
                ignoresMoatBurn = false;
                break;
        }
    }

    void HandleBehaviorByType()
    {
        switch (enemyType)
        {
            case EnemyType.Basic:
                MoveTowardTarget();
                break;

            case EnemyType.FastFlying:
                MoveTowardTarget();
                break;

            case EnemyType.Tank:
                MoveTowardTarget();
                break;
        }
    }

    void MoveTowardTarget()
    {
        Vector3 targetPos = target.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        float currentSpeed = baseSpeed * speedMultiplier;
        transform.position += dir * currentSpeed * Time.deltaTime;
    }

    void HandleBurn()
    {
        if (burnDps > 0f)
        {
            burnAccumulator += burnDps * Time.deltaTime;

            int burnDamage = Mathf.FloorToInt(burnAccumulator);
            if (burnDamage > 0)
            {
                burnAccumulator -= burnDamage;
                TakeDamage(burnDamage);
            }
        }
        else
        {
            burnAccumulator = 0f;
        }
    }

    void UpdateVisuals()
    {
        if (burnDps > 0f)
        {
            sr.color = new Color(1f, 0.5f, 0.2f, 1f);
        }
        else if (isMassiveWaveEnemy)
        {
            sr.color = new Color(0.7f, 0.4f, 1f, 1f);
        }
        else
        {
            sr.color = defaultColor;
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        hp -= amount;

        if (hp <= 0)
            Die();
    }

    void Die()
    {
        double reward = coinReward;
        if (isMassiveWaveEnemy)
            reward += massiveWaveBonusReward;

        GameManager.Instance.AddCoins(coinReward);

        Destroy(gameObject);
    }

    public void SetSpeedMultiplier(float mult)
    {
        speedMultiplier = Mathf.Max(0f, mult);
    }

    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1f;
    }

    public void SetBurnDps(float dps)
    {
        burnDps = Mathf.Max(0f, dps);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.DamageCastle(castleDamage);

        Destroy(gameObject);
    }
}