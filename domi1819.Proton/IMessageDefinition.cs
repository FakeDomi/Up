namespace domi1819.Proton
{
    public interface IMessageDefinition<T>
    {
        void OnMessage(MessageContext context, T connectionObject);
    }
}
