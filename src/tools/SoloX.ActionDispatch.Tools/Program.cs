// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.ActionDispatch.Tools
{
    /// <summary>
    /// Program entry point class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args">Tools arguments.</param>
        public static void Main(string[] args)
        {
            new Program().Run();
        }

        internal void Run()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
