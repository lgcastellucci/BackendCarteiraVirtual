namespace BackendCarteiraVirtual
{
    public class StringConexaoBD
    {
        public static string ObterStringDeConexao()
        {
            // Crie um ConfigurationBuilder
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Defina o diret�rio base para encontrar o arquivo appsettings.json
                .AddJsonFile("appsettings.json"); // Carregue o arquivo appsettings.json

            // Construa a inst�ncia de IConfiguration
            var configuration = builder.Build();
            var stringConexao = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(stringConexao))
                return "";
            else
                return stringConexao;
        }
    }

}