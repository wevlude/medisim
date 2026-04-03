#!/bin/bash
# MediSim HCU Build Script
# ISSUE [HCU-04]: Hardcoded paths throughout

# ISSUE [HCU-04]: Hardcoded Qt path - matches deploy_target.sh
QT_DIR="/IMAX_USER/Qt6.8.5/gcc_64"
# ISSUE [HCU-04]: Hardcoded build directory
BUILD_DIR="/IMAX_USER/Imaxeon-dev/build/"
# ISSUE [HCU-04]: Hardcoded install directory
INSTALL_DIR="/home/user/Imaxeon/install"

echo "=== MediSim HCU Build ==="
echo "Qt: $QT_DIR"
echo "Build dir: $BUILD_DIR"

# ISSUE [HCU-11]: Depends on VCS metadata - matches "depends on Mercurial metadata"
if [ ! -d ".hg" ] && [ ! -d ".git" ]; then
    echo "WARNING: No VCS metadata found. Version info may be incorrect."
fi

# Build
mkdir -p "$BUILD_DIR"
cd "$BUILD_DIR" || exit 1

cmake ../../src/HCU -DCMAKE_PREFIX_PATH="$QT_DIR" 2>&1
make -j$(nproc) 2>&1

if [ $? -ne 0 ]; then
    echo "BUILD FAILED"
    exit 1
fi

echo "Build successful"

# ISSUE [HCU-05]: Only builds debug - matches "CI builds debug, not release"
echo "NOTE: This script only builds debug configuration"
echo "Release builds require the VM environment"

# ISSUE [INS-04]: No checksum verification
# TODO: Add SHA256 hash of build artifacts
# TODO: Add GPG signing
# FIXME: Hardcoded paths should be parameterized
