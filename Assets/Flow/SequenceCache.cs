/**
@file SequenceCache.cs
@author t-sakai
@date 2016/02/19
*/
using System.Collections;

namespace Flow
{
    public class SequenceCache
    {
        private struct Cache<T> where T:EnumeratorSet, new()
        {
            public int top_;
            public T[] items_;

            public Cache(int capacity)
            {
                top_ = 0;
                items_ = new T[capacity];
                for(int i = 0; i < capacity; ++i) {
                    items_[i] = new T();
                }
            }

            public T get()
            {
                if(items_.Length <= top_) {
                    expand();
                }
                return items_[top_++];
            }

            public void clear()
            {
                for(int i = 0; i < top_; ++i) {
                    items_[i].Clear();
                }
                top_ = 0;
            }

            private void expand()
            {
                int newSize = items_.Length + Config.ExpandSize;
                T[] items = new T[newSize];
                System.Array.Copy(items_, items, top_);
                for(int i = top_; i < newSize; ++i) {
                    items[i] = new T();
                }
                items_ = items;
            }
        }

        private struct CacheDelayed<T> where T:DelayedConcurrent, new()
        {
            public int top_;
            public T[] items_;

            public CacheDelayed(int capacity)
            {
                top_ = 0;
                items_ = new T[capacity];
                for(int i = 0; i < capacity; ++i) {
                    items_[i] = new T();
                }
            }

            public T get()
            {
                if(items_.Length <= top_) {
                    expand();
                }
                return items_[top_++];
            }

            public void clear()
            {
                for(int i = 0; i < top_; ++i) {
                    items_[i].Clear();
                }
                top_ = 0;
            }

            private void expand()
            {
                int newSize = items_.Length + Config.ExpandSize;
                T[] items = new T[newSize];
                System.Array.Copy(items_, items, top_);
                for(int i = top_; i < newSize; ++i) {
                    items[i] = new T();
                }
                items_ = items;
            }
        }

        private Cache<Sequence> cacheSequence_ = new Cache<Sequence>(Config.ExpandSize);
        private Cache<Concurrent> cacheConcurrent_ = new Cache<Concurrent>(Config.ExpandSize);
        private CacheDelayed<DelayedConcurrent> cacheDelayedConcurrent_ = new CacheDelayed<DelayedConcurrent>(Config.ExpandSize);

        public void clear()
        {
            cacheSequence_.clear();
            cacheConcurrent_.clear();
            cacheDelayedConcurrent_.clear();
        }

        public Process build(IEnumerator func)
        {
            return new Process(func);
        }

        public Sequence sequence()
        {
            return cacheSequence_.get();
        }

        public Concurrent concurrent()
        {
            return cacheConcurrent_.get();
        }

        public DelayedConcurrent delayedConcurrent()
        {
            return cacheDelayedConcurrent_.get();
        }
    }
}
