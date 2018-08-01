using System;

namespace genusSim
{
    class MainClass
    {
        //public static double[] toFit = { 0, 1, 2, 3, 5 }; //Linear
        public static double[] toFit = { 1, 2, 5, 10, 17 }; //Polynomial

        public static void Main(string[] args)
        {
            //1st p[0] = yinter, p[1] = degree
            genus.Simulator simulator = genus.Optimizer.getOptimizer(new double[] { -2, 0 }, new double[] { 2, 4 }, 2, .5, 1000, sqrOptimize); //Polynomial

            //1st p[0] = yinter, p[1] = slope
            //genus.Simulator simulator = genus.Optimizer.getOptimizer(new double[] { -2, -2 }, new double[] { 2, 2 }, 2, .05, 1000, linOptimize); //Linear

            simulator.genLogEvery = 5;
            //simulator.genLogEvery = 0;

            //simulator.fileLogEvery = 1;
            simulator.fileLogEvery = 0;
            simulator.logFile = "genetic.txt";

            simulator.mutationRate = 0.8;
            simulator.lowerClassDeathRate = 0;//0.1;
            simulator.killOver = 1;

            simulator.debug = true;
            //genus.Optimizer.valueCanidate solution = (genus.Optimizer.valueCanidate)simulator.untilMax((x, y) => (y > 100000 || x.getFitness() > .83), 1);
            genus.Optimizer.valueCanidate solution = (genus.Optimizer.valueCanidate)simulator.untilMax(50000);
            Profiler.start("Solve");
            //genus.Optimizer.valueCanidate solution = (genus.Optimizer.valueCanidate)simulator.untilMax(2500);
            Profiler.stop("Solve");
            Console.WriteLine("SOLVED: {0}, {1}",solution.myVal.toString(), solution.getFitness());
            using (System.IO.FileStream fs = new System.IO.FileStream("solution.txt", System.IO.FileMode.OpenOrCreate))
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(solution.myVal[0] + " " + solution.myVal[1]);
                fs.Write(data, 0, data.Length);
            }

            if (simulator.debug) Profiler.log();
        }

        public static double linOptimize(double[] p)
        {
            double score = 0;
            for (int i = 0; i < toFit.Length; i++)
            {
                 score += Math.Pow(toFit[i] - (p[0] + p[1] * i), 2);
            }
            return 1 - (score / toFit.Length);
        }

        public static double sqrOptimize(double[] p)
        {
            //Profiler.start();
            double score = 0;
            for (int i = 0; i < toFit.Length; i++)
            {
                score += Math.Pow(toFit[i] - (p[0] + Math.Pow(i, p[1])), 2);
            }
            //Profiler.stop();
            return 1 - (score / toFit.Length);
        }
    }
}
