[CmdletBinding(PositionalBinding=$false)]
Param(
    [string][Alias('config')]$configuration = "Debug",
    [switch] $clean,
    [switch] $build,
    [switch] $test,
    [switch] $publish,
    [switch] $deploy
)

function Publish([string] $config) {
    dotnet publish -c $config -r win10-x64
}

if ($clean) {
    dotnet clean genx.cli.sln
}

if ($build) {
    dotnet build /p:configuration=$configuration /p:buildtests=true
}

if ($test) {
    dotnet test
}

if ($publish) {
    Publish -config $configuration
}

if ($deploy) {
    $templatesDir = "Templates"
    $outputDir = "Output"
    $outputBinDir = $outputDir + "\Bin\"
    $publishDir = "src\genx\bin\Release\netcoreapp2.1\win10-x64\publish\"

    if (!(Test-Path $templatesDir)) {
        Write-Host "ERR: $templatesDir folder does not exist."
        Exit 1
    }
    
    if (!(Test-Path $outputBinDir)) {
        Write-Host "Creating output directory $outputBinDir"
        New-Item -ItemType Directory -Force -Path $outputBinDir 
    }

    Publish -config Release    
    
    if (!(Test-Path $publishDir)) {
        Write-Host "ERR: Genx published folder does not exist."
        Exit 1
    }
    
    copy-item -Path $publishDir -Destination $outputBinDir -Force -Recurse
}


