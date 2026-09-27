# Guia de Teste - Fluxo de Login

## Pré-requisitos
1. Abrir o projeto em Visual Studio
2. Compile o projeto (Ctrl+Shift+B)
3. Verifique que o banco SQLite (epjb.db) foi criado

## Teste Manual - Fluxo Completo

### Passo 1: Preparar o Banco de Dados
1. Execute o projeto uma vez para inicializar o banco
2. Usando uma ferramenta SQLite (como DB Browser for SQLite), abra epjb.db
3. Insira um usuário de teste na tabela Usuarios:
   ```sql
   INSERT INTO Usuarios (Username, Senha, DataCriacao) 
   VALUES ('testuser', 'password123', datetime('now'));
   ```

### Passo 2: Executar o Projeto
1. Pressione F5 (ou Ctrl+F5 para rodar sem debug)
2. Você verá:
   - Console rodando o servidor (output "Aguardando conexão...")
   - Tela de Login do Windows Forms

### Passo 3: Testar Login com Sucesso
1. Na tela de Login, insira:
   - **Usuário**: testuser
   - **Senha**: password123
2. Clique no botão "Login"
3. **Resultado Esperado**:
   - Console mostra logs de autenticação
   - MessageBox: "Login realizado com sucesso!"
   - No console, você verá: "[UsuarioRepositorio] Usuário testuser autenticado com sucesso"

### Passo 4: Testar Login com Falha
1. Na tela de Login, insira:
   - **Usuário**: testuser
   - **Senha**: senhaerrada
2. Clique no botão "Login"
3. **Resultado Esperado**:
   - Console mostra: "[UsuarioRepositorio] Falha na autenticação de testuser"
   - MessageBox: "Erro: Credenciais inválidas"

### Passo 5: Testar Usuário Inexistente
1. Na tela de Login, insira:
   - **Usuário**: usuarioqueexiste
   - **Senha**: qualquersenha
2. Clique no botão "Login"
3. **Resultado Esperado**:
   - MessageBox: "Erro: Credenciais inválidas"

## Fluxo Esperado (Cliente → Servidor → Banco)
```
1. Cliente clica "Login" → ServicoApi.LoginAsync()
2. ServicoApi cria ConexaoServidor e se conecta
3. Envia Mensagem com Comando.LOGIN + json (username/password)
4. Servidor recebe em HandleClient()
5. ProcessarComando() → ProcessarLogin()
6. Desserializa JSON com JsonSerializer (case-insensitive)
7. Chama UsuarioRepositorio.Autenticar()
8. UsuarioRepositorio.Autenticar() consulta banco SQLite:
   SELECT * FROM Usuarios WHERE Username = ? AND Senha = ?
9. Se encontrado, retorna Mensagem.RespostaSucesso()
10. Se não encontrado, retorna Mensagem.RespostaErro()
11. Cliente recebe resposta
12. Se sucesso, mostra "Login realizado com sucesso!"
13. Se erro, mostra mensagem de erro
```

## Logs Esperados no Console

### Login Bem-Sucedido
```
Recebido comando: LOGIN
[UsuarioRepositorio] Usuário testuser autenticado com sucesso
```

### Login Falhado
```
Recebido comando: LOGIN
[UsuarioRepositorio] Falha na autenticação de testuser
```

## Possíveis Problemas e Soluções

| Problema | Solução |
|----------|---------|
| "Erro ao conectar" | Servidor não está rodando - verifique console |
| "Sem resposta do servidor" | Servidor caiu - verifique logs no console |
| "Credenciais inválidas" | Username ou senha incorretos no banco |
| Porta já em uso | Mude Config.Porta em Common/Config.cs |

## Próximos Passos (Fase 4)
- [ ] Adicionar testes unitários (xUnit ou NUnit)
- [ ] Implementar hash de senha (bcrypt)
- [ ] Adicionar validação de campos vazios na UI
- [ ] Implementar refresh token / session
- [ ] Separar servidor em processo independente
