@echo off
setlocal
REM Executar uma unica vez no PC que guarda o banco. Manter esta janela aberta.
REM pushd permite iniciar por duplo clique mesmo quando o diretorio atual e outro.
pushd "%~dp0"
echo Iniciando o servidor central. Aguarde "Servidor TCP aguardando clientes".
echo O caminho do banco sera mostrado abaixo. EPJB_DB_PATH permite escolher um banco existente.
dotnet run --project "ServerSide\ServerSide.csproj"
REM Se faltar SDK, houver erro de banco ou porta ocupada, a janela preserva a mensagem para leitura.
pause
popd
endlocal
