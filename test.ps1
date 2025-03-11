# Copyright 2025, Roger Brown
# Licensed under the MIT License.

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

function ConvertTo-Hex
{
	[CmdletBinding()]
	Param([Parameter(ValueFromPipeline)][byte[]]$InputObject)

	Begin
	{
		$OutString = ''
	}

	Process
	{
		foreach ($b in $InputObject)
		{
			$OutString += "{0:X2}" -f $b
		}
	}

	End
	{
		$OutString
	}
}

$Reader = Open-SmartCardReader

try
{
	$ReaderList = Get-SmartCardReader -Reader $Reader
	$Result = @()

	foreach ($Name in $ReaderList) 
	{ 
		$AnswerToReset = $null
		$DataFile = $null
		$StatusWord = $null
		$SelectedAID = $null
	
		try
		{
			$AnswerToReset = Connect-SmartCardReader -Reader $Reader -Name $Name | ConvertTo-Hex

			try
			{
				foreach ($AID in
					([byte[]](50,80,65,89,46,83,89,83,46,68,68,70,48,49)),
					([byte[]](49,80,65,89,46,83,89,83,46,68,68,70,48,49)),
					([byte[]](160,0,0,1,103,69,83,73,71,78)),
					([byte[]](210,118,0,0,133,1,0)),
					([byte[]](49,84,73,67,46,73,67,65)),
					([byte[]](160,0,0,0,3,0,0,0)),
					([byte[]](160,0,0,1,81,0,0))
				)
				{
					$Command = [byte[]](0,164,4,0,$AID.Length) + $AID

					$Response = Invoke-SmartCardReader -Reader $Reader -Command $Command

					if (($Response.Length -eq 2) -and ($Response[0] -eq 0x67) -and ($Response[1] -eq 0))
					{
						$Command = [byte[]](0,164,4,0,$AID.Length) + $AID + ([byte[]](,0))

						$Response = Invoke-SmartCardReader -Reader $Reader -Command $Command
					}

					$StatusWord = ConvertTo-Hex $Response[($Response.length-2)..$Response.length]

					if ($Response.Length -gt 2)
					{
						$DataFile = ConvertTo-Hex $Response[0..($Response.length-3)]

						$SelectedAID = ConvertTo-Hex $AID

						break
					}
					else
					{
						if ( '9000' -eq $StatusWord )
						{
							$SelectedAID = ConvertTo-Hex $AID

							break
						}
					}
				}
			}
			finally
			{
				Disconnect-SmartCardReader -Reader $Reader
			}
		}
		catch
		{
		}

		$Result += [PSCustomObject]@{
			NAME = $Name
			ATR = $AnswerToReset
			AID = $SelectedAID
			ADF = $DataFile
			SW = $StatusWord
		}
	}
}
finally
{
	Close-SmartCardReader -Reader $Reader
}

$Result | Format-Table
