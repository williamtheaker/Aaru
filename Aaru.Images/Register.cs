﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Register.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Core algorithms.
//
// --[ Description ] ----------------------------------------------------------
//
//     Registers all plugins in this assembly.
//
// --[ License ] --------------------------------------------------------------
//
//     Permission is hereby granted, free of charge, to any person obtaining a
//     copy of this software and associated documentation files (the
//     "Software"), to deal in the Software without restriction, including
//     without limitation the rights to use, copy, modify, merge, publish,
//     distribute, sublicense, and/or sell copies of the Software, and to
//     permit persons to whom the Software is furnished to do so, subject to
//     the following conditions:
//
//     The above copyright notice and this permission notice shall be included
//     in all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//     OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//     MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//     IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//     CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//     TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//     SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2022 Natalia Portillo
// ****************************************************************************/

namespace Aaru.DiscImages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aaru.CommonTypes.Interfaces;

/// <inheritdoc />
public sealed class Register : IPluginRegister
{
    /// <inheritdoc />
    public List<Type> GetAllChecksumPlugins() => null;

    /// <inheritdoc />
    public List<Type> GetAllFilesystemPlugins() => null;

    /// <inheritdoc />
    public List<Type> GetAllFilterPlugins() => null;

    /// <inheritdoc />
    public List<Type> GetAllFloppyImagePlugins() => Assembly.GetExecutingAssembly().GetTypes().
                                                             Where(t => t.GetInterfaces().
                                                                          Contains(typeof(IFloppyImage))).
                                                             Where(t => t.IsClass).ToList();

    /// <inheritdoc />
    public List<Type> GetAllMediaImagePlugins() => Assembly.GetExecutingAssembly().GetTypes().
                                                            Where(t => t.GetInterfaces().Contains(typeof(IMediaImage))).
                                                            Where(t => t.IsClass).ToList();

    /// <inheritdoc />
    public List<Type> GetAllPartitionPlugins() => null;

    /// <inheritdoc />
    public List<Type> GetAllReadOnlyFilesystemPlugins() => null;

    /// <inheritdoc />
    public List<Type> GetAllWritableFloppyImagePlugins() => Assembly.GetExecutingAssembly().GetTypes().
                                                                     Where(t => t.GetInterfaces().
                                                                               Contains(typeof(IWritableFloppyImage))).
                                                                     Where(t => t.IsClass).ToList();

    /// <inheritdoc />
    public List<Type> GetAllWritableImagePlugins() => Assembly.GetExecutingAssembly().GetTypes().
                                                               Where(t => t.GetInterfaces().
                                                                            Contains(typeof(IBaseWritableImage))).
                                                               Where(t => t.IsClass).ToList();

    /// <inheritdoc />
    public List<Type> GetAllArchivePlugins() => null;

    /// <inheritdoc />
    public List<Type> GetAllByteAddressablePlugins() => Assembly.GetExecutingAssembly().GetTypes().
                                                                 Where(t => t.GetInterfaces().
                                                                              Contains(typeof(IByteAddressableImage))).
                                                                 Where(t => t.IsClass).ToList();
}