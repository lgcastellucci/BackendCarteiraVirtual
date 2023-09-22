using Microsoft.AspNetCore.Mvc;

namespace BackendCarteiraVirtual.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CriarUsuarioLojistaController : ControllerBase
    {
        private readonly ILogger<CriarUsuarioComunController> _logger;

        public CriarUsuarioLojistaController(ILogger<CriarUsuarioComunController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/usuario/lojista", Name = "UsuarioLojista")]
        public RetornoPost Post(UsuarioLojista usuario)
        {
            var retorno = new RetornoPost();
            if (usuario == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Dados do usuario não pode ser vazio";
                return retorno;
            }

            if (usuario.Nome == null || usuario.Nome == "")
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Nome não pode ser vazio";
                return retorno;
            }

            if (
                 (usuario.Documento == null || usuario.Documento == "") &&
                 (usuario.Email == null || usuario.Email == "")
               )
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Documento ou Email devm ser preenchido";
                return retorno;
            }

            var acessoBD = new AcessoBancoDeDados();
            if (acessoBD.UsuarioExiste(usuario.Documento, usuario.Email))
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuário já existe";
                return retorno;
            }

            if (!acessoBD.InserirUsuario(usuario.Nome, usuario.Documento, usuario.Email, usuario.Tipo))
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Erro ao inserir usuário";
                return retorno;
            }

            retorno.Sucesso = true;
            return retorno;
        }


    }
}