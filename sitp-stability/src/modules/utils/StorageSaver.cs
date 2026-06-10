//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal interface IStorageSave
    {
        void Save(bool immediately);
    }

    internal abstract class StorageSaverCommon : IStorageSave
    {
        //int milliseconds
        protected int _saveTimeout;
        protected long _savedTime = 0;

        internal StorageSaverCommon(int saveTimeout = 10000)
        {
            _saveTimeout = saveTimeout;
        }

        protected bool IsRequireToSave(bool immediately)
        {
            //first request to save (ignored)   
            long timeMS = (DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
            if (0 == _savedTime)
                _savedTime = timeMS;

            long diffTime = timeMS - _savedTime;
            if ((immediately) || (_saveTimeout <= diffTime))
            {
                _savedTime = timeMS;
                return true;
            }
            return false;
        }
        
        protected abstract void SaveInternal();
        public virtual void Save(bool immediately)
        {
            if (IsRequireToSave(immediately))
                SaveInternal();
        }
    }

    internal class StorageSaverXml: StorageSaverCommon
    {
        protected string _filename;

        public StorageSaverXml(string filename) : this(filename, 10000)
        {
        }

        public StorageSaverXml(string filename, int saveTimeout) : base (saveTimeout)
        {
            _filename = filename;
        }
        
        protected override void SaveInternal()
        {
            //save to xml
        }
    }
}
