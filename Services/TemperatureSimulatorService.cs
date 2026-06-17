using Microsoft.JSInterop;
using System.Text.Json;

namespace MorgueManager.Web.Services;

public class TemperatureSimulatorService : IDisposable
{
    private Timer? _timer;
    private const int UpdateIntervalMs = 30000;
    private readonly IJSInProcessRuntime? _js;

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

    public class TemperatureHistoryLog
    {
        public DateTime Timestamp { get; set; }
        public double ZoneACurrent { get; set; }
        public double ZoneBCurrent { get; set; }
        public Dictionary<string, double> SlotTemperatures { get; set; } = new();
    }

    public List<TemperatureHistoryLog> History { get; private set; } = new();

    public TemperatureSimulatorService(IJSRuntime js)
    {
        _js = js as IJSInProcessRuntime;
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

        History.Add(new TemperatureHistoryLog
        {
            Timestamp = DateTime.Now,
            ZoneACurrent = ZoneACurrent,
            ZoneBCurrent = ZoneBCurrent,
            SlotTemperatures = new Dictionary<string, double>(SlotTemperatures)
        });

        if (History.Count > 1000)
        {
            History.RemoveAt(0);
        }

        if (_js != null)
        {
            try
            {
                _js.InvokeVoid("localStorage.setItem", "temperatureHistory", JsonSerializer.Serialize(History));
            }
            catch { }
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
        if (_js != null)
        {
            try
            {
                var json = _js.Invoke<string>("localStorage.getItem", "temperatureHistory");
                if (!string.IsNullOrEmpty(json))
                {
                    var loaded = JsonSerializer.Deserialize<List<TemperatureHistoryLog>>(json);
                    if (loaded != null)
                    {
                        History = loaded;
                    }
                }
            }
            catch { }
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
