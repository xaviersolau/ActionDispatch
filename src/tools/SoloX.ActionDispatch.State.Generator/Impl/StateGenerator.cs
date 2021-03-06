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
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.State.Generator.Patterns.Impl;
using SoloX.ActionDispatch.State.Generator.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator.Impl;
using SoloX.GeneratorTools.Core.CSharp.Generator.Writer.Impl;
using SoloX.GeneratorTools.Core.CSharp.Model;
using SoloX.GeneratorTools.Core.CSharp.Model.Resolver;
using SoloX.GeneratorTools.Core.CSharp.Model.Use;
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
        public void Generate(string projectFile, string inputsFile, string outputsFile, bool generate)
        {
            var projectFolder = Path.GetDirectoryName(projectFile);

            this.logger.LogInformation($"Loading {Path.GetFileName(projectFile)}...");

            var project = this.workspace.RegisterProject(projectFile);

            var inputs = new HashSet<string>();
            var outputs = new HashSet<string>();

            var locator = new RelativeLocator(projectFolder, project.RootNameSpace, suffix: "Impl");
            var fileGenerator = new FileGenerator(".generated.cs", f => outputs.Add(Path.GetFullPath(f)));

            // Generate with a filter on current project interface declarations.
            this.Generate(
                locator,
                fileGenerator,
                inputs,
                generate,
                itfDeclaration => IsDeclarationInProject(itfDeclaration, project));

            if (generate)
            {
                WriteFileList(outputs, outputsFile, Path.GetFullPath(Path.GetDirectoryName(projectFile)));
            }

            WriteFileList(inputs, inputsFile, Path.GetFullPath(Path.GetDirectoryName(projectFile)));
        }

        internal void Generate(ILocator locator, IGenerator fileGenerator, HashSet<string> inputs, bool generate, Func<IInterfaceDeclaration, bool> generateFilter)
        {
            this.workspace.RegisterFile(GetContentFile("./Patterns/Itf/IParentStatePattern.cs"));
            this.workspace.RegisterFile(GetContentFile("./Patterns/Itf/IChildStatePattern.cs"));
            this.workspace.RegisterFile(GetContentFile("./Patterns/Impl/ParentStatePattern.cs"));
            this.workspace.RegisterFile(GetContentFile("./Patterns/Impl/StateFactoryProviderPattern.cs"));

            var resolver = this.workspace.DeepLoad();

            var declaration = resolver.Find(typeof(IState).FullName)
                .Cast<IGenericDeclaration<SyntaxNode>>().Single();
            var itfParentPatternDeclaration = resolver.Find(typeof(IParentStatePattern).FullName)
                .Single() as IInterfaceDeclaration;
            var itfChildPatternDeclaration = resolver.Find(typeof(IChildStatePattern).FullName)
                .Single() as IInterfaceDeclaration;

            var implPatternDeclaration = resolver.Find(typeof(ParentStatePattern).FullName)
                .Single() as IGenericDeclaration<SyntaxNode>;

            ImplementationGenerator generator = null;
            if (generate)
            {
                generator = new ImplementationGenerator(
                    fileGenerator,
                    locator,
                    itfParentPatternDeclaration,
                    implPatternDeclaration);
            }

            var generatedClasses = new List<(string, string)>();
            var interfaceNameSpaceList = new HashSet<string>();

            var extendedBy = declaration.ExtendedBy
                .Where(d => d is IInterfaceDeclaration && d != itfParentPatternDeclaration && d != itfChildPatternDeclaration);

            foreach (var extendedByItem in extendedBy)
            {
                var itfDeclaration = (IInterfaceDeclaration)extendedByItem;

                // We must generate only the filtered interface declaration.
                var isInProject = generateFilter(itfDeclaration);

                if (generate)
                {
                    this.logger.LogInformation(extendedByItem.FullName);

                    interfaceNameSpaceList.Add(itfDeclaration.DeclarationNameSpace);

                    var implName = GeneratorHelper.ComputeClassName(itfDeclaration.Name);

                    if (isInProject)
                    {
                        var writerSelector = CreateWriterSelector(
                            itfParentPatternDeclaration,
                            implPatternDeclaration,
                            itfDeclaration,
                            implName);

                        var className = generator.Generate(writerSelector, itfDeclaration, implName);
                        generatedClasses.Add(className);
                    }
                }

                if (isInProject)
                {
                    inputs.Add(itfDeclaration.Location);
                }
            }

            if (generate)
            {
                GenerateFactory(resolver, locator, fileGenerator, generatedClasses, interfaceNameSpaceList);
            }
        }

        private static bool IsDeclarationInProject(IInterfaceDeclaration itfDeclaration, ICSharpProject project)
        {
            var filePath = itfDeclaration.Location;
            var projectPath = project.ProjectPath;

            return filePath.StartsWith(projectPath, StringComparison.InvariantCulture);
        }

        private static void WriteFileList(HashSet<string> list, string file, string projectFolder)
        {
            if (!string.IsNullOrEmpty(file))
            {
                File.WriteAllLines(file, list.Select(f => f.Replace(projectFolder, string.Empty).Substring(1)));
            }
        }

        private static string GetContentFile(string contentFile)
        {
            return Path.Combine(Path.GetDirectoryName(typeof(StateGenerator).Assembly.Location), contentFile);
        }

        private static void GenerateFactory(
            IDeclarationResolver resolver,
            ILocator locator,
            IGenerator fileGenerator,
            List<(string nameSpace, string name)> generatedClasses,
            HashSet<string> interfaceNameSpaceList)
        {
            var stateFactoryItfDecl = resolver
                .Find(typeof(IStateFactoryProvider).FullName)
                .Single() as IInterfaceDeclaration;
            var factoryPatternDeclaration = resolver
                .Find(typeof(StateFactoryProviderPattern).FullName)
                .Single() as IGenericDeclaration<SyntaxNode>;

            var generator = new ImplementationGenerator(
                fileGenerator,
                locator,
                stateFactoryItfDecl,
                factoryPatternDeclaration);

            var implName = GeneratorHelper.ComputeClassName(stateFactoryItfDecl.Name);

            var nsList = new HashSet<string>(generatedClasses.Select(n => n.nameSpace));

            var nsWriter = new StringReplaceWriter(
                "SoloX.ActionDispatch.State.Generator.Patterns.Impl",
                nsList.ToArray());
            var nsItfWriter = new StringReplaceWriter(
                "SoloX.ActionDispatch.State.Generator.Patterns.Itf",
                interfaceNameSpaceList.Where(ns => !factoryPatternDeclaration.UsingDirectives.Contains(ns))
                .ToArray());
            var ctorWriter = new StringReplaceWriter(
                nameof(ParentStatePattern),
                generatedClasses.Select(n => n.name).ToArray());

            var implNameWriter = new StringReplaceWriter(factoryPatternDeclaration.Name, implName);

            var writerSelector = new WriterSelector(ctorWriter, nsWriter, nsItfWriter, implNameWriter);

            generator.Generate(writerSelector, stateFactoryItfDecl, implName);
        }

        private static IWriterSelector CreateWriterSelector(
            IInterfaceDeclaration itfParentPattern,
            IGenericDeclaration<SyntaxNode> implPattern,
            IInterfaceDeclaration itfDeclaration,
            string implName)
        {
            var propertyWriter = new PropertyWriter(
                itfParentPattern.Properties.Single(x => x.Name == nameof(IParentStatePattern.PropertyPattern)),
                itfDeclaration.Properties.Where(IsNotIStateProperty).ToArray());

            var propertyChildWriter = new PropertyWriter(
                itfParentPattern.Properties.Single(x => x.Name == nameof(IParentStatePattern.ChildPattern)),
                itfDeclaration.Properties.Where(IsIStateProperty).ToArray());

            var propertyChildrenWriter = new PropertyWriter(
                itfParentPattern.Properties.Single(x => x.Name == nameof(IParentStatePattern.ChildrenPattern)),
                itfDeclaration.Properties.Where(IsIStateCollectionProperty).ToArray(),
                TypeParamExtract);

            var itfNs = new HashSet<string>();
            itfNs.Add(itfDeclaration.DeclarationNameSpace);
            foreach (var ns in itfDeclaration.UsingDirectives)
            {
                if (!implPattern.UsingDirectives.Contains(ns))
                {
                    itfNs.Add(ns);
                }
            }

            var usingWriter = new StringReplaceWriter("SoloX.ActionDispatch.State.Generator.Patterns.Itf", itfNs.ToArray());
            var itfNameWriter = new StringReplaceWriter(itfParentPattern.Name, itfDeclaration.Name);
            var implNameWriter = new StringReplaceWriter(implPattern.Name, implName);

            return new WriterSelector(propertyChildrenWriter, propertyChildWriter, propertyWriter, usingWriter, itfNameWriter, implNameWriter);
        }

        private static string TypeParamExtract(string type)
        {
            var start = type.IndexOf('<');
            var end = type.IndexOf('>');
            if (start > 0 && end > start
                && (type.Substring(0, start) == nameof(ICollection<object>)
                    || type.Substring(0, start) == nameof(IList<object>)
                    || type.Substring(0, start) == nameof(IEnumerable<object>)
                    || type.Substring(0, start) == nameof(IStateCollection<IState>)))
            {
                return type.Substring(start, end - start).Trim();
            }

            return type;
        }

        private static bool IsIStateDeclaration(IDeclaration<SyntaxNode> declaration)
        {
            if (declaration is IInterfaceDeclaration idef
                && declaration.FullName != $"{typeof(IStateCollection<>).Namespace}.{nameof(IStateCollection<IState>)}")
            {
                return idef.Extends
                    .Any(d => d.Declaration.FullName == typeof(IState).FullName);
            }

            return false;
        }

        private static bool IsIStateProperty(IPropertyDeclaration p)
        {
            return IsIStateDeclaration(p.PropertyType.Declaration);
        }

        private static bool IsIStateCollectionProperty(IPropertyDeclaration p)
        {
            if (p.PropertyType is IGenericDeclarationUse genUse
                && p.PropertyType.Declaration is IInterfaceDeclaration iDef)
            {
                if (iDef.FullName == $"{typeof(ICollection<>).Namespace}.{nameof(ICollection<object>)}"
                    || iDef.FullName == $"{typeof(IList<>).Namespace}.{nameof(IList<object>)}"
                    || iDef.FullName == $"{typeof(IStateCollection<>).Namespace}.{nameof(IStateCollection<IState>)}")
                {
                    return IsIStateDeclaration(genUse.GenericParameters.First().Declaration);
                }
            }

            return false;
        }

        private static bool IsNotIStateProperty(IPropertyDeclaration p)
        {
            return !IsIStateProperty(p) && !IsIStateCollectionProperty(p);
        }
    }
}
