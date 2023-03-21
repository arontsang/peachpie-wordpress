using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PeachPied.Demo;

if (Assembly.GetEntryAssembly() is { Location: var location })
{
    Directory.SetCurrentDirectory(Path.GetDirectoryName(location)!);
}


var host = WebHost.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .Build();

host.Run();