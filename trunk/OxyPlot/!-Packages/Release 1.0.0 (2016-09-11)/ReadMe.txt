
Background
----------
OxyPlot 1.0.0 is used as that is the last version to support .NET 4.
OxyPlot 2.0.0 is tied to .NET 4.5, issue #1452 "Again support .NET 4.0 for WinForms and WPF" is pending.


Modifications
-------------
1. Unnecessary references (PresentationCore, PresentationFramework, System.Xaml, WindowsBase) removed from:
    > \Source\OxyPlot\OxyPlot_NET40.csproj
2. x64/x86 configurations added to:
    > \Source\OxyPlot\OxyPlot_NET40.csproj                                     => required for integrating projects into YAT
    > \Source\OxyPlot.WindowsForms\OxyPlot.WindowsForms_NET40.csproj           => required for integrating projects into YAT
    > \Source\Examples\ExampleLibrary\ExampleLibrary_NET40.csproj              => for verification purposes only
    > \Source\Examples\WindowsForms\ExampleBrowser\ExampleBrowser_NET40.csproj => for verification purposes only
    > \Source\Examples\WindowsForms\WindowsFormsDemo\WindowsFormsDemo.csproj   => for verification purposes only
    > \Source\OxyPlot.WindowsForms.sln                                        => for verification purposes only
3. Minor project/solution fixes.


Integration
-----------
YAT directly uses the projects (doesn't use the released DLLs) for...
...not having the undesired references mentioned above.
...explicitly using x64/x86 builds.

Disadvantage:
 > OxyPlot version has to be incremented for each YAT release (same as for ALAZ,...).


----------------
2020-01-14 / MKY
