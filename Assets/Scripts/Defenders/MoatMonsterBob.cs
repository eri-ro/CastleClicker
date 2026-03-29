using UnityEngine;
// Gentle bob on the moat monster sprite (cosmetic).
public class MoatMonsterBob : MonoBehaviour
{
    public float bobHeight = 0.05f;
    public float bobSpeed = 3f;

    Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPos + new Vector3(0f, offsetY, 0f);
    }
}
