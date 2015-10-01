﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NGenerics.DataStructures.Queues;
using Perspex.Platform;
using Perspex.Threading;
using Splat;

namespace Perspex.Win32.Threading
{
    /// <summary>
    /// A main loop in a <see cref="Dispatcher"/>.
    /// </summary>
    internal class MainLoop
    {
        private static readonly IPlatformThreadingInterface s_platform;

        private readonly PriorityQueue<Job, DispatcherPriority> _queue =
            new PriorityQueue<Job, DispatcherPriority>(PriorityQueueType.Maximum);

        /// <summary>
        /// Initializes static members of the <see cref="MainLoop"/> class.
        /// </summary>
        static MainLoop()
        {
            s_platform = Locator.Current.GetService<IPlatformThreadingInterface>();
        }

        /// <summary>
        /// Runs the main loop.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to exit the main loop.
        /// </param>
        public void Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                RunJobs();

                s_platform.ProcessMessage();
            }
        }

        /// <summary>
        /// Runs continuations pushed on the loop.
        /// </summary>
        public void RunJobs()
        {
            Job job = null;

            while (job != null || _queue.Count > 0)
            {
                if (job == null)
                {
                    lock (_queue)
                    {
                        job = _queue.Dequeue();
                    }
                }

                if (job.Priority < DispatcherPriority.Input && s_platform.HasMessages())
                {
                    break;
                }

                if (job.TaskCompletionSource == null)
                {
                    job.Action();
                }
                else
                {
                    try
                    {
                        job.Action();
                        job.TaskCompletionSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        job.TaskCompletionSource.SetException(e);
                    }
                }

                job = null;
            }
        }

        /// <summary>
        /// Invokes a method on the main loop.
        /// </summary>
        /// <param name="action">The method.</param>
        /// <param name="priority">The priority with which to invoke the method.</param>
        /// <returns>A task that can be used to track the method's execution.</returns>
        public Task InvokeAsync(Action action, DispatcherPriority priority)
        {
            var job = new Job(action, priority, false);
            AddJob(job);
            return job.TaskCompletionSource.Task;
        }

        /// <summary>
        /// Post action that will be invoked on main thread
        /// </summary>
        /// <param name="action">The method.</param>
        /// 
        /// <param name="priority">The priority with which to invoke the method.</param>
        internal void Post(Action action, DispatcherPriority priority)
        {
            AddJob(new Job(action, priority, true));
        }

        private void AddJob(Job job)
        {
            lock (_queue)
            {
                _queue.Add(job, job.Priority);
            }
            s_platform.Wake();
        }

        /// <summary>
        /// A job to run.
        /// </summary>
        private class Job
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Job"/> class.
            /// </summary>
            /// <param name="action">The method to call.</param>
            /// <param name="priority">The job priority.</param>
            /// <param name="throwOnUiThread">Do not wrap excepption in TaskCompletionSource</param>
            public Job(Action action, DispatcherPriority priority, bool throwOnUiThread)
            {
                Action = action;
                Priority = priority;
                TaskCompletionSource = throwOnUiThread ? null : new TaskCompletionSource<object>();
            }

            /// <summary>
            /// Gets the method to call.
            /// </summary>
            public Action Action { get; set; }

            /// <summary>
            /// Gets the job priority.
            /// </summary>
            public DispatcherPriority Priority { get; set; }

            /// <summary>
            /// Gets the task completion source.
            /// </summary>
            public TaskCompletionSource<object> TaskCompletionSource { get; set; }
        }
    }
}
