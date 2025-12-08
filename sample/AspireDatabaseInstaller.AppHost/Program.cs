using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("TestDb")
    .WithImageTag("2025-latest")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var installer = builder.AddProject<Projects.InstallationSampleConsoleApp>("dbinstaller")
    .WithReference(sqlServer)
    .WaitFor(sqlServer)
    .WithEnvironment("Command", "Install")
    .WithEnvironment("DatabaseName", "SampleDb")
    .WithEnvironment("Schema", "dbo")
    .WithEnvironment("ConnectionStringName", "TestDb");

builder.Build().Run();
