using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeachPied.Demo;
using PeachPied.Demo.Plugins;
using PeachPied.WordPress.AspNetCore;

if (Assembly.GetEntryAssembly() is { Location: var location })
{
    Directory.SetCurrentDirectory(Path.GetDirectoryName(location)!);
}

//
var host = WebHost.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .UseUrls("http://*:5004/")
    .Build();

host.Run();