using UnityEngine;
// Short expanding ring where cannon splash damage lands (visual only).
    public sealed class CannonSplashVfx : MonoBehaviour
    {
    const float Duration = 0.38f;
    const int Segments = 48;

    LineRenderer line;
    float radius;
    float elapsed;
    Color baseColor;

    public static void Play(Vector2 worldCenter, float worldRadius)
    {
        if (worldRadius <= 0f) return;

        GameObject go = new GameObject("CannonSplashVfx");
        go.transform.position = worldCenter;
        go.transform.SetParent(SceneContainers.Instance.projectiles, true);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = false;
        lr.positionCount = Segments;
        float w = Mathf.Clamp(worldRadius * 0.08f, 0.06f, 0.2f);
        lr.startWidth = w;
        lr.endWidth = w;
        lr.numCapVertices = 3;
        lr.numCornerVertices = 2;

        Shader sh = Shader.Find("Sprites/Default");
        if (sh == null)
            sh = Shader.Find("Unlit/Color");
        if (sh != null)
            lr.material = new Material(sh);

        lr.sortingOrder = 80;

        Color ring = new Color(1f, 0.5f, 0.12f, 0.88f);
        lr.startColor = ring;
        lr.endColor = ring;

        CannonSplashVfx vfx = go.AddComponent<CannonSplashVfx>();
        vfx.line = lr;
        vfx.radius = worldRadius;
        vfx.baseColor = ring;
        vfx.SetRingPositions(0.26f);
    }

    void SetRingPositions(float radiusMul)
    {
        float r = radius * Mathf.Clamp(radiusMul, 0.05f, 2f);
        for (int i = 0; i < Segments; i++)
        {
            float ang = i / (float)Segments * Mathf.PI * 2f;
            line.SetPosition(i, new Vector3(Mathf.Cos(ang) * r, Mathf.Sin(ang) * r, 0f));
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float u = elapsed / Duration;
        if (u >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        float scaleT = Mathf.Lerp(0.26f, 1.02f, Mathf.SmoothStep(0f, 1f, u));
        SetRingPositions(scaleT);

        Color c = baseColor;
        c.a = baseColor.a * (1f - u);
        line.startColor = c;
        line.endColor = c;
    }
}
