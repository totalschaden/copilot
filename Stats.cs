using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot
{
    class Stats
    {
        // Was for Livestream Purposes, updating Kills made
        // Recommend to use a Ram Drive if you want to use this at some point.
        CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
        

        public void WriteToFile()
        {

            string fileName = @"S:\Stats.txt";
            FileInfo fi = new FileInfo(fileName);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("Last Update: {0}", DateTime.Now.ToString("T", culture));
                    sw.WriteLine("Kills Normal: ");
                    sw.WriteLine("Kills Elite: ");
                    sw.WriteLine("Kills Rare: ");
                    sw.WriteLine("Kills Unique: ");
                }

                // Write file contents on console.     
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                        
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }
        public void Update()
        {
            WriteToFile();
        }
    }
}
