using Microsoft.AspNetCore.Mvc;

namespace BackendCarteiraVirtual.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly ILogger<TransacaoController> _logger;

        public TransacaoController(ILogger<TransacaoController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/transacao", Name = "Transacao")]
        public RetornoPost Post(Transacao transacao)
        {
            var retorno = new RetornoPost();
            if (transacao == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Dados da transacao n�o pode ser vazio";
                return retorno;
            }

            if (transacao.Valor <= 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Valor n�o pode ser vazio";
                return retorno;
            }

            if (
                 (transacao.PagadorDocumento == null || transacao.PagadorDocumento == "") &&
                 (transacao.PagadorEmail == null || transacao.PagadorEmail == "")
               )
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "PagadorDocumento ou PagadorEmail deve ser preenchidos";
                return retorno;
            }

            if (
                 (transacao.RecebedorDocumento == null || transacao.RecebedorDocumento == "") &&
                 (transacao.RecebedorEmail == null || transacao.RecebedorEmail == "")
               )
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "PagadorDocumento ou PagadorEmail deve ser preenchidos";
                return retorno;
            }

            var acessoBD = new AcessoBancoDeDados();
            int codUsuarioPagador = acessoBD.RetornaCodUsuario(transacao.PagadorDocumento, transacao.PagadorEmail);
            int codUsuarioRecebedor = acessoBD.RetornaCodUsuario(transacao.RecebedorDocumento, transacao.RecebedorEmail);

            if (codUsuarioPagador == 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario pagador n�o encontrado";
                return retorno;
            }
            if (codUsuarioRecebedor == 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario recebedor n�o encontrado";
                return retorno;
            }

            if (acessoBD.InserirTransacao(codUsuarioPagador, codUsuarioRecebedor, transacao.Valor) == 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Erro ao inserir usu�rio";
                return retorno;
            }

            retorno.Sucesso = true;
            return retorno;
        }


    }
}