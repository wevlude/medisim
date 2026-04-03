#!/bin/bash
set -e

BUILD_TYPE="${1:-debug}"
QT_DIR="${MEDISIM_QT_DIR:-/IMAX_USER/Qt6.5.6/gcc_64}"
BUILD_DIR="${MEDISIM_BUILD_DIR:-./build}"

echo "=== MediSim HCU Build ==="
echo "Build type: $BUILD_TYPE"
echo "Qt dir:     $QT_DIR"
echo "Build dir:  $BUILD_DIR"

mkdir -p "$BUILD_DIR"
cd "$BUILD_DIR"

cmake ../../src/HCU -DCMAKE_PREFIX_PATH="$QT_DIR" 2>&1
make -j$(nproc) 2>&1

echo "Build successful ($BUILD_TYPE)"