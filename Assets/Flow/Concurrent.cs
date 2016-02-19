/**
@file Concurrent.cs
@author t-sakai
@date 2016/02/19
*/
using System.Collections;

namespace Flow
{
    public class Concurrent : FunctionSet
    {
        public Concurrent()
        {
        }

        public Concurrent(int capacity)
            :base(capacity)
        {
        }

        public override void Clear()
        {
            base.Clear();
        }

        public object GetCurrent(int index)
        {
            return (0<=index&&index<size_)? funcs_[index].Current : null;
        }

        public override object Current
        {
            get{ return null; }
        }

        public override bool MoveNext()
        {
            bool result = false;
            for(int i=0; i<size_; ++i) {
                result |= funcs_[i].MoveNext();
            }
            return result;
        }

        public override void Reset()
        {
            for(int i=0; i<size_; ++i){
                funcs_[i].Reset();
            }
        }
    }
}
