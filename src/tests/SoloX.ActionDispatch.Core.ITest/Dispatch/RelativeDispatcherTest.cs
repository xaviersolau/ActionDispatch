// ----------------------------------------------------------------------
// <copyright file="RelativeDispatcherTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.ITest.Dispatch.Behavior;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.Utils;
using Xunit;

namespace SoloX.ActionDispatch.Core.ITest.Dispatch
{
    public class RelativeDispatcherTest
    {
        [Fact]
        public void DispatchRelativeActionTest()
        {
            var state = new StateB()
            {
                Child = new StateBa()
                {
                    Value = "Some child value",
                },
                Value = "Some text",
            };

            DispatcherTest.SetupAndTestDispatcher<IStateB>(
                state,
                dispatcher =>
                {
                    string valueToSet = "some text";
                    string newValueFromRootDispatcher = null;
                    string newValueFromRelativeDispatcher = null;
                    using (var subscriber1 = dispatcher.State.SelectWhenChanged(s => s.Child.Value)
                        .Subscribe(v => newValueFromRootDispatcher = v))
                    {
                        var relativeDispatcher = dispatcher.CreateRelativeDispatcher(s => s.Child);

                        using (var subscriber2 = relativeDispatcher.State.SelectWhenChanged(s => s.Value)
                            .Subscribe(v => newValueFromRelativeDispatcher = v))
                        {
                            relativeDispatcher.Dispatch(new SetTextOnIStateBaActionBehavior(valueToSet), s => s);
                        }
                    }

                    Assert.Equal(valueToSet, newValueFromRootDispatcher);
                    Assert.Equal(valueToSet, newValueFromRelativeDispatcher);
                });
        }
    }
}
