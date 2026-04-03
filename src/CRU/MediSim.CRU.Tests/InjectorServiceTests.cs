using Xunit;
using MediSim.CRU.Services;

namespace MediSim.CRU.Tests
{
    public class InjectorServiceTests
    {
        [Fact]
        public void GetStatus_BeforeInit_ReturnsIdle()
        {
            // Arrange - hazırlık
            var service = InjectorService.Instance;

            // Act - testi yapacağın eylem
            var status = service.GetStatus();

            // Assert - beklenen sonuç
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
        public void GetInjectionCount_Initially_IsNotNegative()
        {
            // Arrange & Act
            var count = InjectorService.Instance.GetInjectionCount();

            // Assert
            Assert.True(count >= 0);
        }
    }
}