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

            // Use a operação de módulo (%) para obter a parte decimal
            float parteDecimal = (float)transaction.value % 1;

            // Converta a parte decimal em um número inteiro multiplicando por 100 (ou 1000 para casas decimais adicionais)
            int parteDecimalInteira = (int)(parteDecimal * 100); // 100 para 2 casas decimais

            // Verifique se a parte decimal inteira é par
            bool parteDecimalEPar = parteDecimalInteira % 2 == 0;

            if (parteDecimalEPar)
                return true;
            return false;
        }


    }
}