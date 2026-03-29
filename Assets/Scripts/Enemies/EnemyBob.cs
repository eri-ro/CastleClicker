using UnityEngine;
// Gentle bob/sway on the enemy sprite (cosmetic).
public class EnemyBob : MonoBehaviour
{
    public float bobHeight = 0.05f;
    public float bobSpeed = 6f;

    public float swayAngle = 5f;
    public float swaySpeed = 4f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startPos + new Vector3(0, y, 0);

        float angle = Mathf.Sin(Time.time * swaySpeed) * swayAngle;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
