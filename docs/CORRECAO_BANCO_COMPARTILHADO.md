# Correção: dois clientes acessando o mesmo banco

## Problema e causa

Ao abrir `epjb`, `Program.Main` iniciava um servidor dentro do próprio cliente.
`ConexaoServidor` sempre se conectava a `IPAddress.Loopback`. Como o contexto
SQLite usava `Data Source=epjb.db`, cada computador acessava seu próprio arquivo,
relativo ao diretório de execução. Contas e mensagens de um PC não apareciam no
outro. Mesmo configurar uma pasta compartilhada não resolveria a separação
incorreta entre cliente e servidor.

## Comportamento corrigido

```text
Cliente A ── TCP/IP:11000 ──┐
                           ├── ServerSide ── SQLite central
Cliente B ── TCP/IP:11000 ──┘
```

- `epjb` inicia somente a interface Windows Forms. Banco, modelos persistidos,
  migrations, repositórios e listener são excluídos da compilação do cliente.
- `ServerSide` é um console .NET 10 incluído na solução. Compila os fontes de
  servidor existentes, aplica as migrations antes de aceitar conexões e atende
  cada conexão em uma tarefa própria.
- O servidor escuta em todas as interfaces IPv4, na porta TCP 11000. Falhas na
  inicialização são reportadas no console e encerram o processo com código 1.
- Cada cliente lê `servidor.json` ao lado do executável. `EPJB_SERVER_HOST`, quando
  definida, substitui o host do arquivo. O padrão `localhost` permite teste local;
  em PCs diferentes, configure o endereço alcançável do servidor.
- `EPJB_DB_PATH` seleciona o banco central. Sem essa variável, o caminho padrão é
  `dados/epjb.db` ao lado do executável do servidor, independente do diretório de
  trabalho. Use um caminho absoluto explícito para preservar a localização entre
  publicações. O banco legado versionado foi mantido sem alteração.

## Ajustes necessários ao fluxo compartilhado

Cada chamada de `ServicoApi` possui sua própria conexão, evitando que consultas
simultâneas sobrescrevam o mesmo socket. Os repositórios são descartados após o
comando. O envio TCP repete `Send` até transmitir todos os bytes, preservando o
formato com prefixo de tamanho já implementado pelo grupo.

Durante a integração, publicar uma mensagem falhava porque a desserialização
`dynamic` produzia `JsonElement` e o código tentava compará-lo com `null`. Os
handlers passaram a usar `JsonElement` explicitamente e validar seu tipo.

Na interface, listas passaram a usar DTOs tipados para que as propriedades do
JSON sejam reconhecidas pelo binding do Windows Forms. Atualizar também recarrega
usuários e seguindo; a lista de seguindo é limpa antes de ser preenchida, evitando
duplicações visuais.

## Validação realizada

- Compilação de `epjb.slnx` com .NET SDK 10.0.401 e
  `-p:EnableWindowsTargeting=true`: **zero erros**, 52 avisos de nulabilidade.
- Teste de integração executado com o serviço real do cliente e servidor em
  processo separado: **aprovado**.
- Cenários: dois cadastros simultâneos, login das duas contas, dois posts
  simultâneos, duas consultas ao mesmo feed, relação de seguir e persistência
  após reiniciar o servidor em outro diretório de trabalho.
- O teste usa `127.0.0.2` via `EPJB_SERVER_HOST`, banco temporário e conexões
  independentes. Ele valida a configuração do destino e o compartilhamento pelo
  servidor; não substitui um teste entre dois PCs físicos.
- `git diff --check`: aprovado antes dos commits.

Os comandos para reproduzir o teste e configurar os dois PCs estão no
[README](../README.md). A interface Windows Forms e a travessia do firewall da
rede real ainda precisam de validação no Windows e nos dois computadores.

## Compatibilidade e limites

A correção parte dos commits `14ede2f` e `2f1358b` do Pedro e mantém os comandos
existentes e o protocolo TCP. Não implementa os demais itens futuros do roteiro.

O servidor precisa permanecer ligado e alcançável. Dados de bancos antigos de
PCs diferentes não são mesclados: escolha e faça backup do banco que será usado
como central. A atualização do feed continua sendo manual. O protocolo atual
não inclui TLS nem sessão autenticada; a configuração descrita é para rede de
confiança ou VPN.
