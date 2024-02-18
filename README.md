# Aho-Corasick algorithm

<p align="center">
<img src="https://upload.wikimedia.org/wikipedia/commons/9/90/A_diagram_of_the_Aho-Corasick_string_search_algorithm.svg" width="500"  alt="public domain tree diagram">
</p>

### Library
This is a proof of concept implementation in c# of the [Aho-Corasick algorithm](https://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_algorithm) for efficient O(n + z) pattern matching in text processing. 



### Test
The test example usage demonstrates a text replacement process of (12K x 12K) potential replacements operations. 
It utilizes two key reference files (each containing over 12K lines): 
- 'xREF.txt' random words for redacting and Bates replacement, the cross reference 
- 'STARGATE.dat' for processing public CIA STARGATE document metadata, format is eDiscovery standard, UTF-8

The implementation showcases efficient text processing using parallel and sequential methods, achieving significant performance in both.
Note negligible differences in replacement count repeatability using parallel due to overlapping search key substrings in the test data.

The 'ReplacementDetails' struct orchestrates the replacement operation, leveraging the Aho-Corasick algorithm for pattern matching. 

Helper methods facilitate loading of input and reference data, execution of search operations, and output generation, 
with support for parallel processing to enhance performance.

```c#
using Aho_Corasick_Helpers;
...

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
```
