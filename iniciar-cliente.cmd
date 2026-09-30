@echo off
setlocal
REM Este script abre somente o cliente; execute iniciar-servidor.cmd antes, no PC central.
REM Uso opcional: iniciar-cliente.cmd 192.168.1.50 (substitua pelo IPv4 do servidor).
if not "%~1"=="" set "EPJB_SERVER_HOST=%~1"
pushd "%~dp0"
echo Abra Servidor... na tela de login para configurar e testar a conexao.
dotnet run --project "epjb\epjb.csproj"
if errorlevel 1 pause
popd
endlocal
