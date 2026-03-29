
// Passive tick: adds your current mana-per-second to this second’s mana gain.
    public sealed class ManaWellGenerator : ResourceGenerator
    {
        public override void Produce(ref double coinsDelta, ref double manaDelta)
        {
            manaDelta += ResourceManager.Instance.GetManaPerSecond();
        }
}
