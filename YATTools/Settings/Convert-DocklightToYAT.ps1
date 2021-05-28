#===================================================================================================
# YAT - Yet Another Terminal.
# Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
# Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
# -------------------------------------------------------------------------------------------------
# $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CountLOC.ps1 $
# $Revision: 3584 $
# $Date: 2021-01-04 16:40:12 +0100 (Mo., 04 Jan 2021) $
# $Author: maettu_this $
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
Converts a Docklight settings file to .yat format. SCRIPT IS YET EXPERIMENTAL AND LIMITED!

.DESCRIPTION
This script is a response to YAT feature request #429 'docklight file type' at
https://sourceforge.net/p/y-a-terminal/feature-requests/429/.

The implementation is based on the information given by:
 > https://docklight.de/exampleFiles/projects/PingPong.ptp
 > https://docklight.de/exampleFiles/projects/ModemDiagnostics.ptp
 > https://docklight.de/exampleFiles/network/PingPong_TCP.zip
 > https://docklight.de/exampleFiles/network/PingPong_UDP_Loopback.zip
 > https://docklight.de/exampleFiles/network/PingPong_UDP_Peer.zip

The above information is not sufficient to get the complete schema of a Docklight settings file.
Thus, yet into account taken is only the following content:
 > COMMSETTINGS: #4 baud rate
 > SEND: #2 description, #3 data

For simplicity, a YAT 'Binary' terminal is used, as that allows straight-forward conversion of
Docklight's 'SEND' sections.

In the future, this script could be refined as follows:
 > If data of all 'SEND' sections ends with e.g. "0D 0A", then a YAT 'Text' terminal could be used,
   using the corresponding EOL (end of line) sequence and using text instead of binary data.
 > If there is a 'COMMCHANNELS' section, a YAT workspace file (.yaw) referring to multiple terminal
   files (.yat) could be created.
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
 > CS-Script based script
    + ease of development (Visual Studio,...)
    - additional tool/exe
    - command line parsing

.PARAMETER InputFilePathPattern
The path to Docklight file(s) to convert from. Can either refer to file(s) of the current directory
or file(s) under a relative or absolute path. The default is "*.ptp", i.e. all Docklight settings
files (.ptp) of the current directory.

.PARAMETER YATPath
The path to YAT executables. The default is "C:\Program Files\YAT".
The path is needed to retrieve the YAT terminal settings schema.

.OUTPUTS
YAT terminal file(s) (.yat) limited equivalent to the given Docklight settings file(s).

.EXAMPLE
Convert all Docklight settings files (.ptp) of all "Docklight*" named subdirectories.
.\Convert-DocklightToYAT.ps1 -InputFilePathPattern ".\Docklight*\*.ptp"
#>

[CmdletBinding()]

#---------------------------------------------------------------------------------------------------
# Input parameters
#---------------------------------------------------------------------------------------------------

param
(
	[Parameter(Mandatory=$false)]
	[Alias("Input", "InputFile", "InputFilePath", "InputFilePattern")]
	[string]
	$InputFilePathPattern = "*.ptp",

	[Parameter(Mandatory=$false)]
	[Alias("YAT")]
	[string]
	$YATPath = "C:\Program Files\YAT"
)

# Read the 'Verbose' and 'Debug' parameters into variables for easier processing:
$verbose = $null
if (!$PSBoundParameters.TryGetValue('Verbose', [ref]$verbose)) { $verbose = $false }
$debug = $null
if (!$PSBoundParameters.TryGetValue('Debug', [ref]$debug)) { $debug = $false }


#===================================================================================================
# Local function implementation
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# New-TerminalSettingsRoot
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Returns a new .NET YAT terminal settings object.

.PARAMETER YATPath
The path to the YAT assemblies to use.

.INPUTS
None.

.OUTPUTS
A flag indicating success.
The new .NET YAT terminal settings object, which is defined as [XmlRoot].
#>
function New-TerminalSettingsRoot
{
	[CmdletBinding()]

	param
	(
		[Parameter(Mandatory=$true)]
		[string]
		$YATPath
	)

	Add-Type -Path "$YATPath\MKY.dll"
	Add-Type -Path "$YATPath\YAT.Settings.Application.dll"
	Add-Type -Path "$YATPath\YAT.Settings.Model.dll"

	# Application settings are required to create a terminals settings object:
	[YAT.Settings.Application.ApplicationSettings]::Create([MKY.Settings.ApplicationSettingsFileAccess]::None)

	# Create the terminal settings object:
	$terminalSettingsRoot = New-Object -TypeName "YAT.Settings.Model.TerminalSettingsRoot"

	Write-Output $terminalSettingsRoot
}



#---------------------------------------------------------------------------------------------------
# Get-CommChannel
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Extracts 'COMMCHANNELS' from the Docklight file and configures the terminal settings accordingly.
Only the first channel is used, therefore this method is called singular.

.PARAMETER InputFileRaw
The Docklight file to convert from.

.PARAMETER TerminalSettingsRoot
The YAT terminals settings object to convert to.

.INPUTS
None.

.OUTPUTS
None.
#>
function Get-CommChannel
{
	[CmdletBinding()]

	param
	(
		[Parameter(Mandatory=$true)]
		[string]
		$InputFileRaw,

		[Parameter(Mandatory=$true)]
		[YAT.Settings.Model.TerminalSettingsRoot]
		[ref]$TerminalSettingsRoot
	)

}


#---------------------------------------------------------------------------------------------------
# Get-CommSettings
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Extracts 'COMMSETTINGS' from the Docklight file and configures the terminal settings accordingly.

.PARAMETER InputFileRaw
The Docklight file to convert from.

.PARAMETER TerminalSettingsRoot
The YAT terminals settings object to convert to.

.INPUTS
None.

.OUTPUTS
None.
#>
function Get-CommSettings
{
	[CmdletBinding()]

	param
	(
		[Parameter(Mandatory=$true)]
		[string]
		$InputFileRaw,

		[Parameter(Mandatory=$true)]
		[YAT.Settings.Model.TerminalSettingsRoot]
		[ref]$TerminalSettingsRoot
	)

	$baudRateValue = $null
	$settingsSection = $inputFileRaw | Select-String '(?sm)^COMMSETTINGS\r\n(.*?)(?:\r\n){2}' | %{ $_.Matches.Groups[1].Value }
	if ($verbose) {
		Write-Verbose "Found:"
		$settingsSection | ForEach-Object { Write-Verbose " > ""$_""" }
	}
	$settingsLines = $settingsSection -split "`n"
	if ($settingsLines.Count -ne $null) {
		if ($settingsLines.Count -ne 9) {
			Write-Warning "The COMMSETTINGS section of ""$inputFilePath"" does not consist of 9 lines!"
			Write-Verbose "Found:"
			$settingsLines | ForEach-Object { Write-Verbose " > ""$_""" }
			continue
		}

		$baudRateValue = $settingsLines[3] | Select-String '^(\d+)$' | %{ $_.Matches.Groups[1].Value }
		if ($baudRateValue -eq $null) {
			Write-Warning "Invalid baud rate value in the COMMSETTINGS section!"
			continue
		}
		else {
			Write-Verbose "The baud rate is $baudRateValue."
		}
	}
}


<#COMMSETTINGS

Mode
0

x or COMx
y or COMy to ignore

Baud as int

Parity
0 = even
1 = mark
2 = none
3 = odd
4 = space

Parity Error Char

Data
3 = 7
4 = 8

Stop
0 = 1
1 = 1.5
2 = 2

Flow Control
0 Off
1 Manual Hardware
2 Hardware
3 Software
4 RS485


COMMCHANNELS
LOCALHOST:10001
SERVER:10001
UDP:LOCALHOST:50000
UDP:LOCALHOST:5001:5002
UDP:LOCALHOST:5002:5001

SEND
2 = index 0..10
AT+FCLASS=? = description
41 54 2B 46 43 4C 41 53 53 3D 3F 0D 0A = bytes incl. EOL
0 don't care
5 don't care#>

#---------------------------------------------------------------------------------------------------
# Get-SendButtons
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Extracts 'SEND' from the Docklight file and configures the terminal settings accordingly.

.PARAMETER InputFileRaw
The Docklight file to convert from.

.PARAMETER TerminalSettingsRoot
The YAT terminals settings object to convert to.

.INPUTS
None.

.OUTPUTS
None.
#>
function Get-SendButtons
{
	[CmdletBinding()]

	param
	(
		[Parameter(Mandatory=$true)]
		[string]
		$InputFileRaw,

		[Parameter(Mandatory=$true)]
		[YAT.Settings.Model.TerminalSettingsRoot]
		[ref]$TerminalSettingsRoot
	)


	#$inputFileRaw | Select-String '(?sm)^SEND$(.*)^$' -AllMatches
	#              | Foreach {$_.Matches}
	#              | Foreach {$_.Value}
}


#---------------------------------------------------------------------------------------------------
# Save-TerminalSettingsFile
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Saves the given terminal settings object to a YAT XML settings file.

.PARAMETER OutputFilePath
The YAT file path to save to.

.PARAMETER TerminalSettingsRoot
The YAT terminals settings object to save.

.INPUTS
None.

.OUTPUTS
None.
#>
function Save-TerminalSettingsFile
{
	[CmdletBinding()]

	param
	(
		[Parameter(Mandatory=$true)]
		[string]
		$OutputFilePath,

		[Parameter(Mandatory=$true)]
		[YAT.Settings.Model.TerminalSettingsRoot]
		[ref]$TerminalSettingsRoot
	)
}



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

	$inputFileLines = Get-Content $inputFilePath # Content is returned as string[].
	if ($inputFileLines[0] -ne "VERSION") {
		Write-Warning """$inputFilePath"" does not seem to be a Docklight settings file!"
		continue
	}
	$versionValue = $inputFileLines[1] | Select-String '^(\d+)$' | %{ $_.Matches.Groups[1].Value }
	if ($versionValue -eq $null) {
		Write-Warning """$inputFilePath"" does not seem to be a Docklight settings file!"
		continue
	}
	Write-Verbose "The Docklight file version is $versionValue."
	string $inputFileRaw = Get-Content $inputFilePath -Raw # Content is returned as a single string.

	Write-Verbose "Creating YAT terminal settings object..."
	$result = New-TerminalSettingsRoot $YATPath
	$newSuccess = $result[0]
	$terminalSettingsRoot = $result[1]
	if ((-not $newSuccess) -or ($terminalSettingsRoot -eq $null)) {
		Write-Error "Failed to create a YAT terminal settings object!"
		break
	}

	Write-Verbose "Extracting Docklight settings to YAT terminal settings object..."
	Get-CommChannel( $inputFileRaw, [ref]$terminalSettingsRoot)
	Get-CommSettings($inputFileRaw, [ref]$terminalSettingsRoot) # Get serial port settings in any case.
	Get-SendButtons( $inputFileRaw, [ref]$terminalSettingsRoot)

	$outputFilePath = "$inputFilePath.yat"
	Write-Verbose "Saving ""$outputFilePath""..."
	$saveSuccess = Save-TerminalSettingsFile($outputFilePath, [ref]$terminalSettingsRoot)
	if ($saveSuccess) {
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
# $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CountLOC.ps1 $
#===================================================================================================
