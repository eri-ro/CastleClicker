
// Passive tick: adds your current coins-per-second to this second’s coin gain.
public sealed class GoldMineGenerator : ResourceGenerator
{
    public override void Produce(ref double coinsDelta, ref double manaDelta)
    {
        coinsDelta += ResourceManager.Instance.GetCoinsPerSecond();
    }
}
