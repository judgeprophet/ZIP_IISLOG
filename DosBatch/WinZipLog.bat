REM -es: super fast Set the compression level
REM -t[f|d][date] Include files with a date EQUAL TO OR more recent than the date specified  
REM -T[f|d][date] Include files OLDER THAN the date specified (current date if no date specified)  
REM        note - The f modifier indicates that the date is specified in either of the following country-independent formats
REM -^^ Display the WZZIP command line on your screen
REM
REM USE  ZipLog [START_DATE] [END_DATE] [PATH_ZIP_FILE] [PATH_FILES_TO_ZIP]
REM Example C:\zipLog -t20140101 -T20140501 d:\temp\ d:\log\
REM Zip all d:\log\.log file in an archive named "d:\temp\20140101-20140501.zip" where their date are between  january 1 2014 and  mai 1 2014.  The zip file is named according to the specified dates

wzzip -^^ -es -tf%1 -Tf%2 %3\%1-%2.zip %4\*.log %5 %6 %7 %8 %9