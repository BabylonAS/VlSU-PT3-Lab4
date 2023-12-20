using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Transactions;

namespace VlSU_PT3_Lab4
{
    public class Solver
    {
        public static double Solve(Func<double, double> f, double x0, double x1)
        {
            double x, fx0 = f(x0), fx1;
            do
            {
                fx1 = f(x1);
                x = x1 - fx1 * (x0 - x1) / (fx0 - fx1);

                x0 = x1; fx0 = fx1;
                x1 = x;
            } while (
                Math.Abs(x0 - x1) > 2 * Double.Epsilon &&
                Math.Abs(x0 / x1 - 1) > 2 * Double.Epsilon &&
                Math.Abs(fx1) > 2 * Double.Epsilon
            );

            return x;
        }

        public static double SolveBisect(Func<double, double> f, double x0, double x1)
        {
            // Удостоверимся, что границы отрезка заданы по возрастанию
            double a = x0, b = x1;
            if (x1 < x0) {
                a = x1;
                b = x0;
            }

            // Проверить случаи, когда одна из границ является корнем
            // функции
            double fa = f(a), fb = f(b);
            if (fa == 0.0)
                return a;
            else if (fb == 0.0)
                return b;

            // Удостоверимся, что функция принимает разные знаки на
            // обоих границах отрезка
            if (Math.Sign(fa) == Math.Sign(fb))
                return Double.NaN;

            // Производим деление отрезков
            double c, fc;
            ushort iterations = 0;
            while (iterations < UInt16.MaxValue)
            {
                // Определяем середину отрезка
                c = a + (b - a) * 0.5;
                fc = f(c);

                if (fc == 0.0 || b - a <= 2 * Double.Epsilon)
                    // Корень найден
                    return c;
                else if (Math.Sign(fa) != Math.Sign(fc))
                    // Корень находится в левом подотрезке
                    b = c;
                else
                {
                    // Корень находится в правом подотрезке
                    a = c;
                    fa = fc;
                }
                iterations++;
            }

            // Выполнено слишком много итераций
            return Double.NaN;
        }

        // Количество шагов бисекции
        private const byte _bisectionSteps = 4;

        public static double SolveHybrid(Func<double, double> f, double x0, double x1)
        {
            // Нормализация границ
            double a = x0, b = x1;
            if (x1 < x0)
            {
                a = x1;
                b = x0;
            }

            // Проверить случаи, когда одна из границ является корнем
            // функции
            double fa = f(a), fb = f(b);
            if (fa == 0.0)
                return a;
            else if (fb == 0.0)
                return b;

            // Удостоверимся, что функция принимает разные знаки на
            // обоих границах отрезка
            if (Math.Sign(fa) == Math.Sign(fb))
                return Double.NaN;

            // Этап 1: находим начальные приближения методом бисекции
            double c = a + (b - a) * 0.5, fc = f(c);
            for (byte i = 1; i < _bisectionSteps; i++)
            {
                if (fc == 0.0 || b - a <= 2 * Double.Epsilon)
                    // Маловероятно, но теоретически возможно
                    return c;
                else if (Math.Sign(fa) != Math.Sign(fc))
                    // Корень находится в левом подотрезке
                    b = c;
                else
                {
                    // Корень находится в правом подотрезке
                    a = c;
                    fa = fc;
                }

                // Запоминаем предыдущую срединную точку в неза-
                // действованной переменной fb (неинтуитивно, но
                // экономит память), после чего задаём новое
                // значение "c"
                fb = c;
                c = a + (b - a) * 0.5;
                fc = f(c);
            }

            // Этап 2: находим окончательное приближение методом
            // секущих
            // Сохраняем абсолютные границы локализации, чтобы
            // находить случай расходимости.
            x0 = a; x1 = b;

            // Здесь "a" будет первым из приближений, "c" будет
            // вторым. Если корень находится между "a" и "c", то
            // вместо "a" будет использоваться "b".
            if (fb == a)
            {
                a = b;
                fa = f(a);
            }

            // "b" будет искомым корнем
            do
            {
                b = c - fc * (a - c) / (fa - fc);
                if (b <= x0 || b >= x1)
                    // Метод секущих расходится, возвращаем NaN
                    return Double.NaN;

                a = c; fa = fc;
                c = b; fc = f(c);
            } while (Math.Abs(a - c) > 2 * Double.Epsilon &&
                Math.Abs(a / c - 1) > 2 * Double.Epsilon &&
                Math.Abs(fc) > 2 * Double.Epsilon);

            return b;
        }

        public static double SolveIllinois(Func<double, double> f, double x0, double x1)
        {
            // Нормализация границ
            double a = x0, b = x1;
            if (x1 < x0)
            {
                a = x1;
                b = x0;
            }

            // Проверить случаи, когда одна из границ является корнем
            // функции
            double fa = f(a), fb = f(b);
            if (fa == 0.0)
                return a;
            else if (fb == 0.0)
                return b;

            // Удостоверимся, что функция принимает разные знаки на
            // обоих границах отрезка
            if (Math.Sign(fa) == Math.Sign(fb))
                return Double.NaN;

            // Производим деление отрезков
            double c = 0, fc;
            sbyte side = 0;
            for (ushort i = 0; i < UInt16.MaxValue &&
                Math.Abs(b - a) > Double.Epsilon * Math.Abs(b + a);
                i++)
            {
                // Здесь "c" находится путём строительства секущей,
                // соединяющих точки (a, fa) и (b, fb). В точке "c"
                // секущая пересекает ось x.

                // Особенность иллинойсского метода в том, что если
                // значение fa или fb два раза подряд имеют один и тот
                // же знак, что и fc, то вместо результата функции в
                // другой граничной точке (соответственно fb или fa)
                // подставляется половина этого результата. Тем самым
                // на следующей итерации fc будет иметь общий знак с
                // этим другим результатом.
                c = (fa * b - fb * a) / (fa - fb);
                fc = f(c);

                if (Math.Sign(fc) == Math.Sign(fb))
                {
                    // Корень в левом подотрезке
                    b = c; fb = fc;
                    if (side == -1)
                        fa *= 0.5;
                    side = -1;
                }
                else if (Math.Sign(fc) == Math.Sign(fa))
                {
                    // Корень в правом подотрезке
                    a = c; fa = fc;
                    if (side == 1)
                        fb *= 0.5;
                    side = 1;
                }
                else
                    break;
            }

            return c;
        }

        public static double SolveITP(Func<double, double> f, double x0, double x1, double k1, double k2, int n0, double epsilon)
        {
            // Нормализация границ
            double a = x0, b = x1;
            if (x1 < x0)
            {
                a = x1;
                b = x0;
            }

            // Оценка количества итераций, которые потребовались бы
            // методу бисекции для нахождения корня
            int n1_2 = (int) Math.Ceiling(Math.Log2((b - a) * 0.5 / epsilon));
            int nmax = n1_2 + n0;

            ulong epsilon_modifier = 1UL << nmax;
            double fa = f(a), fb = f(b);
            double fitp, x1_2, xf, xt, xitp, r, delta, sigma;
            while (b - a > 2 * epsilon)
            {
                // Интерполяция
                xf = (fb * a - fa * b) / (fb - fa);

                // Отсечение
                x1_2 = (a + b) * 0.5;
                sigma = x1_2 - xf;
                delta = k1 * Math.Pow(b - a, k2);
                if (delta <= Math.Abs(sigma))
                    xt = xf + Math.CopySign(delta, sigma);
                else
                    xt = x1_2;

                // Проекция
                r = epsilon * epsilon_modifier - 0.5 * (b - a);
                if (Math.Abs(xt - x1_2) <= r)
                    xitp = xt;
                else
                    xitp = x1_2 - Math.CopySign(r, sigma);

                // Смещение границ интервала
                fitp = f(xitp);
                if (Math.Sign(fitp) == Math.Sign(fb))
                {
                    b = xitp;
                    fb = fitp;
                }
                else if (Math.Sign(fitp) == Math.Sign(fa))
                {
                    a = xitp;
                    fa = fitp;
                }
                else
                    return xitp;

                epsilon_modifier >>= 1;
            }

            return (a + b) * 0.5;
        }
    }
}