namespace BackendCarteiraVirtual
{
    public class RetornoPost
    {
        //<summary>
        //Definido como true quando a operação for realizada com sucesso
        //</summary>
        public bool Sucesso { get; set; }

        //<summary>
        //A mensagem conterá valor quando Sucesso for false, ou seja quando não for possível realizar a operação
        //</summary>
        public string Mensagem { get; set; }

        public RetornoPost()
        {
            Sucesso = false;
            Mensagem = "";
        }

    }

}