using System;
using System.Configuration;
using System.IO;

namespace BradyChallenge.InputOutputOperations
{
    /// <summary>
    /// The class to initite pickup input file when it is available and to initiate the remaining operations
    /// </summary>
    class PickupInputFile : IPickupInputFile
    {
        #region Fields
        const string INPUT_FILE_NAME = "01-Basic.xml";
        static readonly string InputFolder = ConfigurationManager.AppSettings["InputPath"];
        static IOperations OperationsObject;
        #endregion

        /// <summary>
        /// Method to watch for the input xml file to pick it up when it is available in the specified path
        /// </summary>
        /// <param name="inputFolder">path to the input folder to be watched.</param>
        public void WatchForInputFile()
        {
            if(string.IsNullOrEmpty(InputFolder))
            {
                // throw error
            }
            try
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = InputFolder;
                watcher.NotifyFilter = NotifyFilters.Attributes |
                                        NotifyFilters.CreationTime |
                                        NotifyFilters.DirectoryName |
                                        NotifyFilters.FileName |
                                        NotifyFilters.LastAccess |
                                        NotifyFilters.LastWrite |
                                        NotifyFilters.Security |
                                        NotifyFilters.Size;
                // Watch xml files.
                watcher.Filter = INPUT_FILE_NAME;

                // Add event handlers. Whenever an xml file is created under the spefied folder, this event handler is invoked.
                watcher.Created += new FileSystemEventHandler(OnChanged);

                //Start monitoring.  
                watcher.EnableRaisingEvents = true;
                
                Console.WriteLine("Press \'q\' to quit the program.");
                Console.WriteLine();
                //Make an infinite loop till 'q' is pressed.  
                while (Console.Read() != 'q') ;
            }
            catch(IOException e)
            {
                Console.WriteLine("An Exception Occurred :" + e);
            }
            catch (Exception oe)
            {
                Console.WriteLine("An Exception Occurred :" + oe);
            }
        }

        /// <summary>
        /// Method to handle the actions performed when an input file is being created. 
        /// </summary>
        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
            OperationsObject = new Operations();
            OperationsObject.OperationsToPerform(e);
        }      
    }
}
