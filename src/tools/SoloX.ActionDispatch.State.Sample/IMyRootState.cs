using SoloX.ActionDispatch.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.State.Sample
{
    public interface IMyRootState : IState
    {
        string Value1 { get; set; }

        int Value2 { get; set; }

        IMyChildState Child1 { get; set; }

        IMyChildState Child2 { get; set; }

    }
}
