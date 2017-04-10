using System.Collections.Generic;

namespace domi1819.Proton
{
    public class MessageRegistry<T>
    {
        private readonly Dictionary<int, IMessageDefinition<T>> messages = new Dictionary<int, IMessageDefinition<T>>();

        public IMessageDefinition<T> this[int id]
        {
            get
            {
                IMessageDefinition<T> message;
                return this.messages.TryGetValue(id, out message) ? message : null;
            }
        }

        public void Register(IMessageDefinition<T> message, int id)
        {
            this.messages.Add(id, message);
        }
    }
}
