using System;

namespace NClass.DependencyResolution
{
    internal abstract class CommitToContainer<TContainer>
    {
        protected Action<TContainer> _commitAction;

        public void Commit(TContainer container)
        {
            _commitAction(container);
        }
    }
}