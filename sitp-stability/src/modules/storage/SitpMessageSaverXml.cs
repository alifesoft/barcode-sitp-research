//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal sealed class SitpMessageSaverXml : DataSaverCommon
    {
        private readonly string _filename;
        private readonly SitpMessageCollection _messages;

        internal SitpMessageCollection Messages
        {
            get { return _messages; }
        }

        internal SitpMessageSaverXml(string filename, SitpMessageCollection messages) : this(filename, messages, 10000)
        {
        }

        internal SitpMessageSaverXml(string filename, SitpMessageCollection messages, int saveTimeout) : base(saveTimeout)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename is empty.", nameof(filename));

            if (messages == null) throw new ArgumentNullException(nameof(messages));

            _filename = filename;
            _messages = messages;
        }

        protected override void SaveInternal()
        {
            SitpMessageXmlCodec.Write(_filename, _messages);
        }

        internal static SitpMessageCollection Load(string filename)
        {
            return SitpMessageXmlCodec.Read(filename);
        }
    }
}
