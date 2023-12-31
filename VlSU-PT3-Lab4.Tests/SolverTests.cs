using Xunit;

namespace VlSU_PT3_Lab4.Tests;

public class SolverTests
{
    [Fact]
    public void SolveEquation()
    {
        double result = Solver.Solve(x => 2 * x * x - 5, 0, -1);
        Assert.Equal(-Math.Sqrt(2.5), result, 1e-10);
    }

    [Fact]
    public void SolveEquationByBisection()
    {
        Func<double, double> f = x => 0.5*x*x*x - 6*x + 2;

        // Функция имеет три корня в пределах отрезка [-4; 4].
        // Сканируем четыре подотрезка длиной 2 каждый.

        // Эталонные значения получены через библиотеку scipy
        // для Python (функция scipy.optimize.fsolve)
        Assert.Equal(-3.6200758584679065, Solver.SolveBisect(f, -4, -2), 1e-10);
        Assert.Equal(Double.NaN, Solver.SolveBisect(f, -2, 0), 1e-10);
        Assert.Equal(0.3365088035620548, Solver.SolveBisect(f, 0, 2), 1e-10);
        Assert.Equal(3.2835670549058515, Solver.SolveBisect(f, 2, 4), 1e-10);
    }

    [Fact]
    public void SolveEquationByHybridMethod()
    {
        // f(x) = sin^2 3x - cos x + 0.8
        Func<double, double> f = x =>
        {
            double sine = Math.Sin(3 * x);
            return sine * sine + Math.Cos(x) - 0.8;
        };

        Assert.Equal(Double.NaN, Solver.SolveHybrid(f, -3, -2), 1e-10);
        Assert.Equal(-1.6755088421230933, Solver.SolveHybrid(f, -2, -1.5), 1e-10);
        Assert.Equal(-1.3293426325582576, Solver.SolveHybrid(f, -1, -1.5), 1e-10);
        Assert.Equal(-0.9013779322224053, Solver.SolveHybrid(f, -1, -0.5), 1e-10);
        Assert.Equal(Double.NaN, Solver.SolveHybrid(f, -0.5, 0.5), 1e-10);
        Assert.Equal(0.9013779322224053, Solver.SolveHybrid(f, 0.5, 1), 1e-10);
        Assert.Equal(1.3293426325582576, Solver.SolveHybrid(f, 1, 1.5), 1e-10);
        Assert.Equal(1.6755088421230933, Solver.SolveHybrid(f, 1.5, 2), 1e-10);
        Assert.Equal(Double.NaN, Solver.SolveHybrid(f, 2, 3), 1e-10);
    }

    [Fact]
    public void SolveEquationByIllinois()
    {
        Func<double, double> f = x => Double.Exp(0.2 * x) - 3 * Double.Cos(x) - 2;

        Assert.Equal(-4.16353504737036, Solver.SolveIllinois(f, -6, -4), 1e-10);
        Assert.Equal(-2.03156034564077, Solver.SolveIllinois(f, -4, -2), 1e-10);
        Assert.Equal(Double.NaN, Solver.SolveIllinois(f, -2, 0), 1e-10);
        Assert.Equal(1.7642916549816123, Solver.SolveIllinois(f, 0, 2), 1e-10);
        Assert.Equal(Double.NaN, Solver.SolveIllinois(f, 2, 4), 1e-10);
        Assert.Equal(4.943735333787863, Solver.SolveIllinois(f, 4, 6), 1e-10);
    }

    [Fact]
    public void SolveEquationByITP()
    {
        double eps = 1e-10;
        Func<double, double> f = x =>
        {
            double sine = Double.Sin(x);
            return Double.Log2(sine * sine - Double.Cos(x * 3) + 0.5);
        };

        Assert.Equal(0.5880791665730997, Solver.SolveITP(f, 0.4, 1, 0.2, 2, 0, eps), eps);
        Assert.Equal(1.7351182001678722, Solver.SolveITP(f, 1, 2, 0.2, 2, 0, eps), eps);
        Assert.Equal(2.734875484633121, Solver.SolveITP(f, 2, 3, 0.2, 2, 0, eps), eps);
        Assert.Equal(3.548309822546464, Solver.SolveITP(f, 3, 4, 0.2, 2, 0, eps), eps);
        Assert.Equal(4.548067107011714, Solver.SolveITP(f, 4, 5, 0.2, 2, 0, eps), eps);
        Assert.Equal(5.695106140606486, Solver.SolveITP(f, 5, 5.9, 0.2, 2, 0, eps), eps);
    }
}