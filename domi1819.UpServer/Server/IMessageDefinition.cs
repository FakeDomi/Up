using domi1819.UpCore.Network;
using domi1819.UpServer.Proton;

namespace domi1819.UpServer.Server
{
    internal interface IMessageDefinition
    {
        void OnMessage(MessageDeserializer deserializer, MessageSerializer serializer, ProtonConnectionUser user);
    }
}
