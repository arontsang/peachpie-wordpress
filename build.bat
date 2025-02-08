pushd app
dotnet publish -c Release -r linux-x64 --self-contained
popd