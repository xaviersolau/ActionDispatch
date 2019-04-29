﻿// ----------------------------------------------------------------------
// <copyright file="StateGenerator.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.State.Generator.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator.Impl;
using SoloX.GeneratorTools.Core.CSharp.Generator.Writer.Impl;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Model.Resolver;
using SoloX.GeneratorTools.Core.CSharp.Workspace;
using SoloX.GeneratorTools.Core.Generator;
using SoloX.GeneratorTools.Core.Generator.Impl;
using SoloX.GeneratorTools.Core.Generator.Writer;
using SoloX.GeneratorTools.Core.Generator.Writer.Impl;
using SoloX.GeneratorTools.Core.Utils;

namespace SoloX.ActionDispatch.State.Generator.Impl
{
    /// <summary>
    /// The state generator implementation.
    /// </summary>
    public class StateGenerator : IStateGenerator
    {
        private readonly ILogger<StateGenerator> logger;
        private readonly ICSharpWorkspace workspace;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateGenerator"/> class.
        /// </summary>
        /// <param name="logger">Logger that will be used for logs.</param>
        /// <param name="workspace">The workspace to use.</param>
        public StateGenerator(ILogger<StateGenerator> logger, ICSharpWorkspace workspace)
        {
            this.logger = logger;
            this.workspace = workspace;
        }

        /// <inheritdoc/>
        public void Generate(string projectFile, string projectNameSpace)
        {
            var projectFolder = Path.GetDirectoryName(projectFile);

            this.logger.LogInformation($"Loading {Path.GetFileName(projectFile)}...");

            this.workspace.RegisterProject(projectFile);

            this.workspace.RegisterFile("./Patterns/Itf/IParentStatePattern.cs");
            this.workspace.RegisterFile("./Patterns/Itf/IChildStatePattern.cs");
            this.workspace.RegisterFile("./Patterns/Impl/ParentStatePattern.cs");
            this.workspace.RegisterFile("./Patterns/Impl/StateFactoryPattern.cs");

            var resolver = this.workspace.DeepLoad();

            var declaration = resolver.Find("SoloX.ActionDispatch.Core.State.IState").Cast<IGenericDeclaration>().Single(d => d.GenericParameters.Any());
            var itfParentPatternDeclaration = resolver.Find("SoloX.ActionDispatch.State.Generator.Patterns.Itf.IParentStatePattern").Single() as IInterfaceDeclaration;
            var itfChildPatternDeclaration = resolver.Find("SoloX.ActionDispatch.State.Generator.Patterns.Itf.IChildStatePattern").Single() as IInterfaceDeclaration;

            var implPatternDeclaration = resolver.Find("SoloX.ActionDispatch.State.Generator.Patterns.Impl.ParentStatePattern").Single() as IGenericDeclaration;

            var locator = new RelativeLocator(projectFolder, projectNameSpace, suffix: "Impl");

            var generator = new ImplementationGenerator(
                new FileGenerator(),
                locator,
                itfParentPatternDeclaration,
                implPatternDeclaration);

            var generatedClasses = new List<(string, string)>();

            foreach (var extendedByItem in declaration.ExtendedBy.Where(d => d is IInterfaceDeclaration && d != itfParentPatternDeclaration && d != itfChildPatternDeclaration))
            {
                this.logger.LogInformation(extendedByItem.FullName);

                var itfDeclaration = (IInterfaceDeclaration)extendedByItem;

                var implName = GeneratorHelper.ComputeClassName(itfDeclaration.Name);

                var writerSelector = this.CreateWriterSelector(itfParentPatternDeclaration, itfChildPatternDeclaration, implPatternDeclaration, itfDeclaration, implName);

                var className = generator.Generate(writerSelector, itfDeclaration, implName);
                generatedClasses.Add(className);
            }

            this.GenerateFactory(resolver, locator, generatedClasses);
        }

        private void GenerateFactory(IDeclarationResolver resolver, RelativeLocator locator, List<(string nameSpace, string name)> generatedClasses)
        {
            var stateFactoryItfDecl = resolver.Find("SoloX.ActionDispatch.Core.State.IStateFactory").Single() as IInterfaceDeclaration;
            var factoryPatternDeclaration = resolver.Find("SoloX.ActionDispatch.State.Generator.Patterns.Impl.StateFactoryPattern").Single() as IGenericDeclaration;

            var generator = new ImplementationGenerator(
                new FileGenerator(),
                locator,
                stateFactoryItfDecl,
                factoryPatternDeclaration);

            var implName = GeneratorHelper.ComputeClassName(stateFactoryItfDecl.Name);

            var nsList = new HashSet<string>(generatedClasses.Select(n => n.nameSpace));

            var nsWriter = new StringReplaceWriter("SoloX.ActionDispatch.State.Generator.Patterns.Itf", nsList.ToArray());
            var ctorWriter = new StringReplaceWriter("ParentStatePattern", generatedClasses.Select(n => n.name).ToArray());
            var implNameWriter = new StringReplaceWriter(factoryPatternDeclaration.Name, implName);

            var writerSelector = new WriterSelector(ctorWriter, nsWriter, implNameWriter);

            generator.Generate(writerSelector, stateFactoryItfDecl, implName);
        }

        private IWriterSelector CreateWriterSelector(
            IInterfaceDeclaration itfParentPattern,
            IInterfaceDeclaration itfChildPattern,
            IGenericDeclaration implPattern,
            IInterfaceDeclaration itfDeclaration,
            string implName)
        {
            var propertyWriter = new PropertyWriter(
                itfParentPattern.Properties.Single(x => x.PropertyType.Declaration != itfChildPattern),
                itfDeclaration.Properties.Where(p => p.PropertyType.Declaration is IPredefinedDeclaration).ToArray());

            var propertyChildWriter = new PropertyWriter(
                itfParentPattern.Properties.Single(x => x.PropertyType.Declaration == itfChildPattern),
                itfDeclaration.Properties.Where(p => p.PropertyType.Declaration is IInterfaceDeclaration).ToArray());

            var itfNameWriter = new StringReplaceWriter(itfParentPattern.Name, itfDeclaration.Name);
            var implNameWriter = new StringReplaceWriter(implPattern.Name, implName);

            return new WriterSelector(propertyChildWriter, propertyWriter, itfNameWriter, implNameWriter);
        }
    }
}