using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

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

        [HttpGet(Name = "inicializarBancoDeDados")]
        public RetornoPost Get()
        {
            var retorno = new RetornoPost();

            var connectionString = StringConexaoBD.ObterStringDeConexao();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "";
                sqlQuery += " IF OBJECT_ID('USUARIOS') IS NULL ";
                sqlQuery += " BEGIN ";
                sqlQuery += "   CREATE TABLE USUARIOS(COD_USUARIO INTEGER IDENTITY(1,1), NOME VARCHAR(100), DOCUMENTO VARCHAR(14), EMAIL VARCHAR(100) NULL, TIPO VARCHAR(1), SALDO NUMERIC(15,2), CRIACAO DATETIME) ";
                sqlQuery += "   ALTER TABLE USUARIOS ADD CONSTRAINT XPKUSUARIOS PRIMARY KEY CLUSTERED(COD_USUARIO ASC) ";
                sqlQuery += " END ";
                sqlQuery += " ";
                sqlQuery += " IF OBJECT_ID('TRANSACOES') IS NULL ";
                sqlQuery += " BEGIN ";
                sqlQuery += "   CREATE TABLE TRANSACOES(COD_TRANSACAO INTEGER IDENTITY(1,1), COD_USUARIO_PAGADOR INTEGER, COD_USUARIO_RECEBEDOR INTEGER, VALOR NUMERIC(15,2), TIPO VARCHAR(15), DATA DATETIME) ";
                sqlQuery += "   ALTER TABLE TRANSACOES ADD CONSTRAINT XPKTRANSACOES PRIMARY KEY CLUSTERED(COD_TRANSACAO ASC) ";
                sqlQuery += " END ";

                try
                {
                    connection.Open();
                    var command = new SqlCommand(sqlQuery, connection);
                    int linhasAfetadas = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = "Erro ao inicializar o banco de dados: " + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            
            retorno.Sucesso = true;
            return retorno;
        }


    }
}