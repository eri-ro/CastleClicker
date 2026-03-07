using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Moat : MonoBehaviour
{
    [Header("Shape")]
    public float innerRadius = 1.5f;
    public float outerRadius = 2.5f;
    public int circleSegments = 128;

    public enum MoatType { Water, Lava }

    [Header("Effects")]
    public MoatType moatType = MoatType.Water;
    public float slowMultiplier = 0.6f;
    public float lavaBurnDps = 25f;

    [Header("Target")]
    public Transform center;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = circleSegments;

        float width = outerRadius - innerRadius;
        lr.startWidth = width;
        lr.endWidth = width;
    }

    void Start()
    {
        if (center != null)
            transform.position = center.position;

        ApplyVisuals();
        DrawCircleAtRadius((innerRadius + outerRadius) * 0.5f);
    }

    void Update()
    {
        transform.position = center.position;

        Vector3 c = center.position;

        GameObject[] enemyGOs = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyGOs == null || enemyGOs.Length == 0) return;

        for (int i = 0; i < enemyGOs.Length; i++)
        {
            GameObject go = enemyGOs[i];
            if (go == null) continue;

            Enemy e = go.GetComponent<Enemy>();
            if (e == null) continue;

            float d = Vector2.Distance(go.transform.position, c);
            bool inRing = (d >= innerRadius && d <= outerRadius);

            if (inRing)
            {
                if (!e.ignoresMoatSlow)
                    e.SetSpeedMultiplier(slowMultiplier);
                else
                    e.ResetSpeedMultiplier();

                if (moatType == MoatType.Lava && !e.ignoresMoatBurn)
                    e.SetBurnDps(lavaBurnDps);
                else
                    e.SetBurnDps(0f);
            }
            else
            {
                e.ResetSpeedMultiplier();
                e.SetBurnDps(0f);
            }
        }
    }

    void DrawCircleAtRadius(float radius)
    {
        for (int i = 0; i < circleSegments; i++)
        {
            float t = (float)i / circleSegments * Mathf.PI * 2f;
            float x = Mathf.Cos(t) * radius;
            float y = Mathf.Sin(t) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    public void ApplyVisuals()
    {
        Color c = (moatType == MoatType.Water)
            ? new Color(0.2f, 0.5f, 1f, 0.75f)
            : new Color(1f, 0.35f, 0.05f, 0.85f);

        lr.startColor = c;
        lr.endColor = c;

        if (lr.material != null)
            lr.material.color = c;
    }
}