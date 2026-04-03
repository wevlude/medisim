@echo off
REM MediSim CRU Build Script — CI-Compatible Version

REM === VERSION ===
if "%1"=="" (
    echo HATA: Version number gerekli.
    echo Kullanim: build_cru.cmd 1.0.0
    exit /b 1
)
SET VERSION=%1
echo Building MediSim CRU v%VERSION%...

REM === PATHS ===
if "%MEDISIM_MSBUILD_PATH%"=="" (
    SET MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe
) else (
    SET MSBUILD_PATH=%MEDISIM_MSBUILD_PATH%
)

if "%MEDISIM_NSIS_PATH%"=="" (
    SET NSIS_PATH=C:\Program Files (x86)\NSIS\makensis.exe
) else (
    SET NSIS_PATH=%MEDISIM_NSIS_PATH%
)

if "%MEDISIM_OUTPUT_DIR%"=="" (
    SET OUTPUT_DIR=C:\builds\medisim\CRU\%VERSION%
) else (
    SET OUTPUT_DIR=%MEDISIM_OUTPUT_DIR%\%VERSION%
)

echo MSBuild: %MSBUILD_PATH%
echo Output:  %OUTPUT_DIR%

REM === BUILD ===
"%MSBUILD_PATH%" src\CRU\MediSim.CRU.sln /p:Configuration=Release

if errorlevel 1 (
    echo BUILD FAILED
    exit /b 1
)

echo Build completed. Output: %OUTPUT_DIR%