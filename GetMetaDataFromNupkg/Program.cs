using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NuGet.Packaging;

namespace GetMetaDataFromNupkg
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 2)
            {
                string pathToJson = args[0];
                string pathToPackages = args[1];

                string[] packages = Directory.GetFiles(pathToPackages);

                AllPackages allPackages = new AllPackages
                {
                    Packages = new List<Package>()
                };

                using (FileStream fs = new FileStream(pathToJson, FileMode.OpenOrCreate))
                {
                    if (fs.Length > 0)
                        allPackages = await JsonSerializer.DeserializeAsync<AllPackages>(fs);
                }

                foreach (string package in packages)
                {
                    var nupkg = new PackageArchiveReader(package);

                    string name = nupkg.NuspecReader.GetId();
                    string version = nupkg.NuspecReader.GetVersion().ToString();

                    allPackages.Packages.Add(new Package()
                    {
                        Name = name,
                        Version = version
                    });
                }

                try
                {
                    using (FileStream fs = new FileStream(pathToJson, FileMode.OpenOrCreate))
                    {
                        await JsonSerializer.SerializeAsync(fs, allPackages, new JsonSerializerOptions() { WriteIndented = true });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Console.WriteLine("Package information is saved");

            }
            else
            {
                Console.WriteLine("No arguments");
            }
        }

        public class Package
        {
            public string Name { get; set; }
            public string Version { get; set; }
        }

        public class AllPackages
        {
            public List<Package> Packages { get; set; }
        }
    }
}