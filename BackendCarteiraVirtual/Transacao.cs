namespace BackendCarteiraVirtual
{
    //<summary>
    //Classe que representa uma transação
    //Uma transação é uma transferência de valor entre dois usuários
    //O usuário que envia o valor é o pagador, ou seja o usuário comum
    //O usuário que recebe o valor é o recebedor, ou seja o lojista
    //Pode ser enviado o documento ou email do usuário
    //</summary>
    public class Transacao
    {
        //<summary>
        //Docuemnto do Usuario Comun
        //Docuemnto do usuário que irá enviar o valor
        //</summary>
        public string? PagadorDocumento { get; set; }

        //<summary>
        //Email do Usuario Comun
        //Email do usuário que irá enviar o valor
        //</summary>
        public string? PagadorEmail { get; set; }

        //<summary>
        //Docuemnto do Lojista
        //Docuemnto do usuário que irá receber o valor
        //</summary>
        public string? RecebedorDocumento { get; set; }

        //<summary>
        //Email do Lojista
        //Email do usuário que irá receber o valor
        //</summary>
        public string? RecebedorEmail { get; set; }

        //<summary>
        //Valor da transacao
        //</summary>
        public double Valor { get; set; }
    }
}