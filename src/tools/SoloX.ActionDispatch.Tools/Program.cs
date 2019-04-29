// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.State.Generator;

namespace SoloX.ActionDispatch.Tools
{
    /// <summary>
    /// Program entry point class.
    /// </summary>
    public class Program
    {
        private readonly ILogger<Program> logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that contains all arguments.</param>
        public Program(IConfiguration configuration)
        {
            this.configuration = configuration;

            IServiceCollection sc = new ServiceCollection();

            sc.AddLogging(b => b.AddConsole());
            sc.AddStateGenerator();

            this.Service = sc.BuildServiceProvider();

            this.logger = this.Service.GetService<ILogger<Program>>();
        }

        private ServiceProvider Service { get; }

        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args">Tools arguments.</param>
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            new Program(config).Run();

            Console.ReadKey();
        }

        /// <summary>
        /// Run the tools command.
        /// </summary>
        public void Run()
        {
            var projectFile = this.configuration.GetValue<string>("project");

            var projectNameSpace = "SoloX.ActionDispatch.State.Sample";

            var generator = this.Service.GetService<IStateGenerator>();
            generator.Generate(projectFile, projectNameSpace);
        }
    }
}
