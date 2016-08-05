// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Avalonia.Markup
{
    /// <summary>
    /// Locates controls relative to other controls.
    /// </summary>
    public static class ControlLocator
    {
        /// <summary>
        /// Tracks a named control relative to another control.
        /// </summary>
        /// <param name="relativeTo">
        /// The control relative from which the other control should be found.
        /// </param>
        /// <param name="name">The name of the control to find.</param>
        public static IObservable<IControlScope> Track(IControlScope relativeTo, string name)
        {
            var attached = Observable.FromEventPattern<LogicalTreeAttachmentEventArgs>(
                x => relativeTo.DoAttachedToLogicalTree(x), //  += x,
                x => relativeTo.DoDetachedFromLogicalTree(x) // += x
                    )
                .Select(x => ((IControlScope)x.Sender).FindNameScope())
                .StartWith(relativeTo.FindNameScope());

            var detached = Observable.FromEventPattern<LogicalTreeAttachmentEventArgs>(
                x => relativeTo.DoAttachedToLogicalTree(x), // += x
                x => relativeTo.DoDetachedFromLogicalTree(x) // += x
                    )
                .Select(x => (INameScope)null);

            return attached.Merge(detached).Select(nameScope =>
            {
                if (nameScope != null)
                {
                    var registered = Observable.FromEventPattern<NameScopeEventArgs>(
                        x => nameScope.Registered += x,
                        x => nameScope.Registered -= x)
                        .Where(x => x.EventArgs.Name == name)
                        .Select(x => x.EventArgs.Element)
                        .OfType<IControlScope>();
                    var unregistered = Observable.FromEventPattern<NameScopeEventArgs>(
                        x => nameScope.Unregistered += x,
                        x => nameScope.Unregistered -= x)
                        .Where(x => x.EventArgs.Name == name)
                        .Select(_ => (IControlScope)null);
                    return registered
                        .StartWith(nameScope.Find<IControlScope>(name))
                        .Merge(unregistered);
                }
                else
                {
                    return Observable.Return<IControlScope>(null);
                }
            }).Switch();
        }

        public static void DoAttachedToLogicalTree(this IControlScope relativeTo, EventHandler<LogicalTreeAttachmentEventArgs> x)
        {
            relativeTo.AttachedToLogicalTree += x;
        }
        public static void DoDetachedFromLogicalTree(this IControlScope relativeTo, EventHandler<LogicalTreeAttachmentEventArgs> x)
        {
            // event EventHandler<LogicalTreeAttachmentEventArgs> DetachedFromVisualTree;
            relativeTo.DetachedFromLogicalTree += x;
        }
    }
}


/*
FindNameScope

FindNameScope' and the best extension method overload 'ControlExtensions.FindNameScope(IControl)' requires a receiver of type 'IControl'
cs(27,33,27,54): error CS1061: 'IControlLayout' does not contain a definition for 'AttachedToLogicalTree' and no extension method 'AttachedToLogicalTree' accepting a first argument of type 'IControlLayout' could be found(are you missing a using directive or an assembly reference?)
cs(28,33,28,56): error CS1061: 'IControlLayout' method 'DetachedFromLogicalTree' accepting a first argument of type 'IControlLayout' could be found(are you missing a using directive or an assembly reference?)
cs(33,33,33,56): error CS1061: 'IControlLayout' method 'DetachedFromLogicalTree' type 'IControlLayout' could be found
cs(34,33,34,56): error CS1061: 'IControlLayout' method 'DetachedFromLogicalTree' type 'IControlLayout' could be found
4>d:\webstack\WPF\Avalonia\src\Markup\Avalonia.Markup\ControlLocator.cs(37,20,60,24): error CS0266: 
Cannot implicitly convert type 'System.IObservable<Avalonia.Controls.IControl>' 
to 'System.IObservable<Avalonia.IControlLayout>'. An explicit conversion exists (are you missing a cast?)

*/
