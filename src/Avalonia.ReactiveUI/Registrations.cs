// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Threading;


namespace ReactiveUI
{
    /// <summary>
    /// Ignore me. This class is a secret handshake between RxUI and RxUI.Xaml
    /// in order to register certain classes on startup that would be difficult
    /// to register otherwise.
    /// </summary>
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            // RxApp.MainThreadScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
        }
    }
}


namespace ReactiveUI
{
    using System;

    /// <summary>
    /// Allows an additional string to make view resolution more specific than
    /// just a type. When applied to your <see cref="IViewFor{T}"/> -derived
    /// View, you can select between different Views for a single ViewModel
    /// instance.
    /// </summary>
    public class ViewContractAttribute : Attribute
    {
        /// <summary>
        /// Constructs the ViewContractAttribute with a specific contract value.
        /// </summary>
        /// <param name="contract">The value of the contract for view
        /// resolution.</param>
        public ViewContractAttribute(string contract)
        {
            Contract = contract;
        }

        internal string Contract { get; set; }
    }

    /// <summary>
    /// Indicates that this View should be constructed _once_ and then used
    /// every time its ViewModel View is resolved.
    /// Obviously, this is not supported on Views that may be reused multiple
    /// times in the Visual Tree.
    /// </summary>
    public class SingleInstanceViewAttribute : Attribute
    {
    }
}
