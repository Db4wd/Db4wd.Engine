dotnet tool uninstall dbforward.postgres -g || echo 'pg4: no previous installation'
dotnet pack src/providers/postgres -o .package
dotnet tool install -g --add-source .package dbforward.postgres