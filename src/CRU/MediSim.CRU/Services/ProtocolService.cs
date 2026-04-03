using System;
using System.Collections.Generic;
using System.IO;

namespace MediSim.CRU.Services
{
    // ISSUE [CRU-04]: Another singleton
    public class ProtocolService
    {
        private static ProtocolService? _instance;

        // ISSUE: Global mutable state
        public static List<InjectionProtocol> AllProtocols = new List<InjectionProtocol>();

        public static ProtocolService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProtocolService();
                return _instance;
            }
        }

        private ProtocolService() { }

        public void LoadProtocols()
        {
            // ISSUE [CRU-02]: Hardcoded path - matches Centargo build path issues
            string protocolPath = @"C:\MediSim\Data\protocols.json";

            // TODO: Load from configurable path
            // TODO: Add protocol validation
            // FIXME: This silently fails if file doesn't exist

            try
            {
                if (File.Exists(protocolPath))
                {
                    string json = File.ReadAllText(protocolPath);
                    // TODO: Actually parse JSON
                    Console.WriteLine("Protocols loaded.");
                }
                else
                {
                    // Load defaults
                    AllProtocols.Add(new InjectionProtocol
                    {
                        Name = "Standard CT",
                        Volume = 100.0,
                        FlowRate = 3.0,
                        Delay = 0
                    });
                    AllProtocols.Add(new InjectionProtocol
                    {
                        Name = "High Flow",
                        Volume = 150.0,
                        FlowRate = 5.0,
                        Delay = 10
                    });
                    Console.WriteLine($"Loaded {AllProtocols.Count} default protocols.");
                }
            }
            catch (Exception) // ISSUE [CRU-05]: Catch-all with no handling
            {
                // Empty catch block - matches Centargo CRU-05
            }
        }

        // ISSUE: No input validation
        public InjectionProtocol? GetProtocol(string name)
        {
            return AllProtocols.Find(p => p.Name == name);
        }
    }

    public class InjectionProtocol
    {
        public string Name { get; set; } = "";
        public double Volume { get; set; }
        public double FlowRate { get; set; }
        public int Delay { get; set; }

        // ISSUE: ToString not overridden, debugging difficult
        // TODO: Add validation for Volume > 0 and FlowRate > 0
    }
}
