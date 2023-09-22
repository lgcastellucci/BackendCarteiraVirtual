using BackendCarteiraVirtual.Controllers;

namespace BackendCarteiraVirtualTest
{
    [TestClass]
    public class InicializarBancoDeDadosControllerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var controller = new InicializarBancoDeDadosController(null);

            // Act
            var result = controller.Get();

            // Assert
            Assert.IsNotNull(result);
        }
    }
}