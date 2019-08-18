// ----------------------------------------------------------------------
// <copyright file="ObservableTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.Utils;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.Utils
{
    public class ObservableTest
    {
        [Fact]
        public void SelectWhenChangedTest()
        {
            var initialValue = "Some initial value";
            var newValue = "Some new text";

            var initialState = new StateB()
            {
                Value = initialValue,
            };

            string changed = null;
            using (var stateSubject = new BehaviorSubject<IStateB>(initialState))
            using (var subscription = stateSubject.SelectWhenChanged(s => s.Value)
                .Subscribe(s =>
                {
                    changed = s;
                }))
            {
                Assert.Equal(initialValue, changed);

                initialState.Value = newValue;
                stateSubject.OnNext(initialState);

                Assert.Equal(newValue, changed);
            }
        }

        [Fact]
        public void CatchAndContinueTest()
        {
            var initialValue = "Some initial value";
            var throwValue = "throw";
            var newValue = "Some new text";

            var initialState = new StateB()
            {
                Value = initialValue,
            };

            Exception catched = null;
            string changed = null;
            using (var stateSubject = new BehaviorSubject<IStateB>(initialState))
            using (var subscription = stateSubject.SelectWhenChanged(s => s.Value)
                .Where(s =>
                {
                    if (s == throwValue)
                    {
                        throw new Exception();
                    }

                    return true;
                })
                .CatchAndContinue<string, Exception>(e => catched = e)
                .Subscribe(s =>
                {
                    changed = s;
                }))
            {
                Assert.Equal(initialValue, changed);

                initialState.Value = throwValue;
                stateSubject.OnNext(initialState);

                Assert.NotNull(catched);
                Assert.Equal(initialValue, changed);

                catched = null;

                initialState.Value = newValue;
                stateSubject.OnNext(initialState);

                Assert.Null(catched);
                Assert.Equal(newValue, changed);
            }
        }
    }
}
