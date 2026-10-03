using System;

// Dados de LOGIN e CADASTRO recebidos pelo servidor; não inclui configuração de rede.
public class LoginRequest
{
	public string Username { get; set; }
	public string Password { get; set; }
}
