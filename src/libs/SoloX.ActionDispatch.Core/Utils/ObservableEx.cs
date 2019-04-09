// ----------------------------------------------------------------------
// <copyright file="ObservableEx.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace SoloX.ActionDispatch.Core.Utils
{
    /// <summary>
    /// IObservable extensions.
    /// </summary>
    public static class ObservableEx
    {
        /// <summary>
        /// Catch exception from the reactive observable and continue subscription processing.
        /// </summary>
        /// <typeparam name="T">Type of the observable items.</typeparam>
        /// <typeparam name="TException">Exception type to catch.</typeparam>
        /// <param name="observable">Observable to catch exception from.</param>
        /// <param name="handler">Exception handler.</param>
        /// <returns>The resulting exception safe observable.</returns>
        public static IObservable<T> CatchAndContinue<T, TException>(this IObservable<T> observable, Action<TException> handler)
            where TException : Exception
        {
            var catchIt = new CatchAndContinueHandler<T, TException>(handler);
            var obs = observable.Catch<T, TException>(catchIt.CatchIt);
            catchIt.Observable = obs;
            return obs;
        }

        /// <summary>
        /// Select only changed element from the sequence.
        /// </summary>
        /// <typeparam name="TIn">Type of the element input element.</typeparam>
        /// <typeparam name="TOut">Type of the selected item.</typeparam>
        /// <param name="observable">Observable to select from.</param>
        /// <param name="selector">Selector delegate to get TOut from TIn.</param>
        /// <returns>The resulting selected observable.</returns>
        public static IObservable<TOut> SelectWhenChanged<TIn, TOut>(this IObservable<TIn> observable, Func<TIn, TOut> selector)
        {
            return observable.Select(selector).Where(new ReferenceChangeDetector<TOut>().HasChanged);
        }

        private class CatchAndContinueHandler<T, TException>
            where TException : Exception
        {
            private readonly Action<TException> handler;

            public CatchAndContinueHandler(Action<TException> handler)
            {
                this.handler = handler;
            }

            internal IObservable<T> Observable { get; set; }

            internal IObservable<T> CatchIt(TException e)
            {
                this.handler(e);

                return this.Observable;
            }
        }

        private class ReferenceChangeDetector<T>
        {
            private T oldValue;

            public bool HasChanged(T value)
            {
                var hasChanged = !ReferenceEquals(value, this.oldValue);
                this.oldValue = value;
                return hasChanged;
            }
        }
    }
}
