cd src/Obama
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ../../dist
cd ..
cd ..
