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
# Copyright © 2003-2019 Matthias Kläy.
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
This is the template for PowerShell scripts. Add a brief description of the module here. It shall
describe the functionality of the module to the user.

.DESCRIPTION
Also add a more detailed description of the module. The description may also contain implementation
specific details for future enhancements to this module.

In case of this template, the module contains the script main function and some local functions:
 > Provide-SomeLocalFunctionWithFullSyntax
 > Provide-SomeLocalFunctionWithSimplifiedSyntax
 > Provide-SomeLocalFunctionWithHintsAndPitfalls
The examples show how to declare, implement and use functions in PowerShell.

.PARAMETER SomeMandatoryParam
This is a mandatory value parameter. The user must provide a string value to it.
For string parameters, consider to allow wildcards or regex for more flexible usage.

.PARAMETER SomeOptionalParam
This is an optional value parameter. The user may provide a string value to it.
For string parameters, consider to allow wildcards or regex for more flexible usage.

.PARAMETER SomeNumericParam
This is a numeric value parameter. The user may provide a numeric value to it.

.PARAMETER SomeArrayParam
This is an array of value parameters. The user may provide one or multiple string values to it.

.PARAMETER SomeSwitchParam
This is a switch parameter. The user may enable it.

.PARAMETER LogFile
Specifies the log file for the script result. It can either be a file of the current directory,
or a relative or absolute file path. This parameter is optional. If no file path is given, no log
file will be created.

.OUTPUTS
Returns something that is useful for further processeing. For example, values above 0 indicate the
total number of erros and warnings; 0 indicates success.

.EXAMPLE
Explain what this example does.
.\Template.ps1 -SomeMandatoryParam "Some\AbsoluteOrRelative\Path"

.EXAMPLE
Explain what this example does.
.\Template.ps1 -SomeMandatoryParam "Some\AbsoluteOrRelative\Path" -SomeOptionalParam "*.txt"

.EXAMPLE
Explain what this example does.
.\Template.ps1 -SomeMandatoryParam "Some\AbsoluteOrRelative\Path" -SomeArrayParam ("ABC.txt", "DEF.txt")

.EXAMPLE
Explain what this example does.
.\Template.ps1 -SomeMandatoryParam "Some\AbsoluteOrRelative\Path" -SomeSwitchParam
#>

[CmdletBinding()]

#---------------------------------------------------------------------------------------------------
# Input parameters
#---------------------------------------------------------------------------------------------------

param
(
	[Parameter(Mandatory=$true)]
	[Alias("Mandatory")]
	[String]
	$SomeMandatoryParam,

	[Parameter(Mandatory=$false)]
	[Alias("Optional")]
	[String]
	$SomeOptionalParam = "*.*",

	[Parameter(Mandatory=$false)]
	[Alias("Numeric", "Percent")] # Use alias where appropriate.
	[Int]
	[ValidateRange(0, 100)] # Either validate a parameter here, or at the beginning of script main.
	$SomeNumericParam = 100,

	[Parameter(Mandatory=$false)]
	[Alias("Array")]
	[String[]]
	$SomeArrayParam,

	[Parameter()] # Switches are never mandatory.
	[Alias("Switch")]
	[Switch]
	$SomeSwitchParam,

	[Parameter(Mandatory=$false)]
	[Alias("Log")]
	[String]
	$LogFile
)
# If the script has no parameters, still keep "param()"!
# Otherwise, 'Verbose' and 'Debug' will not be available!

# Read the 'Verbose' and 'Debug' parameters into variables for easier processing:
$verbose = $null
if (!$PSBoundParameters.TryGetValue('Verbose', [ref]$verbose)) { $verbose = $false }
$debug = $null
if (!$PSBoundParameters.TryGetValue('Debug', [ref]$debug)) { $debug = $false }


#===================================================================================================
# Local function implementation
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# Provide-SomeLocalFunctionWithFullSyntax
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
This is a local function with full syntax. Full syntax is the recommended way to declare a function.

.DESCRIPTION
This is the detailed description.

.PARAMETER SomeMandatoryParam
This is a mandatory value parameter.

.PARAMETER SomeOptionalParam
This is an optional value parameter.

.OUTPUTS
Returns something that is useful for further processeing.

.EXAMPLE
Provide-SomeLocalFunctionWithFullSyntax -SomeMandatoryParam "Some\AbsoluteOrRelative\Path" -SomeOptionalParam "*.txt"
#>
function Provide-SomeLocalFunctionWithFullSyntax
{
	[CmdletBinding()]

	# Input parameter declaration:
	param
	(
		[Parameter(Mandatory=$true)]
		[Alias("Mandatory")]
		[String]
		$SomeMandatoryParam,

		[Parameter(Mandatory=$false)]
		[Alias("Optional")]
		[String]
		$SomeOptionalParam = "*.*"
	)
	# If the function has no parameters, still keep "param()"!
	# Otherwise, 'Verbose' and 'Debug' will not be available!

	# Info:
	Write-Verbose "This is a local function with full syntax."

	# Do whatever needs to be done:
	$folders = $SomeMandatoryParam -split "\\" | Where-Object { $_ -like "*Or*" }
	if ($verbose) {
		Write-Verbose "Folders are:"
		$folders | ForEach-Object { Write-Verbose " > ""$_""" }
	}

	# Return the result:
	Write-Output $folders
}


#---------------------------------------------------------------------------------------------------
# Provide-SomeLocalFunctionWithSimplifiedSyntax
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
This is a local function with simplified syntax. Simplified syntax may be used for small functions.

.PARAMETER SomeMandatoryParam
This is a mandatory value parameter.

.OUTPUTS
Returns something that is useful for further processeing.
#>
function Provide-SomeLocalFunctionWithSimplifiedSyntax([String]$SomeMandatoryParam)
{
	# Do whatever needs to be done:
	$folders = $SomeMandatoryParam -split "\\" | Where-Object { $_ -like "*Or*" }

	# Return the result:
	Write-Output $folders
}


#---------------------------------------------------------------------------------------------------
# Provide-SomeLocalFunctionWithHintsAndPitfalls
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
This is a local function that provides useful hints and common pitfalls.
Additional hints and pitfalls shall be added to add to these best practises.

.OUTPUTS
Returns something that is useful for further processeing.
#>
function Provide-SomeLocalFunctionWithHintsAndPitfalls
{
	# - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	# Dealing with .NET array lists
	# - and - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	# Handling non-obvious extra output
	# - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

	# Prepare an array to collect something:
	$list = New-Object System.Collections.ArrayList

	# Add something to the array:    !!! Attention !!!
	$list.Add("SomethingA") | Out-Null # Add() outputs the index where the element was added!
	$list.Add("SomethingB") | Out-Null # That output will interfere with Write-Output below!
	                                   # Append "| Out-Null" to consume the extra output!

	# Convert all options from the .NET array list to a PowerShell array:
	$array = @($list)

	# Return the result:
	Write-Output $array
}


#===================================================================================================
# Main
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# Info
#---------------------------------------------------------------------------------------------------

if ($verbose) {
	$message = "This is the template for PowerShell scripts"
	if ($SomeOptionalParam -ne "*.*") {
		$message += " processing filter ""$SomeOptionalParam"""
	}
	$message = " for ""$SomeMandatoryParam"""
	Write-Verbose $message
}


#---------------------------------------------------------------------------------------------------
# Preparation
#---------------------------------------------------------------------------------------------------

# Retrieve name and root:
$MY_NAME = $MyInvocation.MyCommand.Name
$MY_PATH = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path)

# Save the initial location:
$START_DIRECTORY = Get-Location # 'Get-Location' is the PowerShell equivalent to 'pwd'.
Write-Verbose "Script has been started in..."
Write-Verbose "...""$START_DIRECTORY"""

# Validate log file first, i.e. before writing anything to the log file:
if (($LogFile) -and -not (Test-Path -Path $LogFile -IsValid)) {
	throw "Log file ""$LogFile"" is invalid!" # Output error and exit.
}

# Either validate parameters at global declaration using one of the [Validate] attributes, or here:
if (($SomeMandatoryParam -like "*\*") -or ($SomeMandatoryParam -match "(\\\w+)+")) {
	Write-Verbose "$SomeMandatoryParam is valid"
}
else {
	$message = "$SomeMandatoryParam is invalid!"
	$message >> $LogFile
	throw $message # Output error and exit.
}


#---------------------------------------------------------------------------------------------------
# Processing
#---------------------------------------------------------------------------------------------------

# Write output as required:
Write-Host    "This message shall be output to the console in any case, indicating what's happening"
Write-Verbose "This message shall only be output in case of -Verbose operation"
Write-Debug   "This message shall only be output in case of -Debug operation"
Write-Debug   "Furthermore, user input may be required to continue or abort the script"
Write-Warning "Output a warning to indicate something went wrong, but continue the script"
Write-Error   "Output an error to indicate something went wrong, but continue the script"
# throw       "Output an error to indicate something went really wrong, and exit the script"

# Do whatever needs to be done:
Provide-SomeLocalFunctionWithFullSyntax -SomeMandatoryParam "Some\AbsoluteOrRelative\Path"
Provide-SomeLocalFunctionWithSimplifiedSyntax -SomeMandatoryParam "Some\AbsoluteOrRelative\Path"
Provide-SomeLocalFunctionWithHintsAndPitfalls

# Make sure to use "" around all paths to be prepared for paths with spaces.

# Evaluate the result:
$result = 0


#---------------------------------------------------------------------------------------------------
# Finishing
#---------------------------------------------------------------------------------------------------

# Restore the initial location:
Set-Location $START_DIRECTORY # 'Set-Location' is the PowerShell equivalent to 'cd'.
Write-Verbose "Current directory restored to..."
Write-Verbose "...""$START_DIRECTORY"""


#---------------------------------------------------------------------------------------------------
# Return
#---------------------------------------------------------------------------------------------------

Write-Output $result
# Ideally, a single value is returned as the output of a script or function. If multiple values are
# required, use the most appropriate of the following options:
#  > Return an array of values, e.g. created using the @() operator.
#  > Return a hash table, created by "New-Object -TypeName PSObject -Property $propertySource".
#  > Return a C# object, created by "Add-Type -TypeDefinition $classSource -Language CSharpVersion3
#                               and "New-Object -TypeName MyClass -Property $propertySource".
# Do not use [ref] parameters whenever possible.


#===================================================================================================
# End of
# $URL$
#===================================================================================================
