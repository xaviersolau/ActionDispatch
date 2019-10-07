// ----------------------------------------------------------------------
// <copyright file="GeneratorTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.ActionDispatch.State.Generator.Impl;
using SoloX.CodeQuality.Test.Helpers;
using SoloX.GeneratorTools.Core.CSharp;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.ActionDispatch.State.Generator.ITest
{
    public class GeneratorTest
    {
        private ITestOutputHelper testOutputHelper;

        public GeneratorTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void BasicProjectGenerationTest()
        {
            var projectFile = "Resources/TestMultiProject/Lib1/Lib1.csproj";
            var expectedInputs = new string[]
            {
                "IMyChildState.cs",
            };
            var expectedOutputs = new string[]
            {
                "Impl/MyChildState.generated.cs",
                "Impl/StateFactoryProvider.generated.cs",
            };

            this.AssertGeneration(
                projectFile,
                nameof(GeneratorTest.BasicProjectGenerationTest),
                expectedInputs,
                expectedOutputs);
        }

        [Fact]
        public void ProjectWithProjectReferenceGenerationTest()
        {
            var projectFile = "Resources/TestMultiProject/Lib2/Lib2.csproj";
            var expectedInputs = new string[]
            {
                "IMyRootState.cs",
            };
            var expectedOutputs = new string[]
            {
                "Impl/MyRootState.generated.cs",
                "Impl/StateFactoryProvider.generated.cs",
            };

            this.AssertGeneration(
                projectFile,
                nameof(GeneratorTest.ProjectWithProjectReferenceGenerationTest),
                expectedInputs,
                expectedOutputs);
        }

        [Fact]
        public void ProjectWithPackageReferenceGenerationTest()
        {
            var projectLib1Path = "Resources/TestMultiProject/Lib1";
            var projectFile = "Resources/TestMultiProject/Lib3/Lib3.csproj";

            var expectedInputs = new string[]
            {
                "IMyRootState.cs",
            };
            var expectedOutputs = new string[]
            {
                "Impl/MyRootState.generated.cs",
                "Impl/StateFactoryProvider.generated.cs",
            };

            // Generate nuget from Lib1
            DotnetHelper.Build(projectLib1Path, out var stdout, out var stderr);

            // And generate Lib3
            this.AssertGeneration(
                projectFile,
                nameof(GeneratorTest.ProjectWithPackageReferenceGenerationTest),
                expectedInputs,
                expectedOutputs);
        }

        private void AssertGeneration(string projectFile, string testName, string[] expectedInputs, string[] expectedOutputs)
        {
            var inputsFile = $"{testName}.inputsFile";
            var outputsFile = $"{testName}.outputsFile";

            if (File.Exists(inputsFile))
            {
                File.Delete(inputsFile);
            }

            if (File.Exists(outputsFile))
            {
                File.Delete(outputsFile);
            }

            var sc = new ServiceCollection();
            sc.AddTestLogging(this.testOutputHelper);
            sc.AddCSharpToolsGenerator();

            using (var sp = sc.BuildServiceProvider())
            {
                var workspace = sp.GetService<ICSharpWorkspace>();

                var generator = new StateGenerator(
                    sp.GetService<ILogger<StateGenerator>>(),
                    workspace);

                generator.Generate(projectFile, inputsFile, outputsFile, true);

                Assert.True(File.Exists(inputsFile));
                Assert.True(File.Exists(outputsFile));
                var inputs = File.ReadAllLines(inputsFile);
                var outputs = File.ReadAllLines(outputsFile);
                Assert.Equal(expectedInputs.Length, inputs.Length);
                Assert.Equal(expectedOutputs.Length, outputs.Length);

                foreach (var expectedInput in expectedInputs)
                {
                    Assert.Contains(expectedInput, inputs.Select(x => x.Replace('\\', '/')));
                }

                foreach (var expectedOutput in expectedOutputs)
                {
                    Assert.Contains(expectedOutput, outputs.Select(x => x.Replace('\\', '/')));
                }
            }
        }
    }
}
