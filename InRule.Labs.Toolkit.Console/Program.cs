using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InRule.Labs.Toolkit;
using InRule.Labs.Toolkit.Shared;
using InRule.Labs.Toolkit.Shared.Model;

namespace InRule.Labs.Toolkit.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Helper h = new Helper();
            string path = "";
            if (args == null || args.Length == 0)
            {
                path = Directory.GetCurrentDirectory();
            }
            else
            {
                path = args[0];
            }
            ObservableCollection<ArtifactCount> result = new ObservableCollection<ArtifactCount>();
            h.CountArtifactsByTypeBatch(path, result);
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(path + "\\summary.csv");
            foreach (ArtifactCount item in result)
            {
                file.WriteLine("\"" + item.ArtifcatType + "\", " + item.Count);
            }
            file.Close();
        }
    }
}
