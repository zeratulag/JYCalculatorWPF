using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using System;

namespace JX3CalculatorShared.Utils
{
    public static class MinimizationTool

    {
        /// <summary>
        /// Find vector x that minimizes the function f(x), constrained within bounds, using the Broyden–Fletcher–Goldfarb–Shanno Bounded (BFGS-B) algorithm.
        /// The missing gradient is evaluated numerically (forward difference).
        /// For more options and diagnostics consider to use <see cref="BfgsBMinimizer"/> directly.
        /// </summary>
        public static MinimizationResult OfFunctionConstrained(Func<Vector<double>, double> function, Vector<double> lowerBound,
            Vector<double> upperBound, Vector<double> initialGuess, double gradientTolerance = 1e-5,
            double parameterTolerance = 1e-5, double functionProgressTolerance = 1e-5, int maxIterations = 1000)
        {
            var objective = ObjectiveFunction.Value(function);
            var objectiveWithGradient =
                new MathNet.Numerics.Optimization.ObjectiveFunctions.ForwardDifferenceGradientObjectiveFunction(objective,
                    lowerBound, upperBound);
            var algorithm = new BfgsBMinimizer(gradientTolerance, parameterTolerance, functionProgressTolerance,
                maxIterations);
            var result = algorithm.FindMinimum(objectiveWithGradient, lowerBound, upperBound, initialGuess);
            return result;
        }
    }
}