using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

// One row in EnemyTypes.xml; XmlSerializer maps attributes to these fields.
[Serializable]
public class EnemyTypeXmlEntry
{
    [XmlAttribute("type")]
    public string type;

    [XmlAttribute("hp")]
    public int hp;

    [XmlAttribute("speed")]
    public float speed;

    [XmlAttribute("coinReward")]
    public double coinReward;

    [XmlAttribute("castleDamage")]
    public int castleDamage;

    [XmlAttribute("ignoresMoatSlow")]
    public bool ignoresMoatSlow;

    [XmlAttribute("ignoresMoatBurn")]
    public bool ignoresMoatBurn;
}

// Root of EnemyTypes.xml
[XmlRoot("EnemyTypes")]
public class EnemyTypesXmlRoot
{
    [XmlElement("Enemy")]
    public List<EnemyTypeXmlEntry> enemies = new List<EnemyTypeXmlEntry>();
}

// Loads EnemyTypes.xml into a dictionary for lookup by EnemyTypeS
public static class EnemyTypeStatsRegistry
{
    public const string XmlFileName = "EnemyTypes.xml";

    static readonly Dictionary<Enemy.EnemyType, EnemyTypeXmlEntry> StatsByType =
        new Dictionary<Enemy.EnemyType, EnemyTypeXmlEntry>();

    public static IReadOnlyDictionary<Enemy.EnemyType, EnemyTypeXmlEntry> Stats => StatsByType;

    // Call once at startup before enemies spawn
    public static void LoadFromXml()
    {
        StatsByType.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, XmlFileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"EnemyTypeStatsRegistry: missing {path}. Using built-in defaults.");
            FillBuiltInDefaults();
            return;
        }

        try
        {
            using (var stream = File.OpenRead(path))
            {
                var serializer = new XmlSerializer(typeof(EnemyTypesXmlRoot));
                var root = (EnemyTypesXmlRoot)serializer.Deserialize(stream);
                if (root?.enemies == null)
                    return;

                foreach (EnemyTypeXmlEntry row in root.enemies)
                {
                    if (string.IsNullOrEmpty(row.type))
                        continue;
                    if (!Enum.TryParse(row.type, ignoreCase: true, out Enemy.EnemyType et))
                    {
                        Debug.LogWarning($"EnemyTypeStatsRegistry: unknown enemy type \"{row.type}\" in XML.");
                        continue;
                    }
                    StatsByType[et] = row;
                }
            }

            Debug.Log($"EnemyTypeStatsRegistry: loaded {StatsByType.Count} enemy type(s) from {XmlFileName}.");
        }
        catch (Exception e)
        {
            Debug.LogError($"EnemyTypeStatsRegistry: failed to read XML — {e.Message}. Using built-in defaults.");
            StatsByType.Clear();
            FillBuiltInDefaults();
        }
    }

    static void FillBuiltInDefaults()
    {
        void Add(Enemy.EnemyType t, int hp, float speed, double coin, int dmg, bool noSlow, bool noBurn)
        {
            StatsByType[t] = new EnemyTypeXmlEntry
            {
                type = t.ToString(),
                hp = hp,
                speed = speed,
                coinReward = coin,
                castleDamage = dmg,
                ignoresMoatSlow = noSlow,
                ignoresMoatBurn = noBurn
            };
        }

        Add(Enemy.EnemyType.Basic, 2, 1.5f, 1, 1, false, false);
        Add(Enemy.EnemyType.FastFlying, 1, 3f, 2, 2, true, true);
        Add(Enemy.EnemyType.Tank, 10, 0.9f, 4, 4, false, false);
    }

    public static bool TryGet(Enemy.EnemyType type, out EnemyTypeXmlEntry entry)
    {
        return StatsByType.TryGetValue(type, out entry);
    }
}
