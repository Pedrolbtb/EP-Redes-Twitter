using System;

namespace epjb.Common
{
    // Envelope do protocolo: Tipo escolhe a operação e PayloadJson contém seus dados.
    // Sucesso/Erro pertencem à resposta; não confundir com a entidade Models.Mensagem (post).
    public class Mensagem
    {
        public Comando Tipo { get; set; }
        public string PayloadJson { get; set; }
        public bool Sucesso { get; set; }
        public string Erro { get; set; }

        // Construtor sem parâmetros usado pelo desserializador JSON.
        public Mensagem() { }

        // Monta uma requisição; o servidor preencherá o resultado na resposta.
        public Mensagem(Comando tipo, string payloadJson)
        {
            Tipo = tipo;
            PayloadJson = payloadJson;
        }

        // Padroniza a resposta positiva para todos os handlers.
        public static Mensagem RespostaSucesso(Comando tipo, string payloadJson = null)
        {
            return new Mensagem { Tipo = tipo, PayloadJson = payloadJson, Sucesso = true };
        }

        // Falhas de negócio ou transporte são exibidas pela interface através de Erro.
        public static Mensagem RespostaErro(Comando tipo, string erro)
        {
            return new Mensagem { Tipo = tipo, Sucesso = false, Erro = erro };
        }
    }
}
