param($SolutionDir, $Platform, $Configuration)
$DebugPreference = "Continue"

function Convert-WithXslt($xmlFilePath, $xsltFilePath, $outputFilePath)
{
    $xsltFilePath = resolve-path $xsltFilePath
    $xmlFilePath = resolve-path $xmlFilePath
    $outputFilePath = resolve-path $outputFilePath

	Write-Debug "[$xsltFilePath] [$xmlFilePath] [$outputFilePath]"
	
    $xslt = new-object system.xml.xsl.xslcompiledtransform
    $xslt.load( $xsltFilePath )
    $xslt.Transform( $xmlFilePath, $outputFilePath )
}

if(-not $SolutionDir)
{
	$SolutionDir = ".";
}

$OutputDir = $SolutionDir + "\OutlookKolab\bin"

"." > $OutputDir\gendarme.txt
gendarme.exe --xml $OutputDir\gendarme.xml --severity high+ --ignore $SolutionDir\gendarmeignore.txt $OutputDir\$Platform\$Configuration\OutlookKolab.dll
convert-withxslt $OutputDir\gendarme.xml $SolutionDir\gendarme.xslt $OutputDir\gendarme.txt
(get-content $OutputDir\gendarme.txt) -replace '\(\D?(\d+)\)', ' ($1,1)' | set-content $OutputDir\gendarme.txt
exit 0
