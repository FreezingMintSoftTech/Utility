/**
@file DelayedConcurrent.cs
@author t-sakai
@date 2016/02/19
*/
using System.Collections;

namespace Flow
{
    public class DelayedConcurrent : IEnumerator
    {
        private struct Func
        {
            public float time_;
            public float delay_;
            public IEnumerator func_;
        }

        private int size_;
        private Func[] funcs_;

        public DelayedConcurrent()
        {
            size_ = 0;
            funcs_ = new Func[0];
            TimeScale = Config.InitialTimeScale;
        }

        public DelayedConcurrent(int capacity)
        {
            //System.Diagnostics.Debug.Assert(0<=capacity);
            size_ = 0;
            funcs_ = new Func[capacity];
            TimeScale = Config.InitialTimeScale;
        }

        public int Size
        {
            get{ return size_;}
        }

        public IEnumerator this[int index]
        {
            get
            {
                //System.Diagnostics.Debug.Assert(0<=index && index<size_);
                return funcs_[index].func_;
            }
        }

        public float TimeScale
        {
            get;
            set;
        }

        public void Clear()
        {
            System.Array.Clear(funcs_, 0, size_);
            size_ = 0;
        }

        public void Add(IEnumerator func, float delay=0.0f)
        {
            //System.Diagnostics.Debug.Assert(null != func);
            if(funcs_.Length<=size_){
                Expand();
            }
            funcs_[size_].time_ = 0.0f;
            funcs_[size_].delay_ = delay;
            funcs_[size_].func_ = func;
            ++size_;
        }


        protected void Expand()
        {
            Func[] funcs = new Func[funcs_.Length+Config.ExpandSize];
            System.Array.Copy(funcs_, funcs, funcs_.Length);
            funcs_ = funcs;
        }

        public void Dispose()
        {
        }

        public object GetCurrent(int index)
        {
            return (0<=index&&index<size_)? funcs_[index].func_.Current : null;
        }

        public object Current
        {
            get{ return null; }
        }

        public bool MoveNext()
        {
            bool result = false;
            for(int i=0; i<size_; ++i) {
                if(funcs_[i].time_<funcs_[i].delay_) {
                    funcs_[i].time_ += TimeScale * UnityEngine.Time.deltaTime;
                    return true;
                }
                result |= funcs_[i].func_.MoveNext();
            }
            return result;
        }

        public void Reset()
        {
            for(int i=0; i<size_; ++i){
                funcs_[i].time_ = 0.0f;
                funcs_[i].func_.Reset();
            }
        }
    }
}
