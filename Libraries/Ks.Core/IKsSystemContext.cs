using Ks.Core.Domain.System;

namespace Ks.Core
{
    public interface IKsSystemContext
    {
        KsSystem CurrentSystem { get; }
    }
}