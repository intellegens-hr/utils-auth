cd ..

get-childitem -include *.nupkg -recurse | remove-item

dotnet clean
dotnet build -c Release
dotnet pack -c Release

copy .\UtilsAuth.Core\bin\Release\*.nupkg .
copy .\UtilsAuth.DbContext.Abstractions\bin\Release\*.nupkg .
copy .\UtilsAuth.DbContext\bin\Release\*.nupkg .
copy .\UtilsAuth.Services\bin\Release\*.nupkg .
copy .\UtilsAuth.Core.Api.Models\bin\Release\*.nupkg .
copy .\UtilsAuth.Core.Api.Controllers\bin\Release\*.nupkg .
copy .\UtilsAuth.Extensions\bin\Release\*.nupkg .

dotnet nuget push "*.nupkg" --api-key $env:IntellegensNugetApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate

del *.nupkg