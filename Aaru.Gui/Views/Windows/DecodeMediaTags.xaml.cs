﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : DecodeMediaTags.xaml.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : GUI windows.
//
// --[ Description ] ----------------------------------------------------------
//
//     Media tags decoding window.
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General public License for more details.
//
//     You should have received a copy of the GNU General public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2022 Natalia Portillo
// ****************************************************************************/

namespace Aaru.Gui.Views.Windows;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

public sealed class DecodeMediaTags : Window
{
    public DecodeMediaTags()
    {
        InitializeComponent();
    #if DEBUG
        this.AttachDevTools();
    #endif
    }

    void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}