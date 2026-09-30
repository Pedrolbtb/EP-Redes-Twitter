using System;

// Os valores numéricos viajam no JSON. Não reordenar: quebraria a compatibilidade com outros clientes.
// EDITAR_MSG e LISTAR_SEGUIDORES são reservados; o dispatcher atual ainda não os implementa.
public enum Comando
{
	LOGIN,
	CADASTRO,
	LISTAR_MSGS,
	POSTAR_MSG,
	EDITAR_MSG,
	DELETAR_MSG,
	SEGUIR,
	LISTAR_SEGUIDORES,
	LISTAR_USUARIOS,
	LISTAR_MEUS_SEGUIDORES,
	LISTAR_MEUS_SEGUINDO,
}
