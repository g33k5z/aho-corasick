using Aho_Corasick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace Aho_Corasick_Helpers
{
    /*
        Provides helper methods and structures for managing file content, organizing replacement details, 
        and facilitating text processing operations using the Aho-Corasick algorithm.
    */

    /// <summary>
    /// Data Struct for file content at a line level with ability to hold xREF matching entries
    /// </summary>
    /// <param name="line"></param>
    /// <param name="text"></param>
    public struct LineResult(int line, string text)
    {   public int Line { get; set; } = line;
        public readonly string ValueOriginal { get; } = text;

        /// <summary>
        /// Hold matching xREF keys and replacement values
        /// </summary>
        public Dictionary<string, string> Matches { get; set; } = [];
    };

    /// <summary>
    /// Struct for organizing the xREF operation
    /// </summary>
    /// <param name="xREF"></param>
    /// <param name="inputFile"></param>
    /// <param name="outputFile"></param>
    public struct ReplacementDetails(string xREF, string inputFile, string outputFile)
    {
        public FileInfo XREFFile { get; set; } = new(xREF);
        public FileInfo InputFile { get; set; } = new(inputFile);
        public FileInfo OutputFile { get; set; } = new(outputFile);

        public List<LineResult> FileContent { get; set; }
        public Dictionary<string, string> XREF { get; set; } = [];
        /// <summary>
        /// Aho-Corasick Trie
        /// </summary>
        public Trie Trie { get; set; } = new();

        /// <summary>
        /// Start Time for processing duration calculation
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// End Time for processing duration calculation
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// Processing time  duration 
        /// </summary>
        public readonly TimeSpan Duration { get { return End - Start; } }


        public readonly int MatchCount => FileContent.Sum(x => x.Matches.Count);

        public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
    }

    public static class Helpers
    {
        /// <summary>
        /// Compare File contents to xREF keys using Trie.Search(). 
        /// Add matching keys and their replacement text directly to the LineResults.Matches
        /// </summary>
        /// <param name="RD"></param>
        public static void PerformSearch(ref ReplacementDetails RD)
        {
            for (int i = 0; i < RD.FileContent.Count; i++)
            {
                var _matches = RD.Trie.Search(RD.FileContent[i].ValueOriginal);
                foreach (var _match in _matches)
                {
                    if (!RD.FileContent[i].Matches.ContainsKey(_match))
                        RD.FileContent[i].Matches.Add(_match, RD.XREF[_match]);
                }
            }
        }

        /// <summary>
        /// Return a string of the xREF'd line content. The original value is unchanged.
        /// </summary>
        /// <param name="LR"></param>
        /// <returns></returns>
        public static string GetLineReplacedValue(LineResult LR)
        {
            string _value = LR.ValueOriginal;

            foreach (var match in LR.Matches)
            {
                _value = _value.Replace(match.Key, match.Value);
            }
            return _value;
        }

        /// <summary>
        /// Verify files and xREF
        /// </summary>
        /// <param name="RD"></param>
        /// <exception cref="Exception"></exception>
        public static void Preflight(ReplacementDetails RD)
        {
            if (!RD.InputFile.Exists)
                throw new Exception($"Input File not found {RD.InputFile}");

             if (!RD.XREFFile.Exists)
                throw new Exception($"xREF File not found {RD.XREFFile}");

            if (RD.OutputFile.Name == "")
                throw new Exception($"Set Output File {RD.OutputFile.FullName}");
        }

        /// <summary>
        /// Performs preflight, loads xREF, building Trie and linking failures
        /// </summary>
        /// <param name="RD"></param>
        /// <param name="delimiter"></param>
        /// <param name="clearTrie">Should we build a new trie? False allows combining of multiple xREF into one search. Defaults to true.</param>
        public static void LoadXREF(ref ReplacementDetails RD, char delimiter = '\t', bool clearTrie = true)
        {
            Preflight(RD);

            if (clearTrie)
                RD.Trie = new();

            foreach (string line in File.ReadLines(RD.XREFFile.FullName))
            {
                var parts = line.Split(delimiter);

                if (!RD.XREF.ContainsKey(parts[0]))
                    RD.XREF.Add(parts[0], parts[1]);

                RD.Trie.Add(parts[0]);
            }

            RD.Trie.BuildFailure();
        }

        public static void LoadInput(ref ReplacementDetails RD)
        {
            RD.FileContent?.Clear();
            RD.FileContent = File.ReadLines(RD.InputFile.FullName, Encoding.UTF8)
                .Select(x => new LineResult(0, x))
                .ToList();
        }

        public static void WriteOutput(ReplacementDetails RD)
        {
            File.WriteAllLines(RD.OutputFile.FullName, RD.FileContent.Select(x => GetLineReplacedValue(x)).ToArray(), Encoding.UTF8);
        }

        public static void WriteOutputParallel(ReplacementDetails RD)
        {
            string[] _results = new string[(RD.FileContent.Count)];

            Parallel.For(0, RD.FileContent.Count, RD.ParallelOptions, i =>
            {
                _results[i] = GetLineReplacedValue(RD.FileContent[i]);
            });

            File.WriteAllLines(RD.OutputFile.FullName, _results, Encoding.UTF8);
        }

    }
    

}
