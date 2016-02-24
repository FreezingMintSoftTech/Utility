/**
@file EnumeratorSet.cs
@author t-sakai
@date 2016/02/23
*/
using System.Collections;

namespace Flow
{
    public abstract class EnumeratorSet : IEnumerator
    {
        protected int size_;
        protected IEnumerator[] funcs_;

        public EnumeratorSet()
        {
            size_ = 0;
            funcs_ = new IEnumerator[0];
        }

        public EnumeratorSet(int capacity)
        {
            //System.Diagnostics.Debug.Assert(0<=capacity);
            size_ = 0;
            funcs_ = new IEnumerator[capacity];
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
                return funcs_[index];
            }
        }

        public virtual void Clear()
        {
            System.Array.Clear(funcs_, 0, size_);
            size_ = 0;
        }

        public void Add(IEnumerator func)
        {
            //System.Diagnostics.Debug.Assert(null != func);
            if(funcs_.Length<=size_){
                Expand();
            }
            funcs_[size_] = func;
            ++size_;
        }


        protected void Expand()
        {
            IEnumerator[] funcs = new IEnumerator[funcs_.Length+Config.ExpandSize];
            System.Array.Copy(funcs_, funcs, funcs_.Length);
            funcs_ = funcs;
        }

        public void Dispose()
        {
        }

        public virtual object Current
        {
            get { return null; }
        }

        public virtual bool MoveNext()
        {
            return false;
        }

        public virtual void Reset()
        {
        }
    }
}
