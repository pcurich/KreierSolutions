using System.Collections.Generic;

namespace Ks.Core.Domain.Messages
{

    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="U"></typeparam>
    public class EntityTokensAddedEvent<T, U> where T : BaseEntity
    {
        private readonly T _entity;
        private readonly IList<U> _tokens;

        public EntityTokensAddedEvent(T entity, IList<U> tokens)
        {
            _entity = entity;
            _tokens = tokens;
        }

        public T Entity { get { return _entity; } }
        public IList<U> Tokens { get { return _tokens; } }
    }

    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class MessageTokensAddedEvent<U>
    {
        private readonly MessageTemplate _message;
        private readonly IList<U> _tokens;

        public MessageTokensAddedEvent(MessageTemplate message, IList<U> tokens)
        {
            _message = message;
            _tokens = tokens;
        }

        public MessageTemplate Message { get { return _message; } }
        public IList<U> Tokens { get { return _tokens; } }
    }

}