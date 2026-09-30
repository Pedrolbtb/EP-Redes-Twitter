using System;

// DTO legado de cadastro; hoje o handler usa LoginRequest e a confirmação de senha fica na tela.
public class CadastroRequest
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string RePassword { get; set; }
}
