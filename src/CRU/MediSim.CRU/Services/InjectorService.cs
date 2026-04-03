using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MediSim.CRU.Services
{
    // ISSUE [CRU-11]: Singleton with concurrent access - matches WCF ConcurrencyMode.Multiple
    // ISSUE [CRU-04]: God-object pattern - this class does too much
    public class InjectorService
    {
        private static InjectorService? _instance;
        private static readonly object _lock = new object();

        // ISSUE: Not thread-safe - matches HCU-03 "not designed for thread-safe"
        private string _status = "IDLE";
        private string _hcuAddress = "";
        private int _injectionCount = 0;
        private bool _isConnected = false;

        // ISSUE [PLT-05]: Hardcoded credentials - matches CentargoPWs.txt finding
        private static readonly string API_KEY =
        Environment.GetEnvironmentVariable("MEDISIM_API_KEY") ?? "";
        private static readonly string DB_PASSWORD =
        Environment.GetEnvironmentVariable("MEDISIM_DB_PASSWORD") ?? "";

        public static InjectorService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new InjectorService();
                    return _instance;
                }
            }
        }

        private InjectorService() { }

        public void Initialize(string hcuAddress)
        {
            _hcuAddress = hcuAddress;
            _isConnected = true;
            Console.WriteLine($"Connected to HCU at {_hcuAddress}");
        }

        // ISSUE [HCU-09]: Using HTTP instead of HTTPS
        public async Task<string> SendCommand(string command)
        {
            // TODO: Add HTTPS support (matches Centargo HTTP-only communication)
            // TODO: Add retry logic
            // FIXME: Timeout not configurable
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetAsync($"{_hcuAddress}/api/v1/{command}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                // ISSUE [CRU-05]: Empty catch block - matches Centargo finding
                return "";
            }
            catch (TaskCanceledException)
            {
                // ISSUE [CRU-05]: Swallowed exception
                return "";
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error handling
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        // ISSUE: Thread-safety - multiple threads can call this concurrently
        public void StartInjection(double volume, double flowRate)
        {
            if (_status != "IDLE")
            {
                // TODO: Handle concurrent injection requests properly
                return;
            }

            _status = "INJECTING";
            _injectionCount++;

            // Simulate injection
            Thread.Sleep(100); // FIXME: Don't use Thread.Sleep in production

            _status = "IDLE";
        }

        public string GetStatus() => _status;
        public int GetInjectionCount() => _injectionCount;
        public bool IsConnected() => _isConnected;
    }
}
