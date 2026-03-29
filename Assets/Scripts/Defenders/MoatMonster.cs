using UnityEngine;
// Orbiting ally that damages enemies on contact; damage scales with upgrades.
public class MoatMonster : MonoBehaviour
{
    [Header("Orbit")]
    public Transform center;
    public float orbitRadius = 1.8f;
    public float orbitSpeed = Mathf.PI / 2f;
    public float angle = 0f;

    [Header("Combat")]
    public int contactDamage = 1;
    public float hitCooldown = 0.4f;

    float hitTimer = 0f;

    void Update()
    {
        UpdateOrbit();

        if (hitTimer > 0f)
        {
            hitTimer -= Time.deltaTime;
        }
    }

    void UpdateOrbit()
    {
        angle += orbitSpeed * Time.deltaTime;
        angle = Mathf.Repeat(angle, Mathf.PI * 2f);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * orbitRadius,
            Mathf.Sin(angle) * orbitRadius,
            0f
        );

        transform.position = center.position + offset;

        Vector3 tangent = new Vector3(
            -Mathf.Sin(angle),
            Mathf.Cos(angle),
            0f
        );

        if (tangent.sqrMagnitude > 0.001f)
        {
            transform.up = tangent.normalized;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (hitTimer > 0f)
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(contactDamage);
        hitTimer = hitCooldown;
    }

    public void SetDamage(int damage)
    {
        contactDamage = damage;
    }
}
