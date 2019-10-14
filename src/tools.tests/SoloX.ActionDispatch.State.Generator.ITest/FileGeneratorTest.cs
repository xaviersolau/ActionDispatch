// ----------------------------------------------------------------------
// <copyright file="FileGeneratorTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.State.Generator.Impl;
using SoloX.CodeQuality.Test.Helpers;
using SoloX.GeneratorTools.Core.CSharp;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator.Impl;
using SoloX.GeneratorTools.Core.Test.Helpers.Snapshot;
using Xunit;
using Xunit.Abstractions;

namespace SoloX.ActionDispatch.State.Generator.ITest
{
    public class FileGeneratorTest
    {
        private ITestOutputHelper testOutputHelper;

        public FileGeneratorTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(@"Resources/State/ISimpleState.cs")]
        [InlineData(@"Resources/State/IObjectPropertyState.cs")]
        [InlineData(@"Resources/State/IObjectArrayPropertyState.cs")]
        public void GenerateStateClassTest(string stateInterfaceFile)
        {
            var snapshotName = nameof(this.GenerateStateClassTest) + Path.GetFileNameWithoutExtension(stateInterfaceFile);

            var sc = new ServiceCollection();
            sc.AddTestLogging(this.testOutputHelper);
            sc.AddCSharpToolsGenerator();

            using (var sp = sc.BuildServiceProvider())
            {
                var workspace = sp.GetService<ICSharpWorkspace>();

                workspace.RegisterAssembly(typeof(IState).Assembly.Location);
                workspace.RegisterFile(stateInterfaceFile);

                var generator = new StateGenerator(
                    sp.GetService<ILogger<StateGenerator>>(),
                    workspace);

                var inputs = new HashSet<string>();
                var locator = new RelativeLocator(string.Empty, "target.name.space");

                var snapshotGenerator = new SnapshotGenerator();

                generator.Generate(locator, snapshotGenerator, inputs, true, i => true);

                var location = SnapshotHelper.GetLocationFromCallingProjectRoot(null);
                SnapshotHelper.AssertSnapshot(snapshotGenerator.GetAllGenerated(), snapshotName, location);
            }
        }
    }
}
