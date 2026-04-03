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

            // Assert - dosya olmasa bile default protokoller yuklenmeli
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
        public void LoadProtocols_DefaultValues_ArePositive()
        {
            // Arrange
            var service = ProtocolService.Instance;
            service.LoadProtocols();

            // Assert - hacim ve akis hizi pozitif olmali
            foreach (var protocol in ProtocolService.AllProtocols)
            {
                Assert.True(protocol.Volume > 0);
                Assert.True(protocol.FlowRate > 0);
            }
        }
    }
}