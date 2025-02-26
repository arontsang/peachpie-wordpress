﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PeachPied.Demo;

public class Startup
{
	private IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddMvc();
		services.AddResponseCompression();
		services.AddWordPress(option => {});
	}

	public void Configure(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseWordPress();

		app.UseDefaultFiles();
	}
}