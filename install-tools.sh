dotnet pack src/postgres -c Debug -o package/postgres
dotnet tool install DbForward.Postgres --add-source package/postgres -g