﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace SOS.Hosting.DbgEng.Interop
{
    [StructLayout(LayoutKind.Explicit)]
    public struct LANGANDCODEPAGE
    {
        [FieldOffset(0)]
        public ushort wLanguage;
        [FieldOffset(2)]
        public ushort wCodePage;
    }
}