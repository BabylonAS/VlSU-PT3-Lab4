namespace VlSU_PT3_Lab4
{
    public class Solver
    {
        public static double Solve(Func<double,double> f, double x0, double x1)
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

        public static double SolveBisect(Func<double,double> f, double x0, double x1)
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

        public static double SolveHybrid(Func<double,double> f, double x0, double x1)
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
    }
}