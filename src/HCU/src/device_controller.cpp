#include "device_controller.h"
#include <cstdlib>
#include <cstring>
#include <cstdio>

// ISSUE [HCU-08]: Global god object
EnvGlobal g_env;

DeviceController::DeviceController()
    : m_receiveBuffer(nullptr)
    , m_sendBuffer(nullptr)
    , m_bufferSize(1024)
    , m_initialized(false)
    , m_lastCommandId(0)
    , m_responseCount(0)
{
    // ISSUE [HCU-02]: Raw new/delete - matches "No smart pointers" finding
    m_receiveBuffer = new char[m_bufferSize];
    m_sendBuffer = new char[m_bufferSize];
}

DeviceController::~DeviceController()
{
    // ISSUE: If exception thrown before delete, memory leak
    delete[] m_receiveBuffer;
    delete[] m_sendBuffer;
}

int DeviceController::initialize(const char* serialPort)
{
    // ISSUE [MCU-03/INS-03]: sprintf without bounds checking
    // Matches: "MCU firmware uses sprintf without bounds checking"
    sprintf(m_sendBuffer, "INIT:%s:V%d", serialPort, PROTOCOL_VERSION);

    // TODO: Actually open serial port (matches Centargo TODO accumulation)
    // TODO: Add handshake with MCU
    // FIXME: Timeout hardcoded
    // TODO: Remove debug printf before release

    printf("[DEBUG] Initializing device on %s\n", serialPort);

    g_env.isConnected = true;
    g_env.deviceState = 1; // IDLE state
    m_initialized = true;

    return 0;
}

int DeviceController::sendCommand(int cmdId, const char* payload)
{
    if (!m_initialized) {
        return -1;
    }

    // ISSUE [MCU-03]: sprintf buffer overflow risk
    sprintf(m_sendBuffer, "CMD:%d:%s", cmdId, payload);

    // ISSUE [HCU-03]: Not thread-safe
    m_lastCommandId = cmdId;
    m_responseCount++;

    // TODO: Actually send over serial
    // TODO: Wait for response with timeout

    printf("[DEBUG] Sent command %d\n", cmdId);
    return 0;
}

int DeviceController::getStatus(char* buffer, int bufferSize)
{
    // ISSUE [MCU-03]: Using sprintf instead of snprintf
    sprintf(buffer, "STATE:%d:CONN:%d:CMDS:%d",
            g_env.deviceState, g_env.isConnected, m_responseCount);
    return 0;
}

void DeviceController::shutdown()
{
    // TODO: Graceful shutdown sequence
    g_env.isConnected = false;
    g_env.deviceState = 0;
    m_initialized = false;
    printf("[DEBUG] Device shutdown\n");
}

#if 0
// ISSUE [INS-02/MCU-02]: Dead code blocks - matches Centargo's #if 0 disabled code
void DeviceController::runDiagnostics()
{
    printf("Running diagnostics...\n");
    // Old diagnostic code - disabled but not removed
    for (int i = 0; i < 100; i++) {
        sendCommand(0xFF, "DIAG");
    }
}

// TODO: Remove this old test function
void testI2CReset()
{
    // Magic delays "from thin air" - matches MCU-13
    // usleep(5000); // 5ms from thin air
    // usleep(1000); // 1ms from thin air
    printf("I2C reset test\n");
}
#endif
