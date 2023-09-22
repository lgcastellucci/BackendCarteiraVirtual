namespace BackendCarteiraVirtual
{
    public class Usuario
    {
        //<summary>
        //Nome completo do usuário
        //</summary>
        public required string Nome { get; set; }

        //<summary>
        //Documento do usuário, CPF ou CNPJ
        //</summary>
        public string? Documento { get; set; }

        //<summary>
        //Email do usuário
        //</summary>
        public string? Email { get; set; }
    }
}