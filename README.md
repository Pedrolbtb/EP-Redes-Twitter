# EP Redes — Twitter simplificado

## Conexão recusada ao abrir o cliente

No PC do banco, execute `iniciar-servidor.cmd` e aguarde **Servidor TCP aguardando
clientes**. Nos PCs dos usuários, execute `iniciar-cliente.cmd` e abra **Servidor...**
na tela de login para informar o IP do servidor, **Testar conexão** e **Salvar**.
No outro PC, não use `localhost`. Os atalhos requerem Windows e SDK .NET 10.

A comunicação principal usa sockets TCP manuais; há também um diagnóstico UDP
manual na porta 11001. Veja o [guia de sockets, comentários e diagnóstico](docs/SOCKETS_E_CONEXAO.md)
para operações de rede, firewall, testes e diferenças entre os protocolos.

## Dois computadores usando o mesmo banco

Execute **um único ServerSide**. Cada pessoa abre o cliente `epjb`, configurado
para o IP desse servidor. O SQLite fica somente na máquina do servidor; cadastro,
login, mensagens e seguidores passam pelos sockets TCP implementados pelo grupo.
O PC servidor também pode executar um cliente.

Requer SDK .NET 10 para compilar. A interface é Windows Forms e roda no Windows;
o servidor de console pode rodar no Windows ou Linux.

### 1. Iniciar o servidor em um dos PCs

Na raiz do repositório, no PowerShell:

```powershell
# Opcional: continuar usando o banco existente do projeto.
# Faça backup primeiro e use somente a cópia escolhida como banco central.
$env:EPJB_DB_PATH = (Resolve-Path .\epjb\epjb.db).Path
dotnet run --project .\ServerSide\ServerSide.csproj
```

Sem `EPJB_DB_PATH`, o servidor cria um banco vazio em `dados/epjb.db` ao lado
do executável e aplica as migrations ao iniciar. O console mostra o caminho
absoluto do banco e a porta de escuta. Para manter o banco independente de
recompilações/publicações, defina `EPJB_DB_PATH` com um caminho absoluto fixo.
Bancos antigos de PCs diferentes **não são mesclados automaticamente**.

### 2. Configurar os dois clientes

Descubra o IPv4 do servidor com `ipconfig` (exemplo: `192.168.1.50`).
Em **cada PC**, edite `epjb/servidor.json` antes de compilar:

```json
{
  "Host": "192.168.1.50",
  "Porta": 11000
}
```

Se estiver usando o aplicativo já publicado, edite `servidor.json` ao lado de
`epjb.exe`. Todos os comandos usam essa mesma configuração. `localhost` só serve
quando o cliente está no próprio PC servidor.

Alternativamente, no PowerShell que iniciará o cliente:

```powershell
$env:EPJB_SERVER_HOST = "192.168.1.50"
dotnet run --project .\epjb\epjb.csproj
```

A variável tem prioridade sobre o JSON. Mantenha a janela do servidor aberta.
No Visual Studio, a solução contém os dois projetos: inicie `ServerSide` uma vez
no PC servidor e `epjb` nos PCs dos usuários.

### 3. Permitir a conexão

Os PCs precisam alcançar o IP escolhido (mesma rede local ou VPN). No Windows,
se o firewall bloquear a conexão, execute no **PC servidor**, em PowerShell
como administrador, para a rede privada local:

```powershell
New-NetFirewallRule -DisplayName "EPJB TCP 11000" -Direction Inbound -Protocol TCP -LocalPort 11000 -Action Allow -Profile Private -RemoteAddress LocalSubnet
```

No outro PC, confira:

```powershell
Test-NetConnection 192.168.1.50 -Port 11000
```

Para redes diferentes, use uma VPN entre os PCs e configure seu endereço
alcançável. O protocolo atual não possui TLS nem sessão autenticada: mantenha o
uso em rede de confiança, sem publicar a porta diretamente na internet.

### 4. Conferir na interface

1. Cadastre a pessoa A em um PC e a pessoa B no outro.
2. Faça login com as contas e publique uma mensagem em cada PC.
3. Clique em **Atualizar** para buscar o feed e a lista de usuários compartilhados.
4. Siga o outro usuário e confira a lista de seguindo.
5. Reinicie o servidor usando o mesmo caminho de banco: as contas e mensagens permanecem.

A atualização é por consulta; não há notificações automáticas de novos posts.
Os clientes não criam nem abrem arquivos SQLite.

## Validação automatizada

O teste usa o serviço de rede real do cliente e um servidor em processo separado,
com banco temporário. Verifica duas contas, chamadas simultâneas, feed comum,
seguir e persistência após reiniciar o servidor em outro diretório. Também testa
conexão recusada, configuração, framing TCP e diagnóstico UDP.
As portas 11000/TCP e 11001/UDP devem estar livres. Na raiz do projeto:

```powershell
dotnet build ServerSide/ServerSide.csproj
dotnet build tests/Integracao/Integracao.csproj
dotnet tests/Integracao/bin/Debug/net10.0/Integracao.dll "$PWD/ServerSide/bin/Debug/net10.0/ServerSide.dll"
```

## Contexto dos commits do Pedro

- `14ede2f` (26/09/2026): envelope `Mensagem`, protocolo com tamanho prefixado,
  correção da porta e teste de login local.
- `2f1358b` (27/09/2026): `ServicoApi`, conexão TCP, repositórios, handlers e telas.

Esta mudança mantém esses comandos e o protocolo. Remove o servidor temporário
iniciado em cada cliente, ativa `ServerSide` como console independente e adiciona
configuração de endereço. Os fontes de banco/servidor continuam em `epjb/` para
preservar a organização existente, mas são compilados somente por `ServerSide`.
Os documentos anteriores em `epjb/` descrevem o estágio anterior; para executar
em rede, siga este README.

Detalhes técnicos, causa e resultados dos testes: [documentação da correção](docs/CORRECAO_BANCO_COMPARTILHADO.md).
