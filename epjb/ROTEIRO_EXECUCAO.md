> **Atualização — banco compartilhado:** este documento registra o estágio anterior à separação do servidor. Para executar em dois PCs, siga o [README atual](../README.md) e a [documentação da correção](../docs/CORRECAO_BANCO_COMPARTILHADO.md). O cliente não inicia mais um servidor local.

# 🚀 ROTEIRO DE EXECUÇÃO - Twitter Simplificado

## 📋 Checklist Pré-Execução

Antes de começar, verifique:

- [ ] Visual Studio 2026 instalado
- [ ] .NET 10 instalado
- [ ] Git instalado
- [ ] Projeto clonado: `https://github.com/Pedrolbtb/EP-Redes-Twitter`
- [ ] Pasta: `C:\Users\pedro\source\repos\epjb\`

---

## 🔧 Instalação do Projeto (Primeira Vez)

### **Passo 1: Clonar Repositório**
```bash
git clone https://github.com/Pedrolbtb/EP-Redes-Twitter.git
cd EP-Redes-Twitter
```

### **Passo 2: Abrir Solução no Visual Studio**
1. Abra Visual Studio
2. **File** → **Open** → `epjb.slnx`
3. Aguarde carregar...

### **Passo 3: Restaurar Pacotes NuGet**
```bash
# Via Package Manager Console
Update-Package -Reinstall

# Ou via terminal
dotnet restore
```

### **Passo 4: Compilar Projeto**
```bash
# Ctrl+Shift+B no Visual Studio
# Ou via terminal
dotnet build
```

Se não houver erros, parabéns! ✅

### **Passo 5: Criar Banco de Dados**
```bash
# Via Package Manager Console
Update-Database

# Ou via terminal
dotnet ef database update
```

Você verá um arquivo `epjb.db` criado.

---

## ▶️ Executar a Aplicação

### **Método 1: Visual Studio (Recomendado)**
1. Pressione **F5** (Start Debugging)
   - Ou: **Debug** → **Start Debugging**
   - Ou: Clique no botão ▶️ Play na toolbar

2. Você verá:
   - Console mostrando: `"Aguardando conexão..."`
   - Tela de Login aparecendo

### **Método 2: Linha de Comando**
```bash
cd C:\Users\pedro\source\repos\epjb\
dotnet run
```

---

## 🎮 Teste Manual - Passo a Passo

### **Cenário 1: Cadastrar Novo Usuário**

```
1. Aplicação inicia
   └─ Vê tela de Login

2. Clique em "Cadastro"
   └─ Abre tela de Cadastro

3. Preencha:
   ├─ Username: pedro
   ├─ Senha: senha123
   └─ Confirmar Senha: senha123

4. Clique "Cadastrar"
   └─ Mensagem: "Cadastro concluído!"
   └─ Volta ao Login

5. Veja no console:
   └─ [UsuarioRepositorio] Usuário pedro registrado com sucesso
```

**Esperado**: ✅ Cadastro criado no banco

---

### **Cenário 2: Login com Sucesso**

```
1. Tela de Login

2. Preencha:
   ├─ Username: pedro
   └─ Senha: senha123

3. Clique "Login"
   └─ Botão mostra "Autenticando..."
   └─ Aguarde resposta do servidor...

4. Mensagem: "Login realizado com sucesso!"

5. Tela Menu abre com:
   ├─ "Bem-vindo, pedro!" no topo
   ├─ Timeline vazia
   ├─ Dropdown de usuários
   └─ Lista "Meus Seguindo" vazia

6. Veja no console:
   ├─ [UsuarioRepositorio] Usuário pedro autenticado com sucesso
   ├─ [MensagemRepositorio] Listadas 0 mensagens
   └─ [UsuarioRepositorio] Listados X usuários
```

**Esperado**: ✅ Login bem-sucedido, Menu aberto

---

### **Cenário 3: Login com Falha**

```
1. Tela de Login

2. Preencha:
   ├─ Username: usuarioequenaoexiste
   └─ Senha: senhaerrada

3. Clique "Login"

4. Mensagem: "Erro: Credenciais inválidas"
   └─ Fica na tela de Login

5. Veja no console:
   └─ [UsuarioRepositorio] Falha na autenticação de usuarioequenaoexiste
```

**Esperado**: ✅ Rejeita login, permanece na tela Login

---

### **Cenário 4: Postar Mensagem**

```
1. Esteja logado (veja Menu aberto)

2. Na área "NOVA MENSAGEM":
   └─ Escreva: "Olá! Este é meu primeiro tweet!"

3. Clique "Postar"
   └─ Mensagem: "Mensagem postada com sucesso!"
   └─ Campo fica vazio
   └─ Timeline atualiza com sua mensagem

4. Veja no console:
   ├─ [ServicoApi] Executando comando: POSTAR_MSG
   ├─ [MensagemRepositorio] Mensagem criada com ID X
   └─ [MensagemRepositorio] Listadas 1 mensagens

5. Na Timeline, veja:
   ├─ ID: 1
   ├─ IdUsuario: [seu ID]
   ├─ Username: pedro
   ├─ Conteudo: "Olá! Este é meu primeiro tweet!"
   └─ DataCriacao: [data/hora atual]
```

**Esperado**: ✅ Mensagem aparece na timeline

---

### **Cenário 5: Seguir Usuário**

```
1. Esteja logado

2. Na área "SEGUIR USUÁRIOS":
   ├─ Clique no Dropdown
   └─ Selecione outro usuário (ex: "admin")

3. Clique "Seguir"
   └─ Mensagem: "Usuário seguido com sucesso!"

4. Em "MEUS SEGUINDO":
   └─ Vê "admin" na lista

5. Veja no console:
   └─ [SeguidorRepositorio] Usuário [seu ID] agora segue [admin ID]
```

**Esperado**: ✅ Usuário adicionado à lista "Meus Seguindo"

---

### **Cenário 6: Deletar Mensagem**

```
1. Esteja logado com mensagens postadas

2. Na Timeline:
   └─ Selecione sua mensagem (clique na linha)

3. Clique "Deletar"

4. Confirmação: "Tem certeza que deseja deletar esta mensagem?"
   └─ Clique "Sim"

5. Mensagem: "Mensagem deletada com sucesso!"
   └─ Mensagem some da timeline

6. Veja no console:
   └─ [MensagemRepositorio] Mensagem X deletada

7. Tente deletar mensagem de outro usuário:
   └─ Mensagem de erro: "Você só pode deletar suas próprias mensagens."
```

**Esperado**: ✅ Apenas próprias mensagens podem ser deletadas

---

### **Cenário 7: Logout**

```
1. Esteja no Menu

2. Clique "Logout" (canto superior direito)

3. Menu fecha

4. Volta à tela de Login

5. Está vazio e pronto para novo login
```

**Esperado**: ✅ Logout bem-sucedido, volta ao Login

---

## 🐛 Troubleshooting

### **Erro: "Falha na compilação"**

**Solução**:
```bash
# Limpar solução
dotnet clean

# Restaurar packages
dotnet restore

# Recompilar
dotnet build
```

---

### **Erro: "Nenhuma resposta do servidor"**

**Causas possíveis**:
- [ ] Servidor não iniciou (veja console)
- [ ] Porta 4343 já está em uso

**Solução**:
```bash
# Mude a porta em Common/Config.cs
public const int Porta = 4344;  // Troque de 4343 para 4344
```

---

### **Erro: "Sem resposta do servidor"**

**Causas possíveis**:
- [ ] Conexão foi perdida
- [ ] Servidor crashou
- [ ] Timeout (muito lento)

**Solução**:
1. Verifique console do servidor
2. Reinicie a aplicação (F5)
3. Aumente timeout em ConexaoServidor.cs

---

### **Erro: "Banco de dados não encontrado"**

**Causas possíveis**:
- [ ] Migrations não foram rodadas
- [ ] Arquivo epjb.db não foi criado

**Solução**:
```bash
# Rodar migrations
dotnet ef database update

# Ou remover e recriar
rm epjb.db
dotnet ef database update
```

---

### **Erro: "Username já existe"**

**Causa**:
- Username já foi cadastrado antes

**Solução**:
1. Use outro username
2. Ou delete usuário do banco:
```sql
DELETE FROM Usuarios WHERE Username = 'pedro';
```

---

## 📊 Estrutura de Diretórios

```
epjb/
├── bin/                          # Binários compilados
├── obj/                          # Arquivos intermediários
│
├── Cliente/Rede/
│   ├── ConexaoServidor.cs
│   └── ServicoApi.cs
│
├── Common/
│   ├── Comando.cs
│   ├── Config.cs
│   ├── Mensagem.cs
│   ├── Protocolo.cs
│   └── DTO/
│
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
│
├── Models/
│   ├── Usuario.cs
│   ├── Mensagem.cs
│   └── UsuarioSeguidor.cs
│
├── Repositorio/
│   ├── UsuarioRepositorio.cs
│   ├── MensagemRepositorio.cs
│   └── SeguidorRepositorio.cs
│
├── Sockets/
│   └── Class1.cs
│
├── View/
│   ├── Login.cs & Login.Designer.cs
│   ├── Menu.cs & Menu.Designer.cs
│   ├── Cadastro.cs & Cadastro.Designer.cs
│   ├── Login.resx
│   └── Menu.resx
│
├── Program.cs
├── epjb.csproj
├── epjb.slnx
├── epjb.db                       # Banco SQLite (criado na primeira execução)
│
├── ARQUITETURA.md
├── BIBLIOTECAS.md
├── DIVISAO_TAREFAS.md
├── PROTOCOLO_TCP.md              # (A criar)
├── ROTEIRO_EXECUCAO.md           # Este arquivo
├── STATUS_IMPLEMENTACAO.md
└── TESTE_LOGIN.md
```

---

## 📝 Console Output Esperado

### **Inicial**
```
Aguardando conexão...
[ServicoApi] Executando comando: LOGIN
[UsuarioRepositorio] Usuário pedro autenticado com sucesso
[Servidor] Resposta enviada
```

### **Ao Postar Mensagem**
```
[ServicoApi] Executando comando: POSTAR_MSG
[MensagemRepositorio] Mensagem criada com ID 5
[MensagemRepositorio] Listadas 5 mensagens
```

### **Ao Deletar Mensagem**
```
[ServicoApi] Executando comando: DELETAR_MSG
[MensagemRepositorio] Mensagem 5 deletada
```

---

## 🔍 Verificar Banco de Dados

### **Método 1: DB Browser for SQLite**
1. Download: https://sqlitebrowser.org/
2. Abra `epjb.db`
3. Veja tabelas: `Usuarios`, `Mensagens`, `UsuarioSeguidores`

### **Método 2: Command Line**
```bash
sqlite3 epjb.db

# Ver tabelas
.tables

# Ver usuarios
SELECT * FROM Usuarios;

# Ver mensagens
SELECT * FROM Mensagens;

# Ver seguidores
SELECT * FROM UsuarioSeguidores;

# Sair
.quit
```

### **Método 3: Visual Studio**
1. **View** → **SQL Server Object Explorer**
2. Adicione conexão ao `epjb.db`
3. Browse tabelas

---

## 📱 Teste de Múltiplos Clientes

Se quiser testar com 2+ clientes simultâneos:

```bash
# Terminal 1: Aplicação principal
dotnet run

# Terminal 2: Segundo cliente
dotnet run
```

Ambas se conectarão ao mesmo servidor!

---

## ⚙️ Configurações Ajustáveis

### **Porta (Config.cs)**
```csharp
public class Config
{
	public const int Porta = 4343;  // Mude aqui
}
```

### **Timeout (ConexaoServidor.cs)**
```csharp
public void Conectar()
{
	socket.ConnectAsync(...);
	// Adicione timeout se necessário
}
```

### **Limite de Caracteres (Menu.cs)**
```csharp
if (conteudo.Length > 500)  // Mude para outro valor
{
	MessageBox.Show("Máximo 500 caracteres");
}
```

---

## 🎬 Demonstração Prática (15 minutos)

**Para apresentar ao professor**, siga este roteiro:

### **0-1 min: Explicação**
- "Este é um Twitter simplificado"
- "Usa TCP puro (System.Net.Sockets)"
- "Banco de dados SQLite"

### **1-3 min: Cadastro**
- Clique Cadastro
- Preencha dados
- Clique Cadastrar
- Mostra sucesso

### **3-5 min: Login**
- Preencha credenciais
- Clique Login
- Abre Menu

### **5-8 min: Postar**
- Escreva mensagem
- Clique Postar
- Vê na timeline

### **8-10 min: Seguir**
- Selecione usuário
- Clique Seguir
- Vê na lista "Meus Seguindo"

### **10-12 min: Deletar**
- Selecione mensagem
- Clique Deletar
- Confirma
- Mensagem some

### **12-14 min: Logout**
- Clique Logout
- Volta ao Login

### **14-15 min: Mostar Console**
- Mostre logs no console
- Explique fluxo TCP

---

## ✅ Checklist Final

Antes de apresentar:

- [ ] Aplicação compila sem erros
- [ ] Banco de dados existe (epjb.db)
- [ ] Servidor inicia (vê "Aguardando conexão...")
- [ ] Login funciona
- [ ] Menu abre corretamente
- [ ] Postar mensagem funciona
- [ ] Timeline atualiza
- [ ] Seguir usuário funciona
- [ ] Deletar mensagem funciona (apenas próprias)
- [ ] Logout funciona
- [ ] Console mostra logs
- [ ] Não há erros em tempo de execução

---

## 🤖 Automação (Opcional)

Se quiser automatizar testes, crie um script PowerShell:

```powershell
# build-and-run.ps1
Write-Host "Compilando..."
dotnet build

Write-Host "Rodando testes..."
dotnet test

Write-Host "Iniciando aplicação..."
dotnet run
```

Execute:
```bash
.\build-and-run.ps1
```

---

## 📞 Suporte

Se algo der errado:

1. Verifique `TESTE_LOGIN.md`
2. Leia `ARQUITETURA.md`
3. Revise `DIVISAO_TAREFAS.md`
4. Consulte `BIBLIOTECAS.md`
5. Check `STATUS_IMPLEMENTACAO.md`

---

**Boa sorte na apresentação! 🚀**

Data: Janeiro 2025  
Projeto: Twitter Simplificado  
Disciplina: Redes (EACH-USP)
