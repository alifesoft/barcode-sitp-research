//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal interface IDataSave
    {
        void Save(bool immediately);
    }

    internal abstract class DataSaverCommon : IDataSave
    {
        //int milliseconds
        protected int _saveTimeout;
        protected long _savedTime = 0;

        internal DataSaverCommon(int saveTimeout = 10000)
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
}
