using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace TreePusherLib.Utils;

/// <summary>
/// Identical to Subject, but fires the events in reverse order of the subscriptions
/// </summary>
sealed class PopSubject<T> : SubjectBase<T>
{
    private SubjectDisposable[] observers;
    private Exception? exception;
    // ReSharper disable UseArrayEmptyMethod
    private static readonly SubjectDisposable[] terminated = new SubjectDisposable[0];
    private static readonly SubjectDisposable[] disposed = new SubjectDisposable[0];
    // ReSharper restore UseArrayEmptyMethod

    public PopSubject() => observers = Array.Empty<SubjectDisposable>();

    public override bool HasObservers => Volatile.Read(ref observers).Length != 0;
    public override bool IsDisposed => Volatile.Read(ref observers) == disposed;

    private static void ThrowDisposed() => throw new ObjectDisposedException(string.Empty);

    
    // ****************
    // * IObserver<T> *
    // ****************
    public override void OnCompleted()
    {
        for (; ; )
        {
            var observersList = Volatile.Read(ref observers);
            if (observersList == disposed)
            {
                exception = null;
                ThrowDisposed();
                break;
            }
            if (observersList == terminated)
                break;

            if (Interlocked.CompareExchange(ref observers, terminated, observersList) == observersList)
            {
                foreach (var observer in observersList/*.Reverse()*/)
                    observer.Observer?.OnCompleted();
                break;
            }
        }
    }

    public override void OnError(Exception error)
    {
        if (error == null) throw new ArgumentNullException(nameof(error));

        for (; ; )
        {
            var observerList = Volatile.Read(ref observers);
            if (observerList == disposed)
            {
                exception = null;
                ThrowDisposed();
                break;
            }
            if (observerList == terminated) break;
            exception = error;
            if (Interlocked.CompareExchange(ref observers, terminated, observerList) == observerList)
            {
                foreach (var observer in observerList/*.Reverse()*/)
                    observer.Observer?.OnError(error);
                break;
            }
        }
    }

    public override void OnNext(T value)
    {
        var observerList = Volatile.Read(ref observers);
        if (observerList == disposed)
        {
            exception = null;
            ThrowDisposed();
            return;
        }
        foreach (var observer in observerList/*.Reverse()*/)
            observer.Observer?.OnNext(value);
    }

    
    // ******************
    // * IObservable<T> *
    // ******************
    public override IDisposable Subscribe(IObserver<T> observer)
    {
        if (observer == null) throw new ArgumentNullException(nameof(observer));

        var disposable = default(SubjectDisposable);
        for (; ; )
        {
            var observerList = Volatile.Read(ref observers);
            if (observerList == disposed)
            {
                exception = null;
                ThrowDisposed();
                break;
            }
            if (observerList == terminated)
            {
                var ex = exception;
                if (ex != null)
                    observer.OnError(ex);
                else
                    observer.OnCompleted();
                break;
            }

            disposable ??= new SubjectDisposable(this, observer);
            var n = observerList.Length;
            var b = new SubjectDisposable[n + 1];
            Array.Copy(observerList, 0, b, 0, n);
            b[n] = disposable;
            if (Interlocked.CompareExchange(ref observers, b, observerList) == observerList)
                return disposable;
        }

        return Disposable.Empty;
    }

    private void Unsubscribe(SubjectDisposable observer)
    {
        for (; ; )
        {
            var a = Volatile.Read(ref observers);
            var n = a.Length;
            if (n == 0)
                break;

            var j = Array.IndexOf(a, observer);

            if (j < 0)
                break;

            SubjectDisposable[] b;

            if (n == 1)
            {
                b = Array.Empty<SubjectDisposable>();
            }
            else
            {
                b = new SubjectDisposable[n - 1];

                Array.Copy(a, 0, b, 0, j);
                Array.Copy(a, j + 1, b, j, n - j - 1);
            }

            if (Interlocked.CompareExchange(ref observers, b, a) == a)
                break;
        }
    }
    

    private sealed class SubjectDisposable : IDisposable
    {
        private PopSubject<T> subject;
        private volatile IObserver<T>? observer;

        public SubjectDisposable(PopSubject<T> subject, IObserver<T> observer)
        {
            this.subject = subject;
            this.observer = observer;
        }

        public IObserver<T>? Observer => observer;

        public void Dispose()
        {
            var observerRead = Interlocked.Exchange(ref observer, null);
            if (observerRead == null) return;
            subject.Unsubscribe(this);
            subject = null!;
        }
    }


    // ***************
    // * IDisposable *
    // ***************
    public override void Dispose()
    {
        Interlocked.Exchange(ref observers, disposed);
        exception = null;
    }
}
