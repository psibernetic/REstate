using Susanoo.ConnectionPooling;

namespace REstate.Repositories.Instances.Susanoo
{
    public abstract class InstanceContextualRepository
        : IInstanceContextualRepository
    {
        protected InstanceContextualRepository(InstanceRepository root)
        {
            Root = root;
        }

        public string ApiKey
            => Root.ApiKey;

        IInstanceRepository IInstanceContextualRepository.Root
            => this.Root;

        public InstanceRepository Root { get; }

        public virtual IDatabaseManagerPool DatabaseManagerPool
            => Root.DatabaseManagerPool;
    }
}