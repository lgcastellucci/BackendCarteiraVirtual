using Microsoft.AspNetCore.Mvc;

namespace BackendCarteiraVirtual.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InicializarBancoDeDadosController : ControllerBase
    {
        private readonly ILogger<InicializarBancoDeDadosController> _logger;

        public InicializarBancoDeDadosController(ILogger<InicializarBancoDeDadosController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "InicializarBancoDeDados")]
        public RetornoPost Get()
        {
            var retorno = new RetornoPost();

            var connectionString = StringConexaoBD.ObterStringDeConexao();



            return retorno;
        }


    }
}