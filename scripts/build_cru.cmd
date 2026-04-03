@echo off
REM MediSim CRU Build Script
REM ISSUE [CRU-02]: Hardcoded paths throughout
REM ISSUE [DIST-09]: Interactive prompts block CI execution

REM ISSUE [DIST-09]: Interactive prompt - matches Centargo's SET /P
if "%1"=="" (
    SET /P VERSION=Enter version number (e.g. 1.0.0): 
) else (
    SET VERSION=%1
)

if "%VERSION%"=="" (
    echo ERROR: Version number is required
    exit /b 1
)

echo Building MediSim CRU v%VERSION%...

REM ISSUE [CRU-02]: Hardcoded Visual Studio path
SET MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe

REM ISSUE [CRU-02]: Hardcoded NSIS path
SET NSIS_PATH=C:\Program Files (x86)\NSIS\makensis.exe

REM ISSUE [CRU-02]: Hardcoded output path
SET OUTPUT_DIR=C:\builds\medisim\CRU\%VERSION%

REM ISSUE [CRU-03]: Warnings suppressed
"%MSBUILD_PATH%" src\CRU\MediSim.CRU.sln /p:Configuration=Release /p:WarningLevel=0

if errorlevel 1 (
    echo BUILD FAILED
    exit /b 1
)

echo Build completed. Output: %OUTPUT_DIR%

REM ISSUE: No artifact signing step (matches DIST-08)
REM ISSUE: No checksum generation (matches INS-04)

REM TODO: Add code signing
REM TODO: Generate release manifest
REM FIXME: Output directory not created automatically
