namespace MorgueManager.Web.Services;

public class TemperatureSimulatorService : IDisposable
{
    private Timer? _timer;
    private const int UpdateIntervalMs = 30000;

    public event Action? OnTemperatureChanged;

    public Dictionary<string, double> SlotTemperatures { get; private set; } = new();
    public double ZoneATarget { get; private set; } = 2.0;
    public double ZoneBTarget { get; private set; } = 2.0;
    public double ZoneACurrent { get; private set; } = 2.3;
    public double ZoneBCurrent { get; private set; } = 3.8;

    private static readonly string[] SlotIds = [
        "A-01","A-02","A-03","A-04","A-05","A-06","A-07","A-08","A-09","A-10",
        "B-01","B-02","B-03","B-04","B-05","B-06","B-07","B-08","B-09","B-10"
    ];

    private static readonly string[] OccupiedSlots = [
        "A-01","A-02","A-03","A-06","A-07","A-08","B-01","B-02","B-03","B-06"
    ];

    public TemperatureSimulatorService()
    {
        LoadFromLocalStorage();
        InitializeSlots();
        _timer = new Timer(_ => Tick(), null, UpdateIntervalMs, UpdateIntervalMs);
    }

    private void InitializeSlots()
    {
        var rng = new Random();
        foreach (var id in SlotIds)
        {
            var zone = id.StartsWith("A") ? ZoneACurrent : ZoneBCurrent;
            var occupied = OccupiedSlots.Contains(id) ? 0.2 : 0;
            SlotTemperatures[id] = Math.Round(zone + (rng.NextDouble() * 0.6 - 0.3) + occupied, 1);
        }
    }

    private void Tick()
    {
        var rng = new Random();
        var baseA = ZoneATarget + (rng.NextDouble() * 1.6 - 0.8);
        var baseB = ZoneBTarget + (rng.NextDouble() * 1.6 - 0.8);
        ZoneACurrent = Math.Round(baseA, 1);
        ZoneBCurrent = Math.Round(baseB, 1);

        foreach (var id in SlotIds)
        {
            var zone = id.StartsWith("A") ? baseA : baseB;
            var occupied = OccupiedSlots.Contains(id) ? 0.2 : 0;
            SlotTemperatures[id] = Math.Round(zone + (rng.NextDouble() * 0.6 - 0.3) + occupied, 1);
        }

        OnTemperatureChanged?.Invoke();
    }

    public void SetTarget(string zone, double value)
    {
        if (zone == "A") ZoneATarget = value;
        else if (zone == "B") ZoneBTarget = value;
    }

    private void LoadFromLocalStorage()
    {
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
