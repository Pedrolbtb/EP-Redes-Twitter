using System;

namespace epjb.Common
{
    public class Mensagem
    {
        public Comando Tipo { get; set; }
        public string PayloadJson { get; set; }
        public bool Sucesso { get; set; }
        public string Erro { get; set; }

        public Mensagem() { }

        public Mensagem(Comando tipo, string payloadJson)
        {
            Tipo = tipo;
            PayloadJson = payloadJson;
        }

        public static Mensagem RespostaSucesso(Comando tipo, string payloadJson = null)
        {
            return new Mensagem { Tipo = tipo, PayloadJson = payloadJson, Sucesso = true };
        }

        public static Mensagem RespostaErro(Comando tipo, string erro)
        {
            return new Mensagem { Tipo = tipo, Sucesso = false, Erro = erro };
        }
    }
}
