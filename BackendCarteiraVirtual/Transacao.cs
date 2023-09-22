namespace BackendCarteiraVirtual
{
    //<summary>
    //Classe que representa uma transa��o
    //Uma transa��o � uma transfer�ncia de valor entre dois usu�rios
    //O usu�rio que envia o valor � o pagador, ou seja o usu�rio comum
    //O usu�rio que recebe o valor � o recebedor, ou seja o lojista
    //Pode ser enviado o documento ou email do usu�rio
    //</summary>
    public class Transacao
    {
        //<summary>
        //Docuemnto do Usuario Comun
        //Docuemnto do usu�rio que ir� enviar o valor
        //</summary>
        public string? PagadorDocumento { get; set; }

        //<summary>
        //Email do Usuario Comun
        //Email do usu�rio que ir� enviar o valor
        //</summary>
        public string? PagadorEmail { get; set; }

        //<summary>
        //Docuemnto do Lojista
        //Docuemnto do usu�rio que ir� receber o valor
        //</summary>
        public string? RecebedorDocumento { get; set; }

        //<summary>
        //Email do Lojista
        //Email do usu�rio que ir� receber o valor
        //</summary>
        public string? RecebedorEmail { get; set; }

        //<summary>
        //Valor da transacao
        //</summary>
        public double Valor { get; set; }
    }
}