using System;

// DTO reservado para evolução de mensagens; os handlers atuais usam idUsuario/conteudo via JsonElement.
public class MensagemRequest
{
	public long? Id { get; set; }
	public string Title { get; set; }
	public string Content { get; set; }
}
