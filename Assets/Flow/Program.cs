/**
@file Program.cs
@author t-sakai
@date 2016/03/2
*/

namespace Flow
{
    public class Program
    {
        private SequenceCache cache_ = new SequenceCache();

        public void clear()
        {
            cache_.clear();
        }

        public Process build(System.Collections.IEnumerator func)
        {
            return new Process(func, this);
        }

        public Sequence sequence()
        {
            return cache_.sequence();
        }

        public Concurrent concurrent()
        {
            return cache_.concurrent();
        }

        public DelayedConcurrent delayedConcurrent()
        {
            return cache_.delayedConcurrent();
        }
    };
}
