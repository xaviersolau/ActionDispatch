using SoloX.ActionDispatch.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.State.Sample
{
    public interface IMyChildState : IState<IMyChildState>
    {
        string Value1 { get; set; }

    }
}
