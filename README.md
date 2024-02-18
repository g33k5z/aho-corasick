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
