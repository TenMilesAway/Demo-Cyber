using System;
using System.Collections.Generic;

namespace MvvmTest
{
    /// <summary>
    /// 用于 MVVM 的数据类
    /// </summary>
    public sealed class ReactiveProperty<T>
    {
        private T value;
        private readonly List<Action<T>> subscribers = new List<Action<T>>();

        public ReactiveProperty(T initialValue = default)
        {
            value = initialValue;
        }

        public T Value
        {
            get => value;
            set
            {
                this.value = value;

                // 值更新时, 订阅者需要执行
                var snapshot = subscribers.ToArray();
                for (var i = 0; i < snapshot.Length; i++)
                {
                    snapshot[i](this.value);
                }
            }
        }

        public IDisposable Subscribe(Action<T> onNext)
        {
            subscribers.Add(onNext);
            onNext(value);

            // 注册 Dispose 回调
            return new Subscription(() => subscribers.Remove(onNext));
        }

        private sealed class Subscription : IDisposable
        {
            private readonly Action disposeAction;

            public Subscription(Action disposeAction)
            {
                this.disposeAction = disposeAction;
            }

            public void Dispose()
            {
                disposeAction();
            }
        }
    }

    public sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public void Add(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposables.Add(disposable);
            }
        }

        public void Dispose()
        {
            for (var i = 0; i < disposables.Count; i++)
            {
                disposables[i].Dispose();
            }

            disposables.Clear();
        }
    }
}
