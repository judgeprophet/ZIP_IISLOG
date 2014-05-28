
REM GetDate
REM SET the EndDate Variable Name : LDate
REM Call DateAdd.bat -7

REM set end_yy=%date:~6,4%
REM set end_mm=%date:~3,2%
REM set end_dd=%date:~0,2%

REM set start_yy=%DateAdd:~6,4%
REM set start_mm=%DateAdd:~3,2%
REM set start_dd=%DateAdd:~0,2%

REM NOTE ForFile.EXE must be present and accessible from prompt (windows Path environnement)
REM 7zZipLog.bat [START_DATE] [END_DATE] [PATH_ZIP_FILE] [PATH_FILES_TO_ZIP]
REM Call 7zZipLog.bat 01032014 07032014 C:\Developpement\Batch\ C:\Developpement\IISLOG\
Call 7zZipLog.bat d 7 C:\Developpement\Batch\ C:\Developpement\IISLOG\
pause