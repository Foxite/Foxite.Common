#!/bin/sh

dotnet build -c Release
dotnet pack Foxite.Common/Foxite.Common.csproj
dotnet pack Foxite.Common.Core/Foxite.Common.Core.csproj
dotnet nuget push --skip-duplicate -s $(cat pushrepo.txt) -k $(cat pushkey.txt) $(ls -t Foxite.Common/bin/Release/*.nupkg | head -n 1)
dotnet nuget push --skip-duplicate -s $(cat pushrepo.txt) -k $(cat pushkey.txt) $(ls -t Foxite.Common.Core/bin/Release/*.nupkg | head -n 1)

