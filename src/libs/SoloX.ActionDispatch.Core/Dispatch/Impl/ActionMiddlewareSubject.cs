// ----------------------------------------------------------------------
// <copyright file="ActionMiddlewareSubject.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.Utils;

namespace SoloX.ActionDispatch.Core.Dispatch.Impl
{
    internal class ActionMiddlewareSubject<TRootState> : IDisposable
        where TRootState : IState
    {
        private readonly Subject<IAction<TRootState, IActionBehavior>> actionSubject;
        private readonly IDisposable subscription;

        public ActionMiddlewareSubject(
            IActionMiddleware<TRootState> actionMiddleware,
            Action<IAction<TRootState, IActionBehavior>> publish,
            Action<Exception> errorReport)
        {
            this.ActionMiddleware = actionMiddleware;
            this.actionSubject = new Subject<IAction<TRootState, IActionBehavior>>();
            this.subscription = actionMiddleware
                .Setup(this.actionSubject.AsObservable())
                .CatchAndContinue(errorReport)
                .Subscribe((action) =>
                    Task.Run(() => publish(action))
                        .ContinueWith(
                            t => errorReport(t.Exception),
                            CancellationToken.None,
                            TaskContinuationOptions.OnlyOnFaulted,
                            TaskScheduler.Current));
        }

        public IActionMiddleware<TRootState> ActionMiddleware { get; }

        public bool Dispatch(IAction<TRootState, IActionBehavior> action)
        {
            if (this.ActionMiddleware.IsApplying(action.Behavior))
            {
                this.actionSubject.OnNext(action);
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.subscription.Dispose();
                this.actionSubject.Dispose();
            }
        }
    }
}
