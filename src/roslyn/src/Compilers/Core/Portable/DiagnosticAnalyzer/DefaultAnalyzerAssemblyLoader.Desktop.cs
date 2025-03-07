﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETCOREAPP

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Loads analyzer assemblies from their original locations in the file system.
    /// Assemblies will only be loaded from the locations specified when the loader
    /// is instantiated.
    /// </summary>
    /// <remarks>
    /// This type is meant to be used in scenarios where it is OK for the analyzer
    /// assemblies to be locked on disk for the lifetime of the host; for example,
    /// csc.exe and vbc.exe. In scenarios where support for updating or deleting
    /// the analyzer on disk is required a different loader should be used.
    /// </remarks>
    internal class DefaultAnalyzerAssemblyLoader : AnalyzerAssemblyLoader
    {
        private readonly object _guard = new();
        private bool _hookedAssemblyResolve;

        internal DefaultAnalyzerAssemblyLoader()
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName, string _)
        {
            EnsureResolvedHooked();

            return AppDomain.CurrentDomain.Load(assemblyName);
        }

        internal bool EnsureResolvedHooked()
        {
            lock (_guard)
            {
                if (!_hookedAssemblyResolve)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                    _hookedAssemblyResolve = true;
                    return true;
                }
            }

            return false;
        }

        internal bool EnsureResolvedUnhooked()
        {
            lock (_guard)
            {
                if (_hookedAssemblyResolve)
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
                    _hookedAssemblyResolve = false;
                    return true;
                }
            }

            return false;
        }

        private Assembly? AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var displayName = AppDomain.CurrentDomain.ApplyPolicy(args.Name);
                var assemblyName = new AssemblyName(displayName);
                string? bestPath = GetBestPath(assemblyName);
                if (bestPath is not null)
                {
                    return Assembly.LoadFrom(bestPath);
                }

                return null;
            }
            catch
            {
                // In the .NET Framework, if a handler to AssemblyResolve throws an exception, other handlers
                // are not called. To avoid any bug in our handler breaking other handlers running in the same process
                // we catch exceptions here. We do not expect exceptions to be thrown though.
                return null;
            }
        }
    }
}

#endif
