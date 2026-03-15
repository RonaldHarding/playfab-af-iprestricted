<#
.SYNOPSIS
    Deploys the FirewallFunctionApp binaries to Azure.

.PARAMETER FunctionAppName
    The name of the Azure Function App to deploy to.

.PARAMETER ResourceGroup
    The resource group containing the Function App.

.EXAMPLE
    .\deploy.ps1 -FunctionAppName "myapp-func" -ResourceGroup "myapp-rg"
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$FunctionAppName,

    [Parameter(Mandatory = $true)]
    [string]$ResourceGroup
)

$ErrorActionPreference = "Stop"

$publishDir = "./publish"

Write-Host "Building and publishing the function app..."
dotnet publish -c Release -o $publishDir

Write-Host "Creating deployment zip..."
$zipPath = "./deploy.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath }
Compress-Archive -Path "$publishDir/*" -DestinationPath $zipPath

Write-Host "Deploying to Azure Function App '$FunctionAppName'..."
az functionapp deployment source config-zip `
    --resource-group $ResourceGroup `
    --name $FunctionAppName `
    --src $zipPath

Write-Host "Cleaning up..."
Remove-Item $zipPath
Remove-Item $publishDir -Recurse -Force

Write-Host "Deployment complete!" -ForegroundColor Green
