using System;
using MediSim.CRU.Services;

namespace MediSim.CRU
{
    // ISSUE [CRU-04]: Singleton pattern - matches Centargo's extensive singleton usage
    public class Program
    {
        private static Program? _instance;
        private static readonly object _lock = new object();

        public static Program Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Program();
                    return _instance;
                }
            }
        }

        // ISSUE [SYS-04]: Hardcoded version - matches Centargo's fragmented versioning
        public static readonly string VERSION = "1.0.25043"; // yyddd format like Centargo

        static void Main(string[] args)
        {
            Console.WriteLine($"MediSim CRU v{VERSION} starting...");

            var injectorService = InjectorService.Instance;
            var protocolService = ProtocolService.Instance;

            // TODO: Implement proper startup sequence (matches CRU-10 TODO accumulation)
            // TODO: Add configuration loading
            // FIXME: This should read from config file, not hardcoded
            string hcuAddress = "http://192.168.11.5:5000"; // ISSUE [HCU-09]: HTTP no TLS

            try
            {
                injectorService.Initialize(hcuAddress);
                protocolService.LoadProtocols();

                Console.WriteLine("System ready. Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception ex) // ISSUE [CRU-05]: Overly broad exception catch
            {
                // ISSUE [CRU-05]: Swallowed exception - only logs, no recovery
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
