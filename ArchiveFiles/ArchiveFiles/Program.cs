using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArchiveFiles
{
    class Program
    {
        static string _archiveExt = Properties.Settings.Default.ArchiveFileExtension;
        static string _filterMask = Properties.Settings.Default.FilterMask;
        static string _archiveExePath = Properties.Settings.Default.ArchiveExePath;

        static string _scanPath; //Repertoire de recherche
        static string _ouputFile; // Repertoire ou sera créé
        static string _optionFlag; // Type de recherche
        static string _startDate; // Date de début
        static string _endDate; // Date de fin

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
                    throw new ArgumentException(Resource1.ERR_ARG);
                }

                if (args[0].ToUpper() != "D" && args[0].ToUpper() != "R")
                {
                    throw new Exception(Resource1.ERR_WRONG_OPTION_FLAG);
                }

                if (!Directory.Exists(args[3]))
                {
                    throw new System.IO.DirectoryNotFoundException(Resource1.ERR_NO_DEST_PATH);
                }

                if (!Directory.Exists(args[4]))
                {
                    throw new System.IO.DirectoryNotFoundException(Resource1.ERR_NO_SOURCE_PATH);
                }

                #endregion

                _optionFlag = args[0];
                _startDate = args[1];
                _endDate = args[2];
                _scanPath = args[4];

                DateTime endDate = Convert.ToDateTime(_endDate.Substring(0, 2) + "-" + _endDate.Substring(2, 2) + "-" + _endDate.Substring(4, 4));
                DateTime startDate = DateTime.Now;
                if (_optionFlag.ToUpper() == "R")
                {
                    startDate = Convert.ToDateTime(_startDate.Substring(0, 2) + "-" + _startDate.Substring(2, 2) + "-" + _startDate.Substring(4, 4));

                }
                if (_optionFlag.ToUpper() == "D")
                {
                    startDate = endDate.AddDays(Convert.ToInt64(_startDate));
                }



                string archiveFileName = startDate.ToString("ddMMyyyy") + "-" + endDate.ToString("ddMMyyyy") + _archiveExt;
                _ouputFile = args[3] + ((args[3].EndsWith("\\")) ? "" : "\\") + archiveFileName;

                //_scanPath = @"C:\Developpement\IISLOG\";

                //Filtre pour les fichier rpt dans le répertoire et les sous-répertoires
                DirectoryInfo rootDir = new DirectoryInfo(_scanPath);
                var files = rootDir.GetFiles(_filterMask, SearchOption.AllDirectories).Where(f => f.LastWriteTime >= startDate && f.LastWriteTime <= endDate);

                //pour chaque fichier trouvé on lit la version du rapport et on l'inscrit dans le fichier
                foreach (var file in files)
                {
                    liftFilesToZip += file.FullName + " ";
                }

                string strCmdText = " a -mx1 " + _ouputFile + " " + liftFilesToZip;

                string _Output = null;
                string _Error = null;
                ExecuteShell.ExecuteShellCommand(_archiveExePath, strCmdText, ref _Output, ref _Error);

            }
            catch (Exception ex)
            {
                Console.WriteLine("ArchiveFiles.exe [d NB_OF_DAYS]|[r START_DATE] [END_DATE] [PATH_ZIP_FILE] [PATH_FILES_TO_ZIP]");
                Console.WriteLine("[START_DATE] [END_DATE] Must be in format DDMMYYYY");
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
                Console.WriteLine("");
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.WriteLine("ERREUR : " + ex.Message);
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.WriteLine("");
            }


        }
    }
}
