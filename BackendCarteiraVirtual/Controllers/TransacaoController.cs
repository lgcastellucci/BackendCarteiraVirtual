using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Net;

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
                retorno.Mensagem = "Dados da transacao não pode ser vazio";
                return retorno;
            }

            if (transacao.Valor <= 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Valor não pode ser vazio";
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
            var usuarioPagador = acessoBD.RetornaUsuario(transacao.PagadorDocumento, transacao.PagadorEmail);
            var usuarioRecebedor = acessoBD.RetornaUsuario(transacao.RecebedorDocumento, transacao.RecebedorEmail);

            if (usuarioPagador.CodUsuario == 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario pagador não encontrado";
                return retorno;
            }
            if (usuarioRecebedor.CodUsuario == 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario recebedor não encontrado";
                return retorno;
            }

            if (usuarioPagador.Saldo <= 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario pagador não tem saldo";
                return retorno;
            }
            if (usuarioPagador.Saldo < transacao.Valor)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario pagador não tem saldo suficiente";
                return retorno;
            }

            if (usuarioPagador.Tipo != "C")
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario pagador não pode fazer essa transação";
                return retorno;
            }
            if (usuarioRecebedor.Tipo != "L")
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuario recebedor não pode fazer essa transação";
                return retorno;
            }

            int codTransacao = acessoBD.InserirTransacao(usuarioPagador.CodUsuario, usuarioRecebedor.CodUsuario, transacao.Valor);
            if (codTransacao == 0)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Erro ao inserir usuário";
                return retorno;
            }

            //chamar o externo
            var httpService = new HttpService();
            var httpResponseCode = httpService.EnviaTransacao(transacao.Valor).Result;

            if (httpResponseCode != HttpStatusCode.OK)
            {
                int codTransacaoDesfazimento = acessoBD.DesfazerTransacao(codTransacao);
            }

            retorno.Sucesso = true;
            return retorno;
        }


    }
}