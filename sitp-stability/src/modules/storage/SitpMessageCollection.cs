//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal sealed class SitpMessageCollection
    {
        private readonly List<SitpMessage> _messages;

        internal List<SitpMessage> Messages
        {
            get { return _messages; }
        }

        internal int Count
        {
            get { return _messages.Count; }
        }

        internal SitpMessageCollection()
        {
            _messages = new List<SitpMessage>();
        }

        internal SitpMessageCollection(IEnumerable<SitpMessage> messages)
        {
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            _messages = new List<SitpMessage>(messages);
        }

        internal void Add(SitpMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            _messages.Add(message);
        }

        internal void Clear()
        {
            _messages.Clear();
        }
    }
}
