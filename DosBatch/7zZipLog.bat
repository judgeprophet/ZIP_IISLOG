REM === HOT TO USE
REM === Par intervalle de date:
REM 7zZipLog.bat [START_DATE] [END_DATE] [PATH_ZIP_FILE] [PATH_FILES_TO_ZIP]
REM === Par Soustraction par rapport a la date du jour
REM 7zZipLog.bat d [NUMBER_OF_DAYS] [PATH_ZIP_FILE] [PATH_FILES_TO_ZIP]

REM ====== FORFILES =====
REM  -pPath               Path where to start searching
REM  -mSearch Mask        Search files according to <Search Mask>
REM  -cCommand            Command to execute on each file(s)
REM  -d<+|-><DDMMYYYY|DD> Select files with date >= or <=DDMMYYYY (UTC)
REM                       or files having date >= or <= (current date - DD days)
REM -s                   Recurse directories
REM ===================== 
REM EX: FORFILES -pc:\ -s -m*.BAT -c"CMD /C Echo @FILE is a batch file"

REM ====== 7-Zip =====
REM 7z <command> [<switches>...] <archive_name> [<file_names>...][<@listfiles...>]
REM a > Add file
REM -m	Set Compression Method  > 1 = Fastest
REM ==================

REM Forfiles -d+%1 -d-%2 -p%4 -m*.log -c"CMD /c echo extension of @FILE is 0x22@EXT0x22" -s

REM ============= One File at the time ==================================================================================================
REM Forfiles -d+%1 -d-%2 -p%4 -m*.log -c"0x22C:\_Dropbox\My Programs\7-Zip\7z.exe0x22 a -mx1 0x22%3\%1-%2.zip0x22 0x22@PATH\@FILE0x22" -s
REM ===================================================================================================================================== 

REM ============= Zip Files In a Oneshot ========================================================

REM === Init Variable (pas d'espace suivant le "égale", et pour un init à Vide/null uniquement le signe d'égalité est nécessaire)
Call DateAdd.bat %2

set end_yy=%date:~6,4%
set end_mm=%date:~3,2%
set end_dd=%date:~0,2%

set start_yy=%DateAdd:~6,4%
set start_mm=%DateAdd:~3,2%
set start_dd=%DateAdd:~0,2%


set _zipfilename=%end_dd%%end_mm%%end_yy%.zip



Set _ListFiles=
Set _ZipExe="C:\_Dropbox\My Programs\7-Zip\7z.exe"
Set _SetUpEnvBat=SetupEnv.Bat
Set _ForFiles=Forfiles.exe

REM === Supprime le fichier d'environnement (S'assure qu'il est inexistant)
Call :DEL_SetupEnv %4

:Main
REM === Cree le fichier de Setup Environnement
IF "%1"=="d" %_ForFiles% -d-0 -d+%2 -p%4 -m*.log -c"cmd /c echo Set _ListFiles=0x25_ListFiles0x25 @PATH\@FILE>>%_SetUpEnvBat%" -s
IF NOT "%1"=="d" %_ForFiles% /d +%1 /d -%2 /p %4 /m *.log /c "cmd /c echo Set _ListFiles=0x25_ListFiles0x25 @PATH\@FILE >> %_SetUpEnvBat%" /s

REM === Execute le fichier d'environnement
IF EXIST %4\%_SetUpEnvBat% Call %4\%_SetUpEnvBat%
REM ==========================================================================================
IF NOT "%_ListFiles%"=="" IF "%1"=="d" %_ZipExe% a -mx1 %3\%_zipfilename% %_ListFiles%
IF NOT "%_ListFiles%"=="" IF NOT "%1"=="d"  %_ZipExe% a -mx1 %3\%1-%2.zip %_ListFiles%

Call :DEL_SetupEnv %4
GOTO quit

REM =====================================================
REM === Supprime le fichier créée 
REM === PARAM : %1 = Path (correspond a %4 de la Main Batch sur le command line)
:DEL_SetupEnv
	IF EXIST %1\%_SetUpEnvBat% del %1\%_SetUpEnvBat%
goto :eof


:quit
goto :eof

