using SquidEyes.Trading.Context;

namespace SquidEyes.Trading.FxData
{
    public class MetaTick
    {
        public MetaTick(Pair pair, Tick tick)
        {
            Pair = pair;
            Tick = tick;
        }

        public Pair Pair { get; }
        public Tick Tick { get; }
    }
}
