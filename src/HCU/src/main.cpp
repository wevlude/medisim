#include <cstdio>
#include "device_controller.h"

// ISSUE [HCU-04]: Hardcoded paths
#define CONFIG_PATH "/home/user/Imaxeon/config/device.conf"
#define LOG_PATH "/home/user/Imaxeon/logs/"

int main(int argc, char* argv[])
{
    printf("MediSim HCU v1.0.0 starting...\n");

    // ISSUE [HCU-02]: Raw pointer, no smart pointer
    DeviceController* controller = new DeviceController();

    // ISSUE [HCU-04]: Hardcoded serial port
    int result = controller->initialize("/dev/ttyUSB0");

    if (result != 0) {
        printf("Failed to initialize device controller\n");
        delete controller;
        return 1;
    }

    // ISSUE [HCU-10]: Data anonymization conditional on build type
#ifdef BUILD_DEV
    printf("[DEV] Patient: John Doe, ID: 12345\n"); // PHI logged in dev builds
#else
    printf("[REL] Patient: ANONYMIZED\n");
#endif

    char statusBuf[256];
    controller->getStatus(statusBuf, sizeof(statusBuf));
    printf("Status: %s\n", statusBuf);

    controller->shutdown();
    delete controller;

    return 0;
}
