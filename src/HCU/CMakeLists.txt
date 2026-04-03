cmake_minimum_required(VERSION 3.16)
project(MediSim_HCU VERSION 1.0.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# ISSUE [HCU-04]: Hardcoded paths - matches Centargo
# set(QT_DIR "/IMAX_USER/Qt6.8.5/gcc_64/lib/cmake/Qt6")  # Uncomment for Qt build
# set(DEPLOY_DIR "/IMAX_USER/Imaxeon-dev/build/")

add_executable(medisim_hcu
    src/main.cpp
    src/device_controller.cpp
)

target_include_directories(medisim_hcu PRIVATE include)
