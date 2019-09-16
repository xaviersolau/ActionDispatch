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
            sc.AddSingleton(configuration);
            sc.AddStateGenerator();

            this.Service = sc.BuildServiceProvider();

            this.logger = this.Service.GetService<ILogger<Program>>();
        }

        private ServiceProvider Service { get; }

        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args">Tools arguments.</param>
        /// <returns>Error code.</returns>
        public static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            return new Program(config).Run();
        }

        /// <summary>
        /// Run the tools command.
        /// </summary>
        /// <returns>Error code.</returns>
        public int Run()
        {
            var projectFile = this.configuration.GetValue<string>("project");
            var inputsFile = this.configuration.GetValue<string>("inputs");
            var outputsFile = this.configuration.GetValue<string>("outputs");
            if (!bool.TryParse(this.configuration.GetValue<string>("generate"), out var generate))
            {
                generate = true;
            }

            if (generate)
            {
                this.logger.LogInformation($"Generating state class in {projectFile}");
            }

            if (string.IsNullOrEmpty(projectFile))
            {
                this.logger.LogError($"Missing project file parameter.");
                return -1;
            }

            if (!File.Exists(projectFile))
            {
                this.logger.LogError($"Could not find project file {projectFile}");
                return -1;
            }

            var generator = this.Service.GetService<IStateGenerator>();
            generator.Generate(projectFile, inputsFile, outputsFile, generate);

            if (generate)
            {
                this.logger.LogInformation($"State classes are generated.");
            }

            return 0;
        }
    }
}
