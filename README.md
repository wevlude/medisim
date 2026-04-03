# MediSim — System Simulator

MediSim, eğitim amaçlı simülasyondur.
CI/CD pipeline oluşturma, teknik borç temizleme ve build otomasyonu pratikleri için kullanılır.

## Sistem Mimarisi

Sistem iki ana birimden oluşur:

- **CRU (Control Room Unit):** C# .NET 8 konsol uygulaması. Doktorun kullandığı kontrol yazılımını simüle eder.
- **HCU (Head Control Unit):** C++ 17 uygulaması. Enjektör donanımının kontrol yazılımını simüle eder.
- **I18n:** JSON formatında çoklu dil desteği dosyaları.

## Dizin Yapısı

```
medisim/
├── src/
│   ├── CRU/          # C# .NET projesi
│   ├── HCU/          # C++ CMake projesi
│   └── I18n/         # Çeviri dosyaları (JSON)
├── scripts/           # Build scriptleri
├── .github/workflows/ # CI pipeline dosyaları
└── README.md
```

## Gereksinimler

- .NET 8 SDK
- CMake 3.16+
- GCC veya Clang (C++ derleyici)
- Python 3 (I18n doğrulama için)

## Build Talimatları

### CRU Build
```bash
cd src/CRU
dotnet restore MediSim.CRU.sln
dotnet build MediSim.CRU.sln --configuration Release
```

### CRU Testleri
```bash
dotnet test src/CRU/MediSim.CRU.sln
```

### HCU Build
```bash
cd src/HCU
cmake -B build -S .
cmake --build build
```
