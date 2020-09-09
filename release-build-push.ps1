nuget restore src
nuget pack .\src\GMO.Vorto\ -build -Symbols -SymbolPackageFormat snupkg -Properties Configuration=Release -IncludeReferencedProjects
nuget pack .\src\GMO.Vorto.Web\ -build -Symbols -SymbolPackageFormat snupkg -Properties Configuration=Release -IncludeReferencedProjects
pause
