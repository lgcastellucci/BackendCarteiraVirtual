using System.Data.SqlClient;
using System.Transactions;

namespace BackendCarteiraVirtual
{
    public class AcessoBancoDeDados
    {
        public bool UsuarioExiste(string? Documento, string? Email)
        {
            if (RetornaCodUsuario(Documento, Email) == 0)
                return false;

            return true;
        }

        public int RetornaCodUsuario(string? Documento, string? Email)
        {
            if (Documento == null)
                Documento = "";
            if (Email == null)
                Email = "";

            var codUsuario = 0;
            var connectionString = StringConexaoBD.ObterStringDeConexao();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "";
                sqlQuery += " SELECT COD_USUARIO ";
                sqlQuery += " FROM USUARIOS ";
                if (!string.IsNullOrWhiteSpace(Documento))
                    sqlQuery += " WHERE DOCUMENTO = '" + Documento + "' ";
                else if (!string.IsNullOrWhiteSpace(Email))
                    sqlQuery += " WHERE EMAIL = '" + Email + "' ";

                try
                {
                    connection.Open();
                    var command = new SqlCommand(sqlQuery, connection);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            codUsuario = Convert.ToInt32(reader["COD_USUARIO"]);
                        }
                    }
                }
                catch
                {
                }
            }

            return codUsuario;
        }

        public bool InserirUsuario(string Nome, string? Documento, string? Email, string Tipo)
        {
            if (UsuarioExiste(Documento, Email))
                return false;

            var cadastrou = false;
            var connectionString = StringConexaoBD.ObterStringDeConexao();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "";
                sqlQuery += " INSERT INTO USUARIOS ";
                sqlQuery += " (NOME, DOCUMENTO, EMAIL, TIPO, SALDO, CRIACAO) ";
                sqlQuery += " VALUES ";
                sqlQuery += " ( '" + Nome + "', '" + Documento + "', '" + Email + "', '" + Tipo + "', 0, GETDATE() ) ";
                try
                {
                    connection.Open();
                    var command = new SqlCommand(sqlQuery, connection);
                    int linhasAfetadas = command.ExecuteNonQuery();
                    if (linhasAfetadas > 0)
                        cadastrou = true;
                }
                catch
                {

                }
            }

            return cadastrou;
        }

        public int InserirTransacao(int CodUsuarioPagador, int CodUsuarioRecebedor, double Valor)
        {
            if (CodUsuarioPagador <= 0)
                return 0;
            if (CodUsuarioRecebedor <= 0)
                return 0;
            if (Valor <= 0)
                return 0;

            var codTransacao = 0;
            var connectionString = StringConexaoBD.ObterStringDeConexao();
            var connection = new SqlConnection(connectionString);
            string sqlQuery = " ";
            var command = new SqlCommand();

            try
            {
                connection.Open();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                return 0;
            }

            SqlTransaction transaction;
            try
            {
                // Inicie a transação
                transaction = connection.BeginTransaction();
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                return 0;
            }

            #region Retirando o saldo do Pagador
            sqlQuery = "";
            sqlQuery += " UPDATE USUARIOS SET SALDO = SALDO - " + Valor.ToString();
            sqlQuery += " WHERE COD_USUARIO_PAGADOR = " + CodUsuarioPagador.ToString();
            command = new SqlCommand(sqlQuery, connection);
            if (command.ExecuteNonQuery() == 0)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }
            #endregion

            #region Retirando o saldo do Pagador
            sqlQuery = "";
            sqlQuery += " UPDATE USUARIOS SET SALDO = SALDO - " + Valor.ToString();
            sqlQuery += " WHERE COD_USUARIO_RECEBDOR = " + CodUsuarioRecebedor.ToString();
            command = new SqlCommand(sqlQuery, connection);
            if (command.ExecuteNonQuery() == 0)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }
            #endregion

            #region Inserindo a transacao
            sqlQuery = "";
            sqlQuery += " INSERT INTO TRANSACOES ";
            sqlQuery += " (COD_USUARIO_PAGADOR, COD_USUARIO_RECEBEDOR, VALOR, DATA) ";
            sqlQuery += " VALUES ";
            sqlQuery += " ( " + CodUsuarioPagador.ToString() + ", " + CodUsuarioRecebedor.ToString() + ", " + Valor.ToString() + "', GETDATE() ) ";
            command = new SqlCommand(sqlQuery, connection);
            if (command.ExecuteNonQuery() == 0)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }

            command = new SqlCommand("SELECT @@IDENTITY COD_TRANSACAO", connection);
            var reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }

            while (reader.Read())
                codTransacao = Convert.ToInt32(reader["COD_TRANSACAO"]);
            #endregion

            //chamar o externo

            transaction.Commit();
            connection.Close();
            connection.Dispose();

            return codTransacao;
        }


    }
}