namespace BackendCarteiraVirtual
{
    public class Usuario
    {
        //<summary>
        //Nome completo do usu�rio
        //</summary>
        public required string Nome { get; set; }

        //<summary>
        //Documento do usu�rio, CPF ou CNPJ
        //</summary>
        public string? Documento { get; set; }

        //<summary>
        //Email do usu�rio
        //</summary>
        public string? Email { get; set; }
    }
}