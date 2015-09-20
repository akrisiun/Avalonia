// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.Generic;
using OmniXaml;

namespace Perspex.Markup.Xaml.Templates
{
    public class TemplateLoader : IDeferredLoader
    {
        public object Load(IEnumerable<XamlInstruction> nodes, IWiringContext context)
        {
            return new TemplateContent(nodes, context);
        }
    }
}