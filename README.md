![GitHub Workflow Status](https://img.shields.io/github/workflow/status/squideyes/trading/Deploy%20to%20NuGet?label=build)
![NuGet Version](https://img.shields.io/nuget/v/SquidEyes.Trading)
![Downloads](https://img.shields.io/nuget/dt/squideyes.trading)
![License](https://img.shields.io/github/license/squideyes/Trading)

**SquidEyes.Trading** is an idiosyncratic collection of high-performance C#/.NET 6.0 trading primatives with a set of full-coverage unit-tests.  

The tick-data management classes are particularly fine, both in terms of speed and storage footprint.  To take a concrete example, a typical "DateTime,Bid,Ask" .CSV file can be compressed to 5% of the uncompressed size using our heretofore-proprietary TickSet format (.STS).  That's a three-fold improvement over .ZIP, plus the compression and decompression speeds are notably faster. 

Be forewarned, though, that literally no effort has been made to make the various classes and methods suitable for a general audience.  
#

Contributions are always welcome (see [CONTRIBUTING.md](https://github.com/squideyes/Trading/blob/master/CONTRIBUTING.md) for details)

**Super-Duper Extra-Important Caveat**:  THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.

More to the point, your use of this code may (literally!) lead to your losing thousands of dollars and more.  Be careful, and remember: Caveat Emptor!!



