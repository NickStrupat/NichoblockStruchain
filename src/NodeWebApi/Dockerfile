from microsoft/dotnet:2.2-sdk-alpine as build
workdir /app
copy . ./
run dotnet publish ./ -c Release -o out -f netcoreapp2.2

from microsoft/dotnet:2.2-aspnetcore-runtime-alpine as runtime
workdir /app
copy --from=build /app/out .

entrypoint ["dotnet", "NodeWebApi.dll"]