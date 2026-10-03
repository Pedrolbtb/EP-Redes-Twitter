namespace epjb.Common.DTO;

// Propriedades correspondem ao JSON já enviado pelo servidor e às colunas da tela.
public sealed class UsuarioResumo
{
    public int id { get; set; }
    public string username { get; set; } = "";
    public override string ToString() => username;
}

public sealed class MensagemResumo
{
    public int id { get; set; }
    public int idUsuario { get; set; }
    public string username { get; set; } = "";
    public string conteudo { get; set; } = "";
    public DateTime dataCriacao { get; set; }
    public DateTime? dataEdicao { get; set; }
}
