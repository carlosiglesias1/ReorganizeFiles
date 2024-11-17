using System.Text.RegularExpressions;

internal class Program
{
    internal static string[] MonthNames = {
        "Enero",
        "Febrero",
        "Marzo",
        "Abril",
        "Mayo",
        "Junio",
        "Julio",
        "Agosto",
        "Septiembre",
        "Octubre",
        "Noviembre",
        "Diciembre"
    };

    private static void Main(string[] args)
    {
        string rootDirectoryPath;

        Console.WriteLine("Hello, World!");

        Console.WriteLine("Introduzca el directorio raíz:");
        rootDirectoryPath = Console.ReadLine() ?? "";

        if (Directory.Exists(rootDirectoryPath))
            ReorganizeDirectory(Directory.GetFileSystemEntries(rootDirectoryPath), rootDirectoryPath);
        else
            Console.WriteLine("El directorio no existe");
    }

    private static void ReorganizeDirectory(string[] dirFiles, string rootPath)
    {
        foreach (var file in dirFiles)
        {
            var attrs = File.GetAttributes(file);
            if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
                ReorganizeDirectory(Directory.GetFileSystemEntries(file), rootPath);
            else
            {
                var fileInfo = new FileInfo(file);
                var destinationFolder = $"{rootPath}/{fileInfo.CreationTime.Year}/{fileInfo.CreationTime.Month.ToString().PadLeft(2, '0')}_{MonthNames[fileInfo.CreationTime.Month - 1]}";
                if (FileNameHasDateFormat(fileInfo.Name))
                {
                    destinationFolder = $"{rootPath}/{fileInfo.Name.Substring(4, 4)}/{fileInfo.Name.Substring(8, 2)}_{MonthNames[int.Parse(fileInfo.Name.Substring(8, 2)) - 1]}";
                }
                var destinationPath = destinationFolder + "/" + fileInfo.Name;

                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);
                try
                {
                    File.Move(file, destinationPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"No se ha podido mover el archivo {fileInfo.FullName} a la carpeta {destinationFolder}: {e.Message}");
                }
            }
        }
    }

    private static bool FileNameHasDateFormat(string fileName)
    {
        var usefulName = fileName.Length > 19 ? fileName[..19] : fileName;
        return Regex.IsMatch(usefulName, @$"^IMG_(\d{{8}})_(\d{{6}})") || Regex.IsMatch(usefulName, @$"^IMG-(\d{{8}})-.+");
    }
}