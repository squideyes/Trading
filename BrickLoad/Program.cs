using SquidEyes.Basics;
using SquidEyes.Trading.Context;
using SquidEyes.Trading.FxData;
using System.Text;
using System.Threading.Tasks.Dataflow;

const string BASEPATH = @"C:\TickData\DC\TICKSETS";
const int PIPS = 2;

foreach (var enumeration in EnumList.FromAll<Symbol>())
{
    var startedOn = DateTime.UtcNow;

    var fileNames = Directory.GetFiles(BASEPATH,
        "*.sts", SearchOption.AllDirectories).ToList();

    var results = new StringBuilder();

    var worker = new ActionBlock<string>(
        fileName =>
        {
            var tickSet = TickSet.Create(fileName);

            using var stream = File.OpenRead(fileName);

            tickSet.LoadFromStream(stream, DataKind.STS);

            var feed = new WickoFeed(
                tickSet.Session, PIPS, MidOrAsk.Mid);

            var candleSetIds = new Dictionary<int, int>();

            feed.OnCandle += (s, e) =>
            {
                if (!candleSetIds.ContainsKey(e.CandleSetId))
                    candleSetIds[e.CandleSetId] = 1;
                else
                    candleSetIds[e.CandleSetId]++;
            };

            foreach (var tick in tickSet)
                feed.HandleTick(tick);

            var multis = candleSetIds.Where(c => c.Value >= 2).Count();

            results.AppendLine($"{fileName},{tickSet.Count},{multis}");

            if (multis == 0)
                Console.WriteLine($"{fileName}: NO Multies");
            else
                Console.WriteLine($"{fileName}: {tickSet.Count:N0} Candles, {multis:N0} Multies");
        });

    fileNames.ForEach(f => worker.Post(f));

    worker.Complete();

    await worker.Completion;

    var elapsed = DateTime.UtcNow - startedOn;

    var perFile = elapsed.TotalMilliseconds / fileNames.Count;

    File.WriteAllText("Results.csv", results.ToString());

    Console.WriteLine($"Elapsed: {elapsed}, PerFile: {perFile:N0}");
}