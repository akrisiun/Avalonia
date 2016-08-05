// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.LogicalTree;
using System;

namespace Avalonia.Controls
{
    /// <summary>
    /// Defines a name scope.
    /// </summary>
    public interface INameScope
    {
        /// <summary>
        /// Raised when an element is registered with the name scope.
        /// </summary>
        event EventHandler<NameScopeEventArgs> Registered;

        /// <summary>
        /// Raised when an element is unregistered with the name scope.
        /// </summary>
        event EventHandler<NameScopeEventArgs> Unregistered;

        /// <summary>
        /// Registers an element eith the name scope.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <param name="element">The element.</param>
        void Register(string name, object element);

        /// <summary>
        /// Finds a named element in the name scope.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The element, or null if the name was not found.</returns>
        object Find(string name);

        /// <summary>
        /// Unregisters an element with the name scope.
        /// </summary>
        /// <param name="name">The name.</param>
        void Unregister(string name);
    }

    public interface IControlScope
    {
        //AttachedToLogicalTree VisualTreeAttachmentEventArgs AttachedToVisualTree
        event EventHandler<LogicalTreeAttachmentEventArgs> AttachedToLogicalTree;

        /// <summary>
        /// Raised when the control is detached from a rooted visual tree.
        /// </summary>
        event EventHandler<LogicalTreeAttachmentEventArgs> DetachedFromLogicalTree;
        //VisualTreeAttachmentEventArgs  DetachedFromLogicalTree

        INameScope FindNameScope();
    }
}


//public static INameScope FindNameScope(this IControlLayout control)
//{
//    Contract.Requires<ArgumentNullException>(control != null);

//    return control.GetSelfAndLogicalAncestors()
//        .OfType<Control>()
//        .Select(x => (x as INameScope) ?? NameScope.GetNameScope(x))
//        .FirstOrDefault(x => x != null);
//}
