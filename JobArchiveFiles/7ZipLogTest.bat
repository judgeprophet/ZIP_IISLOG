REM Get Today Date
REM en-CA
REM set end_yy=%date:~6,4%
REM set end_mm=%date:~3,2%
REM set end_dd=%date:~0,2%

REM en-US
REM set end_yy=%date:~10,4%
REM set end_mm=%date:~4,2%
REM set end_dd=%date:~7,2%

REM ArchiveFiles.exe d -70 %end_dd%%end_mm%%end_yy%  C:\Developpement\Batch C:\Developpement\IISLOG
ArchiveFiles.exe d -70 now  C:\Developpement\Batch C:\Developpement\IISLOG
pause