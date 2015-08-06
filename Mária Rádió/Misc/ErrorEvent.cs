using System;

namespace Maria_Radio.Misc
{
    public class ErrorEvent
    {
        public delegate void onErrorDelegate(Exception e);
        public event onErrorDelegate onError;

        protected void OnError(Exception e)
        {
            if (onError != null)
            {
                onError(e);
            }
        }
    }
}
