﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex.Styling;

namespace Perspex.Controls
{
    public class UserControl : ContentControl, IStyleable
    {
        Type IStyleable.StyleKey // => 
        { get { return typeof(ContentControl); } }
    }
}
