﻿using Grove.Media;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Grove.Utils
{
  public class PrepareImages : Task
  {
    private HashSet<string> OldStyle = new HashSet<string>(new []
    {
      "lea", "leb", "2ed", "arn", "atq", "3ed", "leg" , "drk", "fem", "4ed", "ice", "chr", "hml",
      "all", "mir", "vis", "5ed", "por", "wth", "tmp", "sth", "exo", "p02", "ugl", "usg", "ath", 
      "ulg", "6ed", "ptk", "uds", "s99", "mmq", "brb", "nem", "s00", "pcy", "inv", "btd", "pls",
      "7ed", "apc","ody", "dkm", "tor", "jud", "ons", "lgn", "scg"
    });    

    private readonly Regex CardNameRegex = new Regex(@"^([^\\//\.]+)[\\\/]([^\\//\.]+).*\.jpg");

    public class IndexEntry
    {
      public string CardName;
      public string Edition;
      public string Path;
      public string PathZip;      
    }    
    
    public override bool Execute(Arguments arguments)
    {
      var sourceFolder = arguments["s"];
      var outputFolder = arguments["o"];
      var outputFolderExtract = Path.Combine(outputFolder, @"extract");

      if (!Directory.Exists(outputFolderExtract))
      {
        Directory.CreateDirectory(outputFolderExtract);
      }

      var existingImages = new HashSet<string>(
        MediaLibrary.Folders.Cards
        .ReadAll()
        .Select(x => x.Name.Substring(0, x.Name.Length - 4).ToLowerInvariant())
        .ToArray());

      var missingImages = Cards.All
        .Where(x => !x.Is().BasicLand)
        .Select(x => x.Name.Replace(":", "").ToLowerInvariant())
        .Where(x => !existingImages.Contains(x))
        .ToArray();
      
      var indexPath = Path.Combine(sourceFolder, "index.json");

      var index = (File.Exists(indexPath)
        ? LoadIndex(indexPath)
        : BuildIndex(sourceFolder, indexPath));                 

      try
      {
        foreach (var name in missingImages)
        {
          if (index.TryGetValue(name, out var entry))
          {
            Console.WriteLine($"Processing {name}");

            ExtractFileFromZip(entry, outputFolderExtract);

            var infile = Path.Combine(outputFolderExtract, entry.Path);
            var outFile = Path.Combine(outputFolder, $"{entry.CardName}.jpg");

            if (OldStyle.Contains(entry.Edition))
            {
              PrepareImageOldFormat(infile, outFile);
            }
            else
            {
              PrepareImageNewFormat(infile, outFile);
            }
          }
          else
          {
            Console.Error.WriteLine($"{name} not found");
          }
        }
      }
      catch (Win32Exception ex)
      {
        Console.Error.WriteLine("ImageMagick not found. Please install ImageMagick " +
          "from https://imagemagick.org/script/download.php and add it to PATH.");
        
        return false;
      }
            
      return true;
    }

    private static Dictionary<string, IndexEntry> LoadIndex(string indexPath)
    {      
      Console.WriteLine("Loading index of available images...");
      
      return JsonConvert
        .DeserializeObject<List<IndexEntry>>(File.ReadAllText(indexPath))
        .GroupBy(x => x.CardName)
        .ToDictionary(x => x.Key, x => x.First());
    }

    private Dictionary<string, IndexEntry> BuildIndex(string sourceFolder, string indexPath)
    {
      var sourceFiles = Directory.EnumerateFiles(
              sourceFolder,
              "*",
              SearchOption.AllDirectories);

      var entries = new List<IndexEntry>();

      Console.WriteLine("Building index of available images...");

      foreach (var file in sourceFiles)
      {
        var fileInfo = new FileInfo(file);
        if (fileInfo.Extension == ".zip")
        {
          using (var zip = ZipFile.Read(file))
          {
            foreach (var zipEntry in zip.Entries)
            {
              if (zipEntry.IsDirectory)
              {
                continue;
              }

              var match = CardNameRegex.Match(zipEntry.FileName);

              if (match.Success)
              {
                var dbEntry = new IndexEntry
                {
                  CardName = match.Groups[2].Value.ToLowerInvariant(),
                  Edition = match.Groups[1].Value.ToLowerInvariant(),
                  Path = zipEntry.FileName,
                  PathZip = fileInfo.FullName
                };

                entries.Add(dbEntry);
              }
            }
          }
        }
      }            

      Console.WriteLine("Saving index for future use...");
      File.WriteAllText(
        indexPath,
        JsonConvert.SerializeObject(entries, Formatting.Indented));

      return entries
        .GroupBy(x => x.CardName)
        .ToDictionary(x => x.Key, x => x.First());      
    }

    private static void ExtractFileFromZip(IndexEntry entry, string outputFolder)
    {
      using (var file = ZipFile.Read(entry.PathZip))
      {
        var zipEntry = file[entry.Path];
        zipEntry.Extract(outputFolder, ExtractExistingFileAction.OverwriteSilently);
      }
    }

    private static void PrepareImageNewFormat(string inFile, string outFile)
    {
      var process = new Process();
      process.StartInfo.FileName = "magick";
      process.StartInfo.Arguments = $"\"{inFile}\"[745x1040] -crop 570x453+89+124 -resize 410x326 \"{outFile}\"";
      process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      
      process.Start();
      process.WaitForExit();
    }

    private static void PrepareImageOldFormat(string inFile, string outFile)
    {
      var process = new Process();
      process.StartInfo.FileName = "magick";
      process.StartInfo.Arguments = $"\"{inFile}\"[745x1040] -crop 564x448+89+112 -resize 410x326 \"{outFile}\"";
      process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

      process.Start();
      process.WaitForExit();
    }


    public override void Usage()
    {
      Console.WriteLine("usage: ugrove image s=<source_folder> o=<output_folder>\n\n" +
        "Searches source folder for missing card images, crops them and writes them to output folder.");
    }
  }
}