/**
@file Sequence.cs
@author t-sakai
@date 2016/02/19
*/
using System.Collections;

namespace Flow
{
    public class Sequence : FunctionSet
    {
        private int current_;

        public Sequence()
        {
        }

        public Sequence(int capacity)
            :base(capacity)
        {
        }

        public override void Clear()
        {
            base.Clear();
            current_ = 0;
        }

        public override object Current
        {
            get { return (current_<size_)? funcs_[current_].Current : null; }
        }

        public override bool MoveNext()
        {
            if(size_<=current_) {
                return false;
            }
            if(funcs_[current_].MoveNext()) {
                return true;
            }
            ++current_;
            return current_ < size_;
        }

        public override void Reset()
        {
            current_ = 0;
            for(int i=0; i<size_; ++i){
                funcs_[i].Reset();
            }
        }
    }
}
