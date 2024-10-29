dotnet pack -c Debug -o package
dotnet tool install Db4Wd.Postgres -g --add-source ./package