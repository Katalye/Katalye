#!/usr/bin/env powershell

param (
    [Parameter(Mandatory = $true)]
    [string] $Version,
    [switch] $TagLatest,
    [switch] $Push,
    [string] $DockerUsername,
    [string] $DockerPassword
)

$tag = $Version
$image = "katalye/katalye-api"

Write-Host "Building ${image}:$tag..."
docker build -t ${image}:$tag .

if ($Push)
{
    Write-Host "Logging into user $DockerUsername."
    docker login -u=""$DockerUsername"" -p=""$DockerPassword""

    Write-Host "Pushing ${image}:$tag..."
    docker push ${image}:$tag

    if ($TagLatest )
    {
        Write-Host "Tagging latest and pushing..."
        docker tag ${image}:$tag ${image}:latest
        docker push ${image}:latest
    }
}
