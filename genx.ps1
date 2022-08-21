# Example code generation.
#
# The examples below are just that, examples to demonstrate some possibilities with the Xslt generator.
#
# NOTE: You must run the deploy.ps1 script first to setup the Output directory and the genx executable.
#       These examples use the pre-generated Northwind database metadata Xml file.
#

$genxExe = "Output\Bin\genx.exe" 
$organisationName = "Microsoft"
$applicationName = "Northwind"
$serviceName = "Orders"
$metadataFile = "templates\northwind_metadata.xml"

# Sample T-SQL Audit log table.
# & $genxExe --verbose generate `
#    --metadata $metadataFile `
#    --xslt templates\sql_audit_table.xslt `
#    --outputsuffix History --outputextension .sql --outputdir Output\$ApplicationName.Sql.Audit  

# CQRS examples.

# Commands.
& $genxExe --verbose generate `
    --metadata $metadataFile `
    --xslt templates\CQRS.Create.Command.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName --xsltparam ServiceName:$serviceName `
    --outputprefix Create --outputsuffix Command --outputextension .cs --outputdir Output\$ApplicationName.$ServiceName.Core.Command  

& $genxExe --verbose generate `
    --metadata $metadataFile `
    --xslt templates\CQRS.Create.Command.Validator.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName --xsltparam ServiceName:$serviceName `
    --outputprefix Create --outputsuffix CommandValidator --outputextension .cs --outputdir Output\$ApplicationName.$ServiceName.Core.Command  

& $genxExe --verbose generate `
    --metadata $metadataFile `
    --xslt templates\CQRS.Update.Command.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName --xsltparam ServiceName:$serviceName `
    --outputprefix Update --outputsuffix Command --outputextension .cs --outputdir Output\$ApplicationName.$ServiceName.Core.Command  

& $genxExe --verbose generate `
    --metadata $metadataFile `
    --xslt templates\CQRS.Update.Command.Validator.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName --xsltparam ServiceName:$serviceName `
    --outputprefix Update --outputsuffix CommandValidator --outputextension .cs --outputdir Output\$ApplicationName.$ServiceName.Core.Command  

& $genxExe --verbose generate `
    --metadata $metadataFile `
    --xslt templates\CQRS.Dto.Validator.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName --xsltparam ServiceName:$serviceName `
    --outputsuffix DtoValidator --outputextension .cs --outputdir Output\$ApplicationName.$ServiceName.Core.Command  

# Generate example IRepository interfaces.
#& $genxExe --verbose generate `
#    --metadata $metadataFile `
#    --xslt templates\repository_interface.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName `
#    --outputprefix I  --outputsuffix Repository --outputextension .cs --outputdir Output\$ApplicationName.Core.Repository  

# Generate example Repository implementation. 
#& $genxExe --verbose generate `
#    --metadata $metadataFile `
#    --xslt templates\repository.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName `
#    --outputsuffix Repository --outputextension .cs --outputdir Output\$ApplicationName.Infrastructure.EntityFramework.Repository  

# Generate Entities.
# & $genxExe --verbose generate `
#    --metadata $metadataFile `
#    --xslt templates\entity.xslt --xsltparam OrganisationName:$organisationName --xsltparam ApplicationName:$applicationName `
#    --outputextension .cs --outputdir Output\$ApplicationName.Core.Entities
  
