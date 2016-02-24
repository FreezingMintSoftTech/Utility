/**
@file Functor.cs
@author t-sakai
@date 2016/02/23
*/

namespace Flow
{
    public struct ToDelegate0
    {
        public delegate void Func();
        public static System.Delegate Do(Func func)
        {
            return func;
        }
    };

    public struct ToDelegate1<T>
    {
        public delegate void Func(T t);
        public static System.Delegate Do(Func func)
        {
            return func;
        }
    };

    public struct Functor : System.Collections.IEnumerator
    {
        private System.Delegate func_;
        private System.Object[] args_;

        public Functor(System.Delegate func, params System.Object[] args)
        {
            func_ = func;
            args_ = args;
        }

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            func_.DynamicInvoke(args_);
            return false;
        }

        public void Reset()
        {
        }
    };
}
