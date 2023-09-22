namespace BackendCarteiraVirtual
{
    public class RetornoPost
    {
        //<summary>
        //Definido como true quando a opera��o for realizada com sucesso
        //</summary>
        public bool Sucesso { get; set; }

        //<summary>
        //A mensagem conter� valor quando Sucesso for false, ou seja quando n�o for poss�vel realizar a opera��o
        //</summary>
        public string Mensagem { get; set; }

        public RetornoPost()
        {
            Sucesso = false;
            Mensagem = "";
        }

    }

}