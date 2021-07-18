#===================================================================================================
# YAT - Yet Another Terminal.
# Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
# Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
# -------------------------------------------------------------------------------------------------
# $URL$
# $Revision$
# $Date$
# $Author$
# -------------------------------------------------------------------------------------------------
# See release notes for product version details.
# See SVN change log for file revision details.
# Author(s): Matthias Klaey
# -------------------------------------------------------------------------------------------------
# Copyright © 2003-2021 Matthias Kläy.
# All rights reserved.
# -------------------------------------------------------------------------------------------------
# This source code is licensed under the GNU LGPL.
# See http://www.gnu.org/licenses/lgpl.html for license details.
#===================================================================================================


#===================================================================================================
# Global interface declaration
#===================================================================================================

<#
.SYNOPSIS
Converts a Docklight settings file in best-effort manner to .yat format.

.DESCRIPTION
This script is a response to YAT feature request #429 'docklight file type' at
https://sourceforge.net/p/y-a-terminal/feature-requests/429/.

The implementation is based on the information given by (version 7):
 > https://docklight.de/exampleFiles/projects/PingPong.ptp
 > https://docklight.de/exampleFiles/projects/ModemDiagnostics.ptp
 > https://docklight.de/exampleFiles/network/PingPong_TCP.zip
 > https://docklight.de/exampleFiles/network/PingPong_UDP_Loopback.zip
 > https://docklight.de/exampleFiles/network/PingPong_UDP_Peer.zip
As well default settings provided by a Docklight user (version 8).

The above information is not sufficient to get the complete schema of a Docklight settings file.
Thus, yet into account taken is only the following content:
 > COMMSETTINGS (for serial COM ports) or COMMCHANNELS (for TCP/IP or UDP/IP sockets)
 > SEND (entries #1 index, #2 description and #3 data)

For simplicity, a YAT 'Binary' terminal is used, as that allows straight-forward conversion of
Docklight's 'SEND' sections.

In the future, this script could be refined as follows:
 > If data of all 'SEND' sections ends with e.g. "0D 0A", then a YAT 'Text' terminal could be used,
   using the corresponding EOL (end of line) sequence and using text instead of binary data.
 > Optionally, the 'SEND' sections could be converted to a YAT command page(s) file (.yacp/.yacps).

Alternative approaches considered but discarded:
 > Integration into YAT
    + ease of development (Visual Studio,...)
    + out-of-the-box
    - in the interest of only a few
    - would require better coverage
 > Separate mini-app rather than a script
    + ease of development (Visual Studio,...)
    - command line parsing
    - additional efforts for releasing/distribution
 > Fully CS-Script based script
    + ease of development (Visual Studio,...)
    - additional tool/exe
    - command line parsing

Once YAT Scripting becomes available, this script is intended to be migrated there. To already
prepare for this step, the script now is implemeneted in a hybrid manner:
 > Partly PowerShell and partly CS-Script based
    + command line parsing
    + ease of development (Visual Studio,...)
    - additional tool/exe

.PARAMETER YATPath
The path to YAT executables. The default is "C:\Program Files\YAT".
The path is needed to retrieve the YAT terminal settings schema.

.PARAMETER InputFilePathPattern
The path to Docklight file(s) to convert from. Can either refer to file(s) of the current directory
or file(s) under a relative or absolute path. The default is "*.ptp", i.e. all Docklight settings
files (.ptp) of the current directory.

.PARAMETER OutputPath
An optional output path for converting the YAT terminal file(s) (.yat) to. If not given, the files
will be output to the directory of the respective Docklight file.

.OUTPUTS
YAT terminal file(s) (.yat) limited equivalent to the given Docklight settings file(s).

.EXAMPLE
Convert all Docklight settings files (.ptp) of all "Docklight*" named subdirectories.
.\Convert-DocklightToYAT.ps1 -InputFilePathPattern ".\Docklight*\*.ptp"

Write all converted files to the given output directory.
.\Convert-DocklightToYAT.ps1 -InputFilePathPattern ".\Docklight*\*.ptp" -OutputPath ".\Out"
#>

[CmdletBinding()]

#---------------------------------------------------------------------------------------------------
# Input parameters
#---------------------------------------------------------------------------------------------------

param
(
	[Parameter(Mandatory=$false)]
	[Alias("YAT")]
	[string]
	$YATPath = "C:\Program Files\YAT",

	[Parameter(Mandatory=$false)]
	[Alias("Input", "InputFile", "InputFilePath", "InputFilePattern")]
	[string]
	$InputFilePathPattern = "*.ptp",

	[Parameter(Mandatory=$false)]
	[Alias("Output", "OutputPath")]
	[string]
	$OutputPath = $null
)

# Read the 'Verbose' and 'Debug' parameters into variables for easier processing:
$verbose = $null
if (!$PSBoundParameters.TryGetValue('Verbose', [ref]$verbose)) { $verbose = $false }
$debug = $null
if (!$PSBoundParameters.TryGetValue('Debug', [ref]$debug)) { $debug = $false }


#===================================================================================================
# Main
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# Preparation
#---------------------------------------------------------------------------------------------------

# Retrieve name and root:
$MY_NAME = $MyInvocation.MyCommand.Name
$MY_PATH = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path)

# Check the availability of the YAT executables:
if (Get-Command "$YATPath\YAT.exe") {
	Write-Verbose "YAT is available at ""$YATPath""."
}
else {
	throw "YAT is not available at ""$YATPath""!" # Output error and exit.
}

# Prepare the result:
$fileSuccessCounter = 0


#---------------------------------------------------------------------------------------------------
# Processing
#---------------------------------------------------------------------------------------------------

# Get files:
$inputFilePaths = Get-Item $InputFilePathPattern # Items are returned as string[].
if ($verbose) {
	Write-Verbose "Found:"
	$inputFilePaths | ForEach-Object { Write-Verbose " > ""$_""" }
}

# Loop over files:
foreach ($inputFilePath in $inputFilePaths) {

	Write-Verbose "Processing ""$inputFilePath""..."

	# .\cscs.exe -dir:"C:\Program Files\YAT" .\Convert-DocklightToYAT.cs ".\SomeFile.ptp" ".\SomeFile.yat"
	$cscsCmd = ".\cscs.exe"
	if ($OutputPath -eq $null) {
		$cscsArgs = -dir:$YATPath .\Convert-DocklightToYAT.cs $inputFilePath
	}
	else {
		$outputFileName = (Get-Item $inputFilePath).Basename + ".yat"
		$outputFilePath = JoinPath $OutputPath -ChildPath $outputFileName
		$cscsArgs = -dir:$YATPath .\Convert-DocklightToYAT.cs $inputFilePath $outputFilePath

		Write-Verbose "...to""$outputFilePath""..."
	}

	$cscsResult = & $cscsCmd --% $cscsArgs
	if ($cscsResult -eq 0) {
		$fileSuccessCounter++
	}
}


#---------------------------------------------------------------------------------------------------
# Finishing
#---------------------------------------------------------------------------------------------------

if ($fileSuccessCounter -eq 0) {
	Write-Warning "No files converted!"
}
else {
	Write-Verbose "$fileSuccessCounter files converted."
}


#---------------------------------------------------------------------------------------------------
# Return
#---------------------------------------------------------------------------------------------------

Write-Output $fileSuccessCounter


#===================================================================================================
# End of
# $URL$
#===================================================================================================
