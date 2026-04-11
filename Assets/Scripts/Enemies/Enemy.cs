using UnityEngine;
// Enemy unit: moves toward the castle, can burn, drops coins when killed.
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

    bool appliedStatsFromXmlFile;

    float baseSpeed;
    float speedMultiplier = 1f;
    float burnAccumulator = 0f;

    SpriteRenderer sr;
    Color defaultColor = Color.white;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        defaultColor = sr.color;
    }

    void OnEnable()
    {
        EnemyRegistry.Instance.Register(this);
    }

    void OnDisable()
    {
        EnemyRegistry.Instance.Unregister(this);
    }

    void Start()
    {
        baseSpeed = speed;
        if (!appliedStatsFromXmlFile)
            ApplyTraitDefaultsFromEnum();
    }

    // EnemySpawner calls this after Instantiate; stats come from the XML-loaded dictionary.
    public void ApplyStatsFromXmlFile()
    {
        if (!EnemyTypeStatsRegistry.TryGet(enemyType, out EnemyTypeXmlEntry row))
            return;

        hp = row.hp;
        speed = row.speed;
        coinReward = row.coinReward;
        castleDamage = row.castleDamage;
        ignoresMoatSlow = row.ignoresMoatSlow;
        ignoresMoatBurn = row.ignoresMoatBurn;
        appliedStatsFromXmlFile = true;
    }

    // Flying wave: switch to FastFlying moat flags only; keep current HP/coins (no full XML re-apply).
    public void ApplyMoatTraitsOnlyFromXmlOrEnum()
    {
        if (EnemyTypeStatsRegistry.TryGet(enemyType, out EnemyTypeXmlEntry row))
        {
            ignoresMoatSlow = row.ignoresMoatSlow;
            ignoresMoatBurn = row.ignoresMoatBurn;
        }
        else
            ApplyTraitDefaultsFromEnum();
    }

    void Update()
    {
        if (CastleManager.Instance.gameOver)
            return;

        HandleBehaviorByType();
        HandleBurn();
        UpdateVisuals();
    }

    void ApplyTraitDefaultsFromEnum()
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

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.up = -dir;
        }

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
        TakeDamage(amount, out _, out _);
    }

    public void TakeDamage(int amount, out bool killed, out double goldEarnedIfKill)
    {
        killed = false;
        goldEarnedIfKill = 0;
        if (amount <= 0) return;

        GameStatsTracker.Instance.RecordDamageDealt(amount);

        hp -= amount;

        if (hp <= 0)
        {
            killed = true;
            goldEarnedIfKill = GetCoinRewardValue();
            Die();
        }
    }

    public double GetCoinRewardValue()
    {
        double reward = coinReward;
        if (isMassiveWaveEnemy)
            reward += massiveWaveBonusReward;
        reward *= LegacyManager.Instance.GetEnemyCoinDropMultiplier();
        return reward;
    }

    void Die()
    {
        GameStatsTracker.Instance.RecordEnemyKilled();

        GameManager.Instance.AddCoins(GetCoinRewardValue());
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

        CastleManager.Instance.DamageCastle(castleDamage);
        Destroy(gameObject);
    }
}
