using System.Collections.Generic;

namespace domi1819.Proton
{
    public class MessageRegistry<T>
    {
        private Dictionary<int, IMessageDefinition<T>> messages;

        public void Register(IMessageDefinition<T> message, int id)
        {
            this.messages.Add(id, message);
        }


    }
}
