// ----------------------------------------------------------------------
// <copyright file="ObservableTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.Utils
{
    public class ObservableTest
    {
        [Fact]
        public void SelectWhenChangedTest()
        {
            var initialState = new StateB()
            {
                Value = "Some initial value",
            };

            using (var stateSubject = new BehaviorSubject<IStateB>(initialState))
            {
                // stateSubject.OnNext();
            }
        }
    }
}
