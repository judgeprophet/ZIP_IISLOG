using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ArchiveFiles
{
    //TODO Refactor to try to put all the Validation in the same place,  For Now Validation is All over the place
    //TODO Validate is all the settings are present in the app.config
    //TODO Add Format Date to app.config
    //TOOO Add a setting in app.config for Prefix and/or Suffize of the Zip file
    //TODO Add Flag for a better handling of the console output, for those who want to log the output
    class Program
    {
        static string _archiveExt = Properties.Settings.Default.ArchiveFileExtension;
        static string _filterMask = Properties.Settings.Default.FilterMask;
        static string _archiveExePath = Properties.Settings.Default.ArchiveExePath;
        static string _archiveExeOption = Properties.Settings.Default.ArchiveExeOption;

        static string _scanPath; //Repertoire de recherche
        static string _ouputFile; // Repertoire ou sera créé
        static string _optionFlag; // Type de recherche
        static string _startDate; // Date de début
        static string _endDate; // Date de fin

        const string dateFormat = "ddMMyyyy";

        //All Possible Flag accepted on the command line
        public enum optionFlags
        {
            [Description("R")]
            Range,
            [Description("D")]
            Days,
            [Description("NOW")]
            TodayDate
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">[OPTION_FLAG] [START_DATE] [END_DATE] [PATH_ZIP_FILE] [PATH_FILES_TO_ZIP]</param>
        static void Main(string[] args)
        {
            try
            {
                string liftFilesToZip = "";

                #region validation

                //Parametre sur la ligne de commande est obligatoire
                if (args.Length < 5)
                {
                    throw new ArgumentException(Resource1.ERR_ARG_EXPECTED);
                }

                if (!File.Exists(_archiveExePath))
                {
                    throw new ArgumentException(Resource1.ERR_ZIPEXE_NOT_EXISTS);
                }

                if (args[0].ToUpper() != optionFlags.Days.GetDescription() && args[0].ToUpper() != optionFlags.Range.GetDescription()) //Invariant Case
                {
                    throw new Exception(Resource1.ERR_WRONG_OPTION_FLAG);
                }

                if (!Directory.Exists(args[3]))
                {
                    throw new System.IO.DirectoryNotFoundException(Resource1.ERR_NO_DEST_PATH + " " +  args[3]);
                }

                if (!Directory.Exists(args[4]))
                {
                    throw new System.IO.DirectoryNotFoundException(Resource1.ERR_NO_SOURCE_PATH + " " + args[4]);
                }

                #endregion

                _optionFlag = args[0];
                _startDate = args[1];
                _endDate = args[2];
                _scanPath = args[4];

                DateTime endDate = DateTime.Now;
                DateTime startDate = DateTime.Now;

                //Validate and Set Date in the proper expected Format, If the keyword NOW is entered.  We use Today's Date
                if (_endDate.ToUpper() != optionFlags.TodayDate.GetDescription() && !DateTime.TryParseExact(_endDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out endDate))
                    throw new ArgumentException(Resource1.ERR_WRONG_DATE_FORMAT);

                if (_optionFlag.ToUpper() == optionFlags.Range.GetDescription()) //Invariant Case
                {
                    //=======================================================================================
                    //using the RANGE command line option.  Looking for file between two date

                    //Validate and Set Date in the proper expected Format, If the keyword NOW is entered.  We use Today's Date
                    if (_startDate.ToUpper() != optionFlags.TodayDate.GetDescription() && !DateTime.TryParseExact(_startDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate))
                        throw new ArgumentException(Resource1.ERR_WRONG_DATE_FORMAT);
                }
                else if (_optionFlag.ToUpper() == optionFlags.Days.GetDescription()) //Invariant Case
                {
                    //=======================================================================================
                    //using the DAY command line option.  Looking for file older or newer than a specified date
                    long nbDays;
                    if (!long.TryParse(_startDate, out nbDays))
                        throw new ArgumentException(Resource1.ERR_NB_DAYS_INTEGER);

                    startDate = endDate.AddDays(Convert.ToInt64(nbDays));
                }
                
                string archiveFileName = startDate.ToString(dateFormat) + "-" + endDate.ToString(dateFormat) + _archiveExt;
                _ouputFile = Path.Combine(args[3], archiveFileName);

                //Filtre pour les fichier rpt dans le répertoire et les sous-répertoires
                DirectoryInfo rootDir = new DirectoryInfo(_scanPath);
                var files = rootDir.GetFiles(_filterMask, SearchOption.AllDirectories).Where(f => f.LastWriteTime >= startDate && f.LastWriteTime <= endDate);

                //pour chaque fichier trouvé on lit la version du rapport et on l'inscrit dans le fichier
                foreach (var file in files)
                {
                    liftFilesToZip += file.FullName + " ";
                }

                if (String.IsNullOrEmpty(liftFilesToZip))
                {
                    Console.WriteLine(Resource1.NO_FILES_TO_ARCHIVE);
                }
                else
                {
                    string strCmdText = " " + _archiveExeOption + " " + _ouputFile + " " + liftFilesToZip;

                    string _Output = null;
                    string _Error = null;
                    ExecuteShell.ExecuteShellCommand(_archiveExePath, strCmdText, ref _Output, ref _Error);
                    Console.WriteLine(_archiveExePath + " " + strCmdText);

                    if (!String.IsNullOrEmpty(_Error))
                    {
                        throw new Exception(_Error);
                    }

                    Console.WriteLine(_Output);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ArchiveFiles.exe [d NB_OF_DAYS]|[r START_DATE] [END_DATE] [PATH_ZIP_FILE] [PATH_FILES_TO_ZIP]");
                Console.WriteLine("[START_DATE] [END_DATE] Must be in format DDMMYYYY OR use the 'now' keyword if you want to use today's date");
                Console.WriteLine();
                Console.WriteLine(@"The First Parameter [d|r]  stand for d=days OR r=range.");
                Console.WriteLine(@"If 'd' is specified the next parameter must be how much days to Add or Substract from the [END_DATE] parameter");
                Console.WriteLine(@"If 'r' is specified a [START_DATE] and [END_DATE] is expected as parameters");
                Console.WriteLine();
                Console.WriteLine(@"EX: ArchiveFiles r 01012014 01052014 C:\Archives\ C:\IISLOG\");
                Console.WriteLine(@"Create a archive in folder 'C:\Archive' with log located in 'C:\IISLOG\' modified between January 1st 2014 and Mai 1st 2014");
                Console.WriteLine();
                Console.WriteLine(@"EX: ArchiveFiles d -7 01052014 C:\Archives\ C:\IISLOG\");
                Console.WriteLine(@"Create a archive in folder 'C:\Archive' with log located in 'C:\IISLOG\' modified between Mai 1st 2014 and seven days before that");
                Console.WriteLine();
                Console.WriteLine(@"EX: ArchiveFiles r 01052014 now C:\Archives\ C:\IISLOG\");
                Console.WriteLine(@"Create a archive in folder 'C:\Archive' with log located in 'C:\IISLOG\' modified between Mai 1st 2014 and today");
                Console.WriteLine("");
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.WriteLine("ERROR : " + ex.Message);
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.WriteLine("");
            }


        }
    }
}
