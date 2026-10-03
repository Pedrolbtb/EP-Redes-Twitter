namespace epjb.Common;

// UDP é usado apenas para diagnóstico, sem banco nem comandos que alterem dados.
// Formato textual UTF-8: EPJB_PING:<nonce> -> EPJB_PONG:<mesmo nonce>.
// O nonce permite rejeitar respostas antigas. UDP não garante entrega, ordem ou ausência de duplicatas.
public static class ProtocoloUdp
{
    public const int Porta = 11001;
    public const string Ping = "EPJB_PING:";
    public const string Pong = "EPJB_PONG:";
    public const int TamanhoNonce = 32; // Guid em formato N: 32 caracteres hexadecimais.
}
