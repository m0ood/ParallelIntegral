using System;
using System.Threading;

namespace ParallelIntegral
{
    public class ParalIntegral
    {
        private readonly Func<double, double> _y;

        private readonly double _a;
        private readonly double _b;
        private readonly double _h;

        private readonly Semaphore _semaphore;

        private double _result;

        public double Result
        {
            get { return _result; }
        }

        private volatile uint _started;
        private volatile uint _finished;

        public ParalIntegral(Func<double, double> y, double a, double b, double h)
        {
            _y = y;
            _semaphore = new Semaphore(Environment.ProcessorCount, Environment.ProcessorCount);
            _a = a;
            _b = b;
            _h = h;
        }

        public bool Finish => _started == _finished;

        public void Parallel()
        {
            var thread = new Thread(ParalIntegraloutine);
            Interlocked.Increment(ref _started);
            thread.Start(new object[] {_a, _b});
        }

        public double solo()
        {
            double res = 0;
            for (double a = _a; a < _b; a += _h)
            {
                double b = a + _h;
                double center = a + (b - a) / 2;
                res += _y(center) * _h;
            }

            return res;
        }

        private void ParalIntegraloutine(object args)
        {
            _semaphore.WaitOne();
            var a = (double)((object[])args)[0];
            var b = (double)((object[])args)[1];
            var distance = b - a;

            var half = distance / 2;
            var center = a + half;

            if (distance < _h)
            {
                _result += (_y(a) + _y(b)) / 2 * distance;
            }
            else
            {
                if (_started < Environment.ProcessorCount)
                {
                    Interlocked.Add(ref _started, 2);
                    var threada = new Thread(ParalIntegraloutine);
                    threada.Start(new object[] {a, center});

                    var threadb = new Thread(ParalIntegraloutine);
                    threadb.Start(new object[] {center, b});
                }
                else
                {
                    double res = 0;
                    for (double l = a; l < b; l += _h)
                    {
                        double r = l + _h;
                        double c = l + (r - l) / 2;
                        res += _y(c) * _h;
                    }

                    _result += res;
                }
            }

            Interlocked.Increment(ref _finished);
            _semaphore.Release();
        }
    }
}