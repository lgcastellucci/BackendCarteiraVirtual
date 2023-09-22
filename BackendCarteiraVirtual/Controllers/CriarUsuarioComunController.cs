using Microsoft.AspNetCore.Mvc;

namespace BackendCarteiraVirtual.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CriarUsuarioComunController : ControllerBase
    {
        private readonly ILogger<CriarUsuarioComunController> _logger;

        public CriarUsuarioComunController(ILogger<CriarUsuarioComunController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/usuario/comun", Name = "UsuarioComun")]
        public RetornoPost Post(UsuarioComun usuario)
        {
            var retorno = new RetornoPost();
            if (usuario == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Dados do usuario n�o pode ser vazio";
                return retorno;
            }

            if (usuario.Nome == null || usuario.Nome == "")
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Nome n�o pode ser vazio";
                return retorno;
            }

            if (
                 (usuario.Documento == null || usuario.Documento == "") &&
                 (usuario.Email == null || usuario.Email == "")
               )
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Documento ou Email deve ser preenchido";
                return retorno;
            }

            var acessoBD = new AcessoBancoDeDados();
            if (acessoBD.UsuarioExiste(usuario.Documento, usuario.Email))
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usu�rio j� existe";
                return retorno;
            }

            if (!acessoBD.InserirUsuario(usuario.Nome, usuario.Documento, usuario.Email, usuario.Tipo))
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