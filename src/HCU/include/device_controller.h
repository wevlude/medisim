#ifndef DEVICE_CONTROLLER_H
#define DEVICE_CONTROLLER_H

#include <string>
#include <cstdio>

// ISSUE [HCU-08]: God object - matches Centargo's EnvGlobal
// Single struct holds pointers to everything - tightly couples all modules
struct EnvGlobal {
    void* mcuComm;
    void* cruComm;
    void* workflowEngine;
    void* alertManager;
    void* examData;
    void* systemConfig;
    int deviceState;
    bool isConnected;
    char lastError[256];
};

// Global instance - matches Centargo pattern
extern EnvGlobal g_env;

class DeviceController {
public:
    DeviceController();
    ~DeviceController();

    int initialize(const char* serialPort);
    int sendCommand(int cmdId, const char* payload);
    int getStatus(char* buffer, int bufferSize);
    void shutdown();

private:
    // ISSUE [HCU-02]: Raw pointers, no smart pointers - matches Centargo
    char* m_receiveBuffer;
    char* m_sendBuffer;
    int m_bufferSize;
    bool m_initialized;

    // ISSUE [MCU-15]: Protocol version hardcoded - matches Centargo
    static const int PROTOCOL_VERSION = 54;

    // ISSUE [HCU-03]: Not thread-safe by design
    // "ds.imrServerData is not designed for thread safe"
    int m_lastCommandId;
    int m_responseCount;
};

#endif // DEVICE_CONTROLLER_H
