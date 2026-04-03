# MediSim Pratik Rehberi — Adım Adım

Bu rehber seni sıfırdan alıp, Centargo projesine katkı sunabilecek seviyeye getirecek.
Her adımda tam olarak ne yapacağını, hangi dosyayı nereye koyacağını ve neden yaptığını anlatıyorum.

---

## BAŞLANGIÇ: Projeyi GitHub'a Koy

### Neden?
GitHub Actions sadece GitHub'daki repolarda çalışır. Projeyi GitHub'a koyman lazım ki CI pipeline'ları deneyebilesin.

### Ne yapacaksın?

1. İndirdiğin `medisim-practice-project.zip` dosyasını bir klasöre aç
2. Terminal (veya Git Bash) aç ve o klasöre git:

```bash
cd medisim
```

3. Git reposu oluştur:

```bash
git init
git add .
git commit -m "Initial commit: MediSim project with intentional tech debt"
```

4. GitHub.com'a git → "New Repository" tıkla
   - Repository name: `medisim`
   - Public seç (GitHub Actions ücretsiz olsun diye)
   - "Create repository" tıkla

5. GitHub'ın verdiği komutları çalıştır:

```bash
git remote add origin https://github.com/SENIN-KULLANICI-ADIN/medisim.git
git branch -M main
git push -u origin main
```

6. GitHub'da reponun sayfasına git → "Actions" sekmesine tıkla
   - Şu an boş olacak çünkü henüz workflow yazmadın

### Ne öğrendin?
Git ile proje oluşturup GitHub'a push etmeyi. Bu Centargo'da da ilk yapacağın şey — repoları klonlayıp üzerinde çalışmak.

---

## GÖREV 1: README Yaz (30 dakika)

### Neden?
Centargo'daki 8 reponun hepsinin README'si boş. İlk "quick win" bu.
Yeni bir geliştirici repoya baktığında ne olduğunu anlamalı.

### Ne yapacaksın?

1. Yeni branch oluştur:
```bash
git checkout -b task/01-readme
```

2. `README.md` dosyasını aç ve içeriğini şununla değiştir:

```markdown
# MediSim — CT Injection System Simulator

MediSim, Centargo CT enjeksiyon sisteminin eğitim amaçlı simülasyonudur.
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
```

3. Kaydet, commit et ve push et:

```bash
git add README.md
git commit -m "docs: Add comprehensive README with build instructions"
git push origin task/01-readme
```

4. GitHub'a git → "Pull Requests" → "New Pull Request"
   - base: main ← compare: task/01-readme
   - "Create Pull Request" tıkla
   - "Merge Pull Request" tıkla

5. Lokalinde main'e dön:
```bash
git checkout main
git pull
```

### Ne öğrendin?
- Branch oluşturma ve PR süreci (Centargo'da her değişiklik böyle yapılıyor)
- README yazma standardı
- Git workflow: branch → commit → push → PR → merge

---

## GÖREV 2: İlk CI Pipeline — I18n Doğrulama (1 saat)

### Neden?
Bu en basit pipeline. Sadece JSON dosyalarını kontrol edecek.
GitHub Actions'ı öğrenmek için ideal başlangıç noktası.

### Ne yapacaksın?

1. Yeni branch:
```bash
git checkout -b task/02-i18n-ci
```

2. Şu dosyayı oluştur: `.github/workflows/ci-i18n.yml`

```yaml
# Bu dosya GitHub'a "şu işleri otomatik yap" diyen talimat dosyası.
# Her push ve PR'da çalışacak.

name: I18n Validation

# NE ZAMAN çalışsın?
on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

# HANGİ İŞLERİ yapsın?
jobs:
  validate-translations:
    # HANGİ MAKİNEDE çalışsın?
    runs-on: ubuntu-latest

    # ADIMLAR (sırayla çalışır)
    steps:
      # Adım 1: Kodu GitHub'dan CI makinesine indir
      - name: Checkout code
        uses: actions/checkout@v4

      # Adım 2: İngilizce JSON dosyasının geçerli olup olmadığını kontrol et
      - name: Validate English JSON
        run: python3 -m json.tool src/I18n/en/i18n-bundle.json > /dev/null

      # Adım 3: Türkçe JSON dosyasının geçerli olup olmadığını kontrol et
      - name: Validate Turkish JSON
        run: python3 -m json.tool src/I18n/tr/i18n-bundle.json > /dev/null

      # Adım 4: İki dosyadaki key'lerin aynı olup olmadığını kontrol et
      - name: Check key consistency
        run: |
          echo "İngilizce key'leri çıkarıyorum..."
          python3 -c "
          import json

          with open('src/I18n/en/i18n-bundle.json') as f:
              en = json.load(f)
          with open('src/I18n/tr/i18n-bundle.json') as f:
              tr = json.load(f)

          en_keys = set(en['phrases'].keys())
          tr_keys = set(tr['phrases'].keys())

          missing_in_tr = en_keys - tr_keys
          missing_in_en = tr_keys - en_keys

          if missing_in_tr:
              print(f'HATA: Türkçe dosyada eksik key var: {missing_in_tr}')
          if missing_in_en:
              print(f'HATA: İngilizce dosyada eksik key var: {missing_in_en}')
          if missing_in_tr or missing_in_en:
              exit(1)

          print(f'BASARILI: Tüm {len(en_keys)} key her iki dilde de mevcut.')
          "
```

3. Commit ve push:
```bash
git add .github/workflows/ci-i18n.yml
git commit -m "ci: Add I18n validation workflow"
git push origin task/02-i18n-ci
```

4. GitHub'da Actions sekmesine git ve workflow'un çalışmasını izle.

### Ne olmasını bekliyorsun?
**Workflow FAIL edecek!** Çünkü Türkçe dosyada bilerek eksik key'ler bıraktım.
`ui.label.patient`, `error.volume`, `warning.lowBattery` key'leri Türkçe'de yok.

Bu iyi bir şey — quality gate çalışıyor demek! Centargo'da da tam olarak bu yapılacak:
SonarQube quality gate'i fail edecek ve PR merge edilemeyecek.

5. Şimdi hatayı düzelt. `src/I18n/tr/i18n-bundle.json` dosyasını aç ve eksik key'leri ekle:

```json
{
  "metadata": {
    "locale": "tr",
    "version": "1.0.0",
    "lastUpdated": "2025-01-10"
  },
  "phrases": {
    "app.title": "MediSim Enjeksiyon Sistemi",
    "app.status.idle": "Hazır",
    "app.status.injecting": "Enjeksiyon Devam Ediyor",
    "app.status.error": "Sistem Hatası",
    "app.status.disconnected": "Bağlantı Kesildi",
    "ui.button.start": "Enjeksiyonu Başlat",
    "ui.button.stop": "Acil Durdur",
    "ui.button.prime": "Sistemi Hazırla",
    "ui.label.volume": "Hacim (mL)",
    "ui.label.flowRate": "Akış Hızı (mL/s)",
    "ui.label.pressure": "Basınç (PSI)",
    "ui.label.patient": "Hasta Bilgileri",
    "error.connection": "Enjektöre bağlanılamıyor",
    "error.pressure": "Basınç limiti aşıldı",
    "error.volume": "Hacim limiti aşıldı",
    "warning.lowBattery": "Düşük pil uyarısı"
  }
}
```

6. Commit ve push:
```bash
git add src/I18n/tr/i18n-bundle.json
git commit -m "fix: Add missing Turkish translation keys"
git push origin task/02-i18n-ci
```

7. GitHub Actions'a tekrar bak — bu sefer PASS edecek! (yeşil tik)

8. PR oluştur ve merge et.

### Ne öğrendin?
- GitHub Actions workflow dosyası nasıl yazılır
- `on`, `jobs`, `steps`, `run`, `uses` ne demek
- Quality gate kavramı: hatalı kod merge edilemiyor
- Fail → fix → pass döngüsü

---

## GÖREV 3: CRU Build ve Test Pipeline (1-2 saat)

### Neden?
Bu senin Centargo'daki ana çalışma alanın. CRU .NET projesi ve senin .NET bilgin var.
Centargo'da CRU-01 (testler CI'da çalışmıyor) ve CRU-06 (quality gate yok) bulgularını düzelteceksin.

### Ne yapacaksın?

1. Yeni branch:
```bash
git checkout -b task/03-cru-ci
```

2. Şu dosyayı oluştur: `.github/workflows/ci-cru.yml`

```yaml
name: CRU Build and Test

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      # 1. Kodu indir
      - name: Checkout code
        uses: actions/checkout@v4

      # 2. .NET 8 SDK kur
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0'

      # 3. NuGet paketlerini indir (xunit vs.)
      - name: Restore dependencies
        run: dotnet restore src/CRU/MediSim.CRU.sln

      # 4. Projeyi derle — Release modunda
      - name: Build
        run: dotnet build src/CRU/MediSim.CRU.sln --configuration Release --no-restore

      # 5. Testleri çalıştır — Bu Centargo'da yapılmayan şey!
      - name: Run tests
        run: dotnet test src/CRU/MediSim.CRU.sln --configuration Release --no-build --verbosity normal

      # 6. Build çıktısını sakla (artifact)
      - name: Upload build artifact
        uses: actions/upload-artifact@v4
        with:
          name: cru-release-build
          path: src/CRU/MediSim.CRU/bin/Release/
          retention-days: 30
```

3. Commit ve push:
```bash
git add .github/workflows/ci-cru.yml
git commit -m "ci: Add CRU build and test workflow"
git push origin task/03-cru-ci
```

4. GitHub Actions'da izle. Bu sefer PASS edecek çünkü placeholder testler `Assert.True(true)` diyor.
   Ama dikkat et — testler "geçiyor" ama aslında hiçbir şeyi test etmiyor!

5. PR oluştur ve merge et.

### Ne öğrendin?
- .NET projesini CI'da nasıl build edersin
- `dotnet restore → build → test` akışı
- Artifact saklama (build kanıtı)
- Centargo CRU-01 bulgusunun nasıl düzeltileceği

---

## GÖREV 4: HCU Build Pipeline — C++ (1-2 saat)

### Neden?
Centargo HCU'nun CI'ı sadece debug build yapıyor. Bu pipeline C++ build'ini ve
code formatting kontrolünü ekleyecek.

### Ne yapacaksın?

1. Yeni branch:
```bash
git checkout -b task/04-hcu-ci
```

2. Önce bir `.clang-format` dosyası oluştur: `src/HCU/.clang-format`

```yaml
# Kod formatlama kuralları — Centargo'da HCU-06 bulgusunu düzeltir
BasedOnStyle: LLVM
IndentWidth: 4
ColumnLimit: 120
BreakBeforeBraces: Allman
AllowShortFunctionsOnASingleLine: None
```

Bu dosya "C++ kodun nasıl görünmesi gerektiğini" tanımlıyor.
Centargo'da böyle bir dosya yok (HCU-06 bulgusu) ve herkes farklı formatlıyor.

3. Workflow dosyasını oluştur: `.github/workflows/ci-hcu.yml`

```yaml
name: HCU Build

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      # clang-format kur ve kontrol et
      - name: Check code formatting
        run: |
          sudo apt-get install -y clang-format
          # src/HCU altındaki tüm .cpp ve .h dosyalarını kontrol et
          find src/HCU -name '*.cpp' -o -name '*.h' | while read file; do
            clang-format --style=file:src/HCU/.clang-format --dry-run --Werror "$file" 2>&1
            if [ $? -ne 0 ]; then
              echo "FORMATLAMA HATASI: $file"
              FAILED=1
            fi
          done
          if [ "$FAILED" = "1" ]; then
            echo "Bazı dosyalar formatlama kurallarına uymuyor!"
            echo "Düzeltmek için: clang-format -i <dosya>"
            exit 1
          fi

      # CMake ile build
      - name: Build with CMake
        run: |
          cmake -B build -S src/HCU -DCMAKE_CXX_FLAGS="-Wall -Wextra"
          cmake --build build 2>&1
```

4. Commit, push, izle:
```bash
git add src/HCU/.clang-format .github/workflows/ci-hcu.yml
git commit -m "ci: Add HCU build and format check workflow"
git push origin task/04-hcu-ci
```

5. Muhtemelen formatting hatası verecek. Bu beklenen davranış!
   Centargo'da da kod formatting standardı olmadığı için ilk çalıştırmada hatalar çıkacak.

6. PR oluştur ve merge et (formatting hatasını şimdilik görmezden gelebilirsin).

### Ne öğrendin?
- C++ projesini CI'da build etmek
- clang-format ile kod standardı zorlama
- CMake build sistemi temelleri
- Centargo HCU-05 ve HCU-06 bulgularının çözümü

---

## GÖREV 5: Build Scriptlerini Düzelt (1 saat)

### Neden?
Centargo'daki en yaygın sorun: hardcoded path'ler ve interactive prompt'lar.
Bunlar CI'da çalışmayı engelliyor.

### Ne yapacaksın?

1. Yeni branch:
```bash
git checkout -b task/05-fix-build-scripts
```

2. `scripts/build_cru.cmd` dosyasını aç. Şu an böyle:

```batch
if "%1"=="" (
    SET /P VERSION=Enter version number (e.g. 1.0.0):
)
SET MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\...
```

Bunu şununla değiştir:

```batch
@echo off
REM MediSim CRU Build Script — CI-Compatible Version
REM Tüm path'ler environment variable ile ayarlanabilir.
REM Environment variable yoksa varsayılan değer kullanılır.

REM === VERSİYON ===
REM Eskiden interactive prompt vardı, şimdi argument olarak alınıyor
if "%1"=="" (
    echo HATA: Version number gerekli. Kullanım: build_cru.cmd 1.0.0
    exit /b 1
)
SET VERSION=%1

echo Building MediSim CRU v%VERSION%...

REM === PATH'LER ===
REM Her path environment variable'dan okunuyor.
REM Tanımlı değilse varsayılan değer kullanılıyor (eski davranış korunuyor).

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
echo NSIS:    %NSIS_PATH%
echo Output:  %OUTPUT_DIR%

REM === BUILD ===
"%MSBUILD_PATH%" src\CRU\MediSim.CRU.sln /p:Configuration=Release

if errorlevel 1 (
    echo BUILD FAILED
    exit /b 1
)

echo Build completed successfully. Output: %OUTPUT_DIR%
```

3. `scripts/build_hcu.sh` dosyasını da düzelt:

```bash
#!/bin/bash
# MediSim HCU Build Script — CI-Compatible Version
# Tüm path'ler environment variable ile ayarlanabilir.

set -e  # Herhangi bir hata olursa script durur

# === PARAMETRELER ===
BUILD_TYPE="${1:-debug}"  # İlk argument: debug veya release. Varsayılan: debug

# === PATH'LER ===
# Environment variable varsa onu kullan, yoksa varsayılan değeri kullan
QT_DIR="${MEDISIM_QT_DIR:-/IMAX_USER/Qt6.5.6/gcc_64}"
BUILD_DIR="${MEDISIM_BUILD_DIR:-./build}"
INSTALL_DIR="${MEDISIM_INSTALL_DIR:-./install}"

echo "=== MediSim HCU Build ==="
echo "Build type: $BUILD_TYPE"
echo "Qt dir:     $QT_DIR"
echo "Build dir:  $BUILD_DIR"

# === BUILD ===
mkdir -p "$BUILD_DIR"
cd "$BUILD_DIR"

cmake ../../src/HCU -DCMAKE_PREFIX_PATH="$QT_DIR" 2>&1
make -j$(nproc) 2>&1

echo "Build successful ($BUILD_TYPE)"
```

4. Commit ve push:
```bash
git add scripts/build_cru.cmd scripts/build_hcu.sh
git commit -m "fix: Parameterize build scripts, remove interactive prompts

- Replace hardcoded paths with environment variables
- Remove interactive SET /P prompt (blocks CI)
- Add default values to preserve existing behavior
- Addresses Centargo findings: CRU-02, HCU-04, DIST-09"
git push origin task/05-fix-build-scripts
```

5. PR oluştur ve merge et.

### Ne öğrendin?
- Hardcoded path'leri environment variable'a çevirme (Centargo'daki 1 numaralı sorun)
- Varsayılan değerlerle geriye uyumluluğu koruma
- Interactive prompt'ları CI-uyumlu hale getirme
- Detaylı commit mesajı yazma (finding ID referansları ile)

---

## GÖREV 6: Credential Temizleme (1 saat)

### Neden?
Centargo'da CentargoPWs.txt dosyasında şifreler açık metin duruyor.
Ve kaynak kodda API key'ler hardcoded. Bunların hepsini temizlemen gerekiyor.

### Ne yapacaksın?

1. Yeni branch:
```bash
git checkout -b task/06-clean-credentials
```

2. `secrets/passwords.txt` dosyasını sil:
```bash
rm secrets/passwords.txt
```

3. `.gitignore` dosyasına ekle:
```
# Secrets — asla repoya eklenmemeli
secrets/
*.key
*.pem
```

4. `src/CRU/MediSim.CRU/Services/InjectorService.cs` dosyasını aç.
   Şu satırları bul:

```csharp
private const string API_KEY = "medisim-secret-key-2024";
private const string DB_PASSWORD = "admin123!";
```

Şununla değiştir:

```csharp
// Credential'lar artık environment variable'dan okunuyor
// CI'da GitHub Secrets kullanılıyor, lokalde .env dosyasından
private static readonly string API_KEY = 
    Environment.GetEnvironmentVariable("MEDISIM_API_KEY") ?? "";
private static readonly string DB_PASSWORD = 
    Environment.GetEnvironmentVariable("MEDISIM_DB_PASSWORD") ?? "";
```

5. Pre-commit hook config dosyası oluştur: `.pre-commit-config.yaml`

```yaml
# Pre-commit hooks: commit öncesi otomatik kontroller
# Kurulum: pip install pre-commit && pre-commit install
repos:
  - repo: https://github.com/pre-commit/pre-commit-hooks
    rev: v4.5.0
    hooks:
      - id: check-json              # JSON dosyaları geçerli mi?
      - id: check-merge-conflict    # Merge conflict marker kalmış mı?
      - id: detect-private-key      # Private key commit ediliyor mu?
      - id: trailing-whitespace     # Satır sonu boşluk var mı?
      - id: end-of-file-fixer       # Dosya sonunda newline var mı?

  - repo: https://github.com/Yelp/detect-secrets
    rev: v1.4.0
    hooks:
      - id: detect-secrets          # Şifre/API key pattern tespit
        args: ['--baseline', '.secrets.baseline']
```

6. Commit ve push:
```bash
git add -A
git commit -m "security: Remove plaintext credentials, add secret scanning

- Remove secrets/passwords.txt from repo
- Move hardcoded credentials to environment variables
- Add .pre-commit-config.yaml for secret scanning
- Update .gitignore to prevent future credential commits
- Addresses Centargo findings: PLT-05, MCU-09"
git push origin task/06-clean-credentials
```

7. PR oluştur ve merge et.

### Ne öğrendin?
- Credential temizleme süreci (Centargo'daki en acil güvenlik sorunu)
- Environment variable ile secret yönetimi
- Pre-commit hook kavramı
- .gitignore ile dosya koruması

---

## GÖREV 7: Coding Standards Dosyaları (30 dakika)

### Neden?
Centargo'da hiçbir repoda formatlama kuralı yok.
Her geliştirici farklı formatlıyor ve kod okumak zorlaşıyor.

### Ne yapacaksın?

1. Yeni branch:
```bash
git checkout -b task/07-coding-standards
```

2. Proje kökünde `.editorconfig` dosyası oluştur:

```ini
# EditorConfig: IDE'lerin otomatik tanıdığı formatlama kuralları
# VS Code, Visual Studio, JetBrains hepsi bunu okur
root = true

# Tüm dosyalar için genel kurallar
[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true
trim_trailing_whitespace = true
indent_style = space

# C# dosyaları
[*.cs]
indent_size = 4

# C/C++ dosyaları
[*.{c,cpp,h}]
indent_size = 4

# JSON dosyaları
[*.json]
indent_size = 2

# YAML dosyaları (workflow dosyaları)
[*.{yml,yaml}]
indent_size = 2

# Markdown
[*.md]
trim_trailing_whitespace = false

# Windows batch dosyaları
[*.cmd]
end_of_line = crlf

# Shell scriptleri
[*.sh]
end_of_line = lf
```

3. Commit, push, PR, merge:
```bash
git add .editorconfig
git commit -m "style: Add .editorconfig for consistent code formatting

Addresses Centargo findings: HCU-06, SCB-13"
git push origin task/07-coding-standards
```

### Ne öğrendin?
- EditorConfig standart dosyası
- Farklı dosya türleri için farklı kurallar
- IDE'lerin bu dosyayı otomatik tanıması

---

## GÖREV 8: Gerçek Unit Testler Yaz (2 saat)

### Neden?
Centargo CRU'da testler var ama hepsi placeholder — `Assert.True(true)`.
Gerçek testler yazarak CI'ın gerçekten bir şeyleri kontrol etmesini sağlayacaksın.

### Ne yapacaksın?

1. Yeni branch:
```bash
git checkout -b task/08-real-tests
```

2. `src/CRU/MediSim.CRU.Tests/PlaceholderTests.cs` dosyasını tamamen sil ve
   yerine iki yeni dosya oluştur.

Birinci dosya: `src/CRU/MediSim.CRU.Tests/InjectorServiceTests.cs`

```csharp
using Xunit;
using MediSim.CRU.Services;

namespace MediSim.CRU.Tests
{
    public class InjectorServiceTests
    {
        [Fact]
        public void GetStatus_BeforeInit_ReturnsIdle()
        {
            // Arrange & Act
            var status = InjectorService.Instance.GetStatus();

            // Assert — Başlangıçta status IDLE olmalı
            Assert.Equal("IDLE", status);
        }

        [Fact]
        public void Initialize_WithAddress_SetsConnected()
        {
            // Arrange
            var service = InjectorService.Instance;

            // Act
            service.Initialize("http://localhost:5000");

            // Assert
            Assert.True(service.IsConnected());
        }

        [Fact]
        public void GetInjectionCount_Initially_ReturnsZeroOrMore()
        {
            // Arrange & Act
            var count = InjectorService.Instance.GetInjectionCount();

            // Assert — Count negatif olmamalı
            Assert.True(count >= 0);
        }
    }
}
```

İkinci dosya: `src/CRU/MediSim.CRU.Tests/ProtocolServiceTests.cs`

```csharp
using Xunit;
using MediSim.CRU.Services;

namespace MediSim.CRU.Tests
{
    public class ProtocolServiceTests
    {
        [Fact]
        public void LoadProtocols_WhenNoFile_LoadsDefaults()
        {
            // Arrange
            var service = ProtocolService.Instance;

            // Act
            service.LoadProtocols();

            // Assert — Default protokoller yüklenmeli
            Assert.NotEmpty(ProtocolService.AllProtocols);
        }

        [Fact]
        public void GetProtocol_ExistingName_ReturnsProtocol()
        {
            // Arrange
            var service = ProtocolService.Instance;
            service.LoadProtocols();

            // Act
            var protocol = service.GetProtocol("Standard CT");

            // Assert
            Assert.NotNull(protocol);
            Assert.Equal("Standard CT", protocol.Name);
        }

        [Fact]
        public void GetProtocol_NonExistent_ReturnsNull()
        {
            // Arrange
            var service = ProtocolService.Instance;
            service.LoadProtocols();

            // Act
            var protocol = service.GetProtocol("Bu Protokol Yok");

            // Assert
            Assert.Null(protocol);
        }

        [Fact]
        public void LoadProtocols_DefaultProtocols_HaveValidValues()
        {
            // Arrange
            var service = ProtocolService.Instance;
            service.LoadProtocols();

            // Assert — Tüm protokollerin volume ve flowRate değeri pozitif olmalı
            foreach (var protocol in ProtocolService.AllProtocols)
            {
                Assert.True(protocol.Volume > 0,
                    $"Protocol '{protocol.Name}' has invalid volume: {protocol.Volume}");
                Assert.True(protocol.FlowRate > 0,
                    $"Protocol '{protocol.Name}' has invalid flow rate: {protocol.FlowRate}");
            }
        }
    }
}
```

3. Eski placeholder dosyasını sil:
```bash
rm src/CRU/MediSim.CRU.Tests/PlaceholderTests.cs
```

4. Commit, push, PR, merge:
```bash
git add -A
git commit -m "test: Replace placeholder tests with real unit tests

- Add InjectorServiceTests (3 tests)
- Add ProtocolServiceTests (4 tests)
- Remove PlaceholderTests.cs
- Addresses Centargo finding: CRU-01"
git push origin task/08-real-tests
```

5. GitHub Actions'da CRU pipeline'ının bu testleri çalıştırmasını izle.
   Artık testler gerçek şeyleri kontrol ediyor!

### Ne öğrendin?
- xUnit ile unit test yazma (Centargo CRU'da da xunit kullanılacak)
- Arrange-Act-Assert pattern
- Characterization test kavramı (mevcut davranışı test etme)
- Placeholder testleri gerçek testlerle değiştirme

---

## TAMAMLANMA KONTROL LİSTESİ

Her görevi tamamladığında işaretle:

- [ ] Görev 1: README yazıldı
- [ ] Görev 2: I18n CI pipeline çalışıyor (önce fail, sonra pass)
- [ ] Görev 3: CRU build + test pipeline çalışıyor
- [ ] Görev 4: HCU build + format check pipeline çalışıyor
- [ ] Görev 5: Build scriptleri parametrik hale getirildi
- [ ] Görev 6: Credential'lar temizlendi
- [ ] Görev 7: .editorconfig oluşturuldu
- [ ] Görev 8: Gerçek unit testler yazıldı

Tüm bunları tamamladığında 3 çalışan CI pipeline'ın, temizlenmiş credential'ların,
parametrik build scriptlerin ve gerçek unit testlerin olacak.

Bu tam olarak Centargo WP-1'de yapılacak işlerin küçük ölçekli provası.

---

## SIRALAMA ÖNERİSİ

Görevleri bu sırayla yap:
1 → 2 → 3 → 5 → 6 → 7 → 4 → 8

Neden bu sıra?
- 1 (README) ısınma turu, git workflow öğrenirsin
- 2 (I18n CI) en basit pipeline, GitHub Actions öğrenirsin
- 3 (CRU CI) senin ana alanın, .NET pipeline öğrenirsin
- 5 (Build scripts) CI'ı anladıktan sonra scriptleri düzeltmek daha kolay
- 6 (Credentials) güvenlik temizliği
- 7 (EditorConfig) en kısa görev, moral boost
- 4 (HCU CI) C++ pipeline biraz daha zor
- 8 (Testler) en son çünkü test yazmak en derin bilgi gerektiriyor
