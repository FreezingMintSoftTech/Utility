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
        private Program program_;

        public Process(IEnumerator func, Program program)
        {
            func_ = func;
            program_ = program;
        }

        public bool valid()
        {
            return null != func_;
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
            program_.clear();
        }
    }
}
