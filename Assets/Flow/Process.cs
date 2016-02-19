/**
@file Process.cs
@author t-sakai
@date 2016/02/19
*/
using System.Collections;

namespace Flow
{
    public struct Process
    {
        private IEnumerator func_;

        public Process(IEnumerator func)
        {
            //System.Diagnostics.Debug.Assert(null != func);
            func_ = func;
        }

        public bool run()
        {
            return func_.MoveNext();
        }

        public IEnumerator Func
        {
            get { return func_; }
        }

        public void reset()
        {
            func_.Reset();
        }

        public void clear()
        {
            func_ = null;
        }
    }
}
