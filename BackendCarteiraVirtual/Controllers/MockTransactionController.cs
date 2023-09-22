using Microsoft.AspNetCore.Mvc;

namespace BackendCarteiraVirtual.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockTransactionController : ControllerBase
    {
        private readonly ILogger<MockTransactionController> _logger;

        public MockTransactionController(ILogger<MockTransactionController> logger)
        {
            _logger = logger;
        }

        public class Transaction
        {
            public double value { get; set; }
        }

        [HttpPost("/mock/transaction", Name = "MockTransaction")]
        public bool Post(Transaction transaction)
        {
            if (transaction == null)
                return false;

            // Use a opera��o de m�dulo (%) para obter a parte decimal
            float parteDecimal = (float)transaction.value % 1;

            // Converta a parte decimal em um n�mero inteiro multiplicando por 100 (ou 1000 para casas decimais adicionais)
            int parteDecimalInteira = (int)(parteDecimal * 100); // 100 para 2 casas decimais

            // Verifique se a parte decimal inteira � par
            bool parteDecimalEPar = parteDecimalInteira % 2 == 0;

            if (parteDecimalEPar)
                return true;
            return false;
        }


    }
}