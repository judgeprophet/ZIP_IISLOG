REM @ECHO OFF
REM ECHO.

:: Check Windows version (XP Pro or later)
IF NOT "%OS%"=="Windows_NT" GOTO Syntax

:: Check command line argument (one mandatory)
IF     "%~1"=="" GOTO Syntax
IF NOT "%~2"=="" GOTO Syntax

:: Check if help is required
ECHO.%1 | FINDSTR.EXE /R /C:"[/?\.]" >NUL && GOTO Syntax

:: Check if WMIC is available
WMIC.EXE Alias /? >NUL 2>&1 || GOTO Syntax

:: Localize variables
SETLOCAL ENABLEDELAYEDEXPANSION

:: Fill array with valid arguments
SET DatePart.d=Day
SET DatePart.Day=Day

SET DatePart.DayOfWeek=DayOfWeek
SET DatePart.w=DayOfWeek

SET DatePart.h=Hour
SET DatePart.Hour=Hour

SET DatePart.n=Minute
SET DatePart.Minute=Minute

SET DatePart.m=Month
SET DatePart.Month=Month

SET DatePart.q=Quarter
SET DatePart.Quarter=Quarter

SET DatePart.s=Second
SET DatePart.Second=Second

SET DatePart.yyyy=Year 
SET DatePart.Year=Year

:: Check if command line argument is listed in array
SET DatePart. | FINDSTR /R /I /C:"\.%~1=" >NUL
IF ERRORLEVEL 1 (
    ENDLOCAL
    GOTO Syntax
)

:: Initialize variable
SET Error=0

:: Use WMIC to display the requested part of the date or time
FOR /F "skip=1" %%A IN ('WMIC Path Win32_LocalTime Get !DatePart.%~1! /Format:table 2^>NUL ^|^| SET Error=1') DO SET DatePart=%%A
ECHO.%DatePart%

:: Check for errors trapped by WMIC
IF "%Error%"=="1" (
    ENDLOCAL
    GOTO Syntax
)

:: Done
ENDLOCAL & SET DatePart=%DatePart%
GOTO:EOF


:Syntax
ECHO DatePart.bat, Version 2.01 for Windows XP Pro and later
ECHO Returns the specified part of the current date or time
ECHO.
ECHO Usage:  DATEPART  option
ECHO.
ECHO Where:  option(s)               display
ECHO.        =================       ============
ECHO         d    or Day             day of month
ECHO         w    or DayOfWeek       day of week
ECHO         h    or Hour            hour
ECHO         n    or Minute          minutes
ECHO         m    or Month           month
ECHO         q    or Quarter         quarter
ECHO         s    or Second          seconds
ECHO         yyyy or Year            year
ECHO.
ECHO Notes:  All values returned are numeric, without leading zeros.
ECHO         The requested value is displayed on screen and stored
ECHO         in an environment variable %%DatePart%%.
ECHO.
ECHO Written by Rob van der Woude
ECHO http://www.robvanderwoude.com