using System.Data.SqlClient;
namespace BackendCarteiraVirtual
{
    public class AcessoBancoDeDados
    {
        public class Usuario
        {
            public int CodUsuario { get; set; }
            public string Nome { get; set; }
            public string Documento { get; set; }
            public string Email { get; set; }
            public string Tipo { get; set; }
            public double Saldo { get; set; }
            public Usuario()
            {
                CodUsuario = 0;
                Nome = "";
                Documento = "";
                Email = "";
                Tipo = "";
                Saldo = 0;
            }
        }

        public bool UsuarioExiste(string? Documento, string? Email)
        {
            if (RetornaUsuario(Documento, Email).CodUsuario == 0)
                return false;

            return true;
        }

        public Usuario RetornaUsuario(string? Documento, string? Email)
        {
            var usuario = new Usuario();

            if (Documento == null)
                Documento = "";
            if (Email == null)
                Email = "";

            var connectionString = StringConexaoBD.ObterStringDeConexao();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "";
                sqlQuery += " SELECT COD_USUARIO, NOME, DOCUMENTO, EMAIL, TIPO, SALDO ";
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
                            usuario.CodUsuario = Convert.ToInt32(reader["COD_USUARIO"]);
                            usuario.Nome = (string)reader["NOME"];
                            usuario.Documento = (string)reader["DOCUMENTO"];
                            usuario.Email = (string)reader["EMAIL"];
                            usuario.Tipo = (string)reader["TIPO"];
                            usuario.Saldo = (double)((decimal)reader["SALDO"]);
                        }
                    }
                    reader.Close();
                }
                catch
                {
                }
            }

            return usuario;
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
            sqlQuery += " UPDATE USUARIOS SET SALDO = SALDO - " + Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            sqlQuery += " WHERE COD_USUARIO = " + CodUsuarioPagador.ToString();
            command = new SqlCommand(sqlQuery, connection, transaction);
            if (command.ExecuteNonQuery() == 0)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }
            #endregion

            #region Retirando o saldo do Recebedor
            sqlQuery = "";
            sqlQuery += " UPDATE USUARIOS SET SALDO = SALDO + " + Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            sqlQuery += " WHERE COD_USUARIO = " + CodUsuarioRecebedor.ToString();
            command = new SqlCommand(sqlQuery, connection, transaction);
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
            sqlQuery += " (COD_USUARIO_PAGADOR, COD_USUARIO_RECEBEDOR, VALOR, TIPO, DATA) ";
            sqlQuery += " VALUES ";
            sqlQuery += " ( ";
            sqlQuery += CodUsuarioPagador.ToString() + ", ";
            sqlQuery += CodUsuarioRecebedor.ToString() + ", ";
            sqlQuery += Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ", ";
            sqlQuery += "'TRANSACAO', ";
            sqlQuery += "'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "' ";
            sqlQuery += " ) ";
            command = new SqlCommand(sqlQuery, connection, transaction);
            if (command.ExecuteNonQuery() == 0)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }

            command = new SqlCommand("SELECT @@IDENTITY COD_TRANSACAO", connection, transaction);
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
            reader.Close();
            #endregion

            transaction.Commit();
            connection.Close();
            connection.Dispose();

            return codTransacao;
        }

        public int DesfazerTransacao(int CodTransacao)
        {
            if (CodTransacao <= 0)
                return 0;

            var codTransacaoDesfazimento = 0;
            var connectionString = StringConexaoBD.ObterStringDeConexao();
            var connection = new SqlConnection(connectionString);
            string sqlQuery = " ";
            SqlCommand command;
            SqlDataReader reader; ;

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

            #region pegando os usuarios
            sqlQuery = "";
            sqlQuery += " SELECT COD_USUARIO_PAGADOR, COD_USUARIO_RECEBEDOR, VALOR ";
            sqlQuery += " FROM TRANSACOES ";
            sqlQuery += " WHERE COD_TRANSACAO = " + CodTransacao.ToString();
            command = new SqlCommand(sqlQuery, connection, transaction);
            reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }

            int CodUsuarioPagador = 0;
            int CodUsuarioRecebedor = 0;
            double Valor = 0;
            while (reader.Read())
            {
                //aqui o pagador da transação passa no desfazimento a ser o recebedor
                CodUsuarioRecebedor = Convert.ToInt32(reader["COD_USUARIO_PAGADOR"]);
                CodUsuarioPagador = Convert.ToInt32(reader["COD_USUARIO_RECEBEDOR"]);
                Valor = Convert.ToDouble(reader["VALOR"]);
            }
            reader.Close();
            #endregion

            #region Retornando o saldo do Recebedor
            sqlQuery = "";
            sqlQuery += " UPDATE USUARIOS SET SALDO = SALDO - " + Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            sqlQuery += " WHERE COD_USUARIO = " + CodUsuarioPagador.ToString();
            command = new SqlCommand(sqlQuery, connection, transaction);
            if (command.ExecuteNonQuery() == 0)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }
            #endregion

            #region Retornando o saldo do Pagador
            sqlQuery = "";
            sqlQuery += " UPDATE USUARIOS SET SALDO = SALDO + " + Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            sqlQuery += " WHERE COD_USUARIO = " + CodUsuarioRecebedor.ToString();
            command = new SqlCommand(sqlQuery, connection, transaction);
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
            sqlQuery += " (COD_USUARIO_PAGADOR, COD_USUARIO_RECEBEDOR, VALOR, TIPO, DATA) ";
            sqlQuery += " VALUES ";
            sqlQuery += " ( ";
            sqlQuery += CodUsuarioPagador.ToString() + ", ";
            sqlQuery += CodUsuarioRecebedor.ToString() + ", ";
            sqlQuery += Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ", ";
            sqlQuery += "'DESFAZIMENTO', ";
            sqlQuery += "'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "' ";
            sqlQuery += " ) ";
            command = new SqlCommand(sqlQuery, connection, transaction);
            if (command.ExecuteNonQuery() == 0)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }

            command = new SqlCommand("SELECT @@IDENTITY COD_TRANSACAO", connection, transaction);
            reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                transaction.Rollback();
                connection.Close();
                connection.Dispose();
                return 0;
            }

            while (reader.Read())
                codTransacaoDesfazimento = Convert.ToInt32(reader["COD_TRANSACAO"]);
            reader.Close();
            #endregion

            transaction.Commit();
            connection.Close();
            connection.Dispose();

            return codTransacaoDesfazimento;
        }
    }
}