using Aho_Corasick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aho_Corasick_Helpers;

/*
    This is a proof of concept implementation of the Aho-Corasick algorithm for efficient O(n + z) pattern matching in text processing. 

    The test example usage demonstrates a text replacement process of (12K x 12K) potential replacements operations. 
    It utilizes two key reference files (each containing over 12K lines): 
        'xREF.txt' random words for redacting and Bates replacement
        'STARGATE.dat' for processing public CIA documents metadata, format is eDiscovery standard, UTF-8

    The implementation showcases efficient text processing using parallel and sequential methods, achieving significant performance in both.
    Note negligible differences in replacement count repeatability using parallel due to overlapping search key substrings in the test data.

    The 'ReplacementDetails' struct orchestrates the replacement operation, leveraging the Aho-Corasick algorithm for pattern matching. 

    Helper methods facilitate loading of input and reference data, execution of search operations, and output generation, 
    with support for parallel processing to enhance performance.
*/


// Defines the structure for managing replacement operations, including file paths, processing times, and match counts.
ReplacementDetails RD = new("xREF.txt", "STARGATE.dat", "STARGATE.xREF.dat") { Start = DateTime.Now };

// Operations to load input, reference data, perform the replacement search, and generate the output.
Helpers.LoadInput(ref RD);
Helpers.LoadXREF(ref RD);
Helpers.PerformSearch(ref RD);
Helpers.WriteOutputParallel(RD);

// Mark the end of processing and display the duration and match count.
RD.End = DateTime.Now;
Console.WriteLine($"Time: {RD.Duration.Seconds}secs \nReplacements: {RD.MatchCount}");



// Time: 1secs
// Replacements: 222194

