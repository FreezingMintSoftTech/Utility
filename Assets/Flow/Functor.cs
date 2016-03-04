/**
@file Functor.cs
@author t-sakai
@date 2016/02/23
*/

namespace Flow
{
    public struct ToFunctor0
    {
        public delegate void Func();

        public static Functor Do(Func func)
        {
            return new Functor(func, null);
        }
    };

    public struct ToFunctor1<T>
    {
        public delegate void Func(T t);

        public static Functor Do(Func func, params System.Object[] args)
        {
            return new Functor(func, args);
        }
    };

    public struct ToFunctor2<T0, T1>
    {
        public delegate void Func(T0 t0, T1 t1);

        public static Functor Do(Func func, params System.Object[] args)
        {
            return new Functor(func, args);
        }
    };

    public struct Functor : System.Collections.IEnumerator
    {
        private bool done_;
        private System.Delegate func_;
        private System.Object[] args_;

        public Functor(System.Delegate func, params System.Object[] args)
        {
            func_ = func;
            args_ = args;
            done_ = (null == func_);
        }

        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            if(!done_) {
                func_.DynamicInvoke(args_);
                done_ = true;
            }
            return false;
        }

        public void Reset()
        {
            done_ = false;
        }
    };
}
