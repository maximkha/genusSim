using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace genusSim
{
    public static class genus
    {
        public interface ICandidate// : IDisposable
        {
            ICandidate mate(ICandidate other);
            ICandidate clone();
            double getFitness();
            string getDescription();
            string getGenome();
            void mutate();
        }

        public class Simulator
        {
            public List<ICandidate> population = new List<ICandidate>();
            public double topPercentage = 0.25;
            public double lowerClassDeathRate = 0.125;
            public double mutationRate = 0.375;
            public double killOver = 0.25;
            //private List<Tuple<ICandidate, double>> generationLeaderBoard = new List<Tuple<ICandidate, double>>();
            private SortedDictionary<double, ICandidate> generationLeaderBoard = new SortedDictionary<double, ICandidate>();
            private int genCount = 0;
            private int beginingPop = 0;
            private Random random = new Random();
            private FileStream fileStream = null;


            //public bool logToFile = false;
            public string logFile = "";
            public int fileLogEvery = 0;

            public int genLogEvery = 0;
            //public bool log = false;

            public bool debug = true;

            public Simulator(int pop)
            {
                beginingPop = pop;
                population = new List<ICandidate>(pop);
            }

            public ICandidate until(Func<ICandidate, bool> predicateStop)
            {
                generation();
                if (debug) while (!predicateStop(generationLeaderBoard.Last().Value)) generationDebug();
                else while (!predicateStop(generationLeaderBoard.Last().Value)) generation();
                return generationLeaderBoard.Last().Value;
                //throw new NotImplementedException();
            }

            public ICandidate until(int genCap)
            {
                generation();
                if (debug) while (genCap > genCount) generationDebug();
                else while (genCap > genCount) generation();
                return generationLeaderBoard.Last().Value;
                //throw new NotImplementedException();
            }

            public ICandidate until(Func<ICandidate, int, bool> predicateStop)
            {
                generation();
                if (debug) while (!predicateStop(generationLeaderBoard.Last().Value, genCount)) generationDebug();
                else while (!predicateStop(generationLeaderBoard.Last().Value, genCount)) generation();
                return generationLeaderBoard.Last().Value;
                //throw new NotImplementedException();
            }

            public ICandidate untilMax(Func<ICandidate, bool> predicateStop)
            {
                Tuple<ICandidate, double> best = null;

                if (debug)
                {
                    generationDebug();

                    best = new Tuple<ICandidate, double>(generationLeaderBoard.Last().Value.clone(), generationLeaderBoard.Last().Key);

                    while (!predicateStop(generationLeaderBoard.Last().Value))
                    {
                        generationDebug();

                        KeyValuePair<double, ICandidate> original = generationLeaderBoard.Last();
                        Tuple<ICandidate, double> compare = new Tuple<ICandidate, double>(original.Value.clone(), original.Key);
                        if (best.Item2 < compare.Item2)
                        {
                            best = compare;
                        }
                    }
                }
                else
                {
                    generation();

                    while (!predicateStop(generationLeaderBoard.Last().Value))
                    {
                        generation();

                        KeyValuePair<double, ICandidate> original = generationLeaderBoard.Last();
                        Tuple<ICandidate, double> compare = new Tuple<ICandidate, double>(original.Value.clone(), original.Key);
                        if (best.Item2 < compare.Item2)
                        {
                            best = compare;
                        }
                    }
                }

                return best.Item1;
                //throw new NotImplementedException();
            }

            public ICandidate untilMax(Func<ICandidate, int, bool> predicateStop)
            {
                Tuple<ICandidate, double> best = null;

                if (debug)
                {
                    generationDebug();

                    best = new Tuple<ICandidate, double>(generationLeaderBoard.Last().Value.clone(), generationLeaderBoard.Last().Key);

                    while (!predicateStop(generationLeaderBoard.Last().Value, genCount))
                    {
                        generationDebug();

                        KeyValuePair<double, ICandidate> original = generationLeaderBoard.Last();
                        Tuple<ICandidate, double> compare = new Tuple<ICandidate, double>(original.Value.clone(), original.Key);
                        if (best.Item2 < compare.Item2)
                        {
                            best = compare;
                        }
                    }
                }
                else
                {
                    generation();

                    best = new Tuple<ICandidate, double>(generationLeaderBoard.Last().Value.clone(), generationLeaderBoard.Last().Key);

                    while (!predicateStop(generationLeaderBoard.Last().Value, genCount))
                    {
                        generation();

                        KeyValuePair<double, ICandidate> original = generationLeaderBoard.Last();
                        Tuple<ICandidate, double> compare = new Tuple<ICandidate, double>(original.Value.clone(), original.Key);
                        if (best.Item2 < compare.Item2)
                        {
                            best = compare;
                        }
                    }
                }

                return best.Item1;
                //throw new NotImplementedException();
            }

            public ICandidate untilMax(int genCap)
            {
                Tuple<ICandidate, double> best = null;

                if (debug)
                {
                    generationDebug();

                    best = new Tuple<ICandidate, double>(generationLeaderBoard.Last().Value.clone(), generationLeaderBoard.Last().Key);

                    while (genCap > genCount)
                    {

                        generationDebug();

                        KeyValuePair<double, ICandidate> original = generationLeaderBoard.Last();
                        Tuple<ICandidate, double> compare = new Tuple<ICandidate, double>(original.Value.clone(), original.Key);
                        if (best.Item2 < compare.Item2)
                        {
                            best = compare;
                        }
                    }
                }
                else
                {
                    generation();

                    best = new Tuple<ICandidate, double>(generationLeaderBoard.Last().Value.clone(), generationLeaderBoard.Last().Key);

                    while (genCap > genCount)
                    {

                        generation();

                        KeyValuePair<double, ICandidate> original = generationLeaderBoard.Last();
                        Tuple<ICandidate, double> compare = new Tuple<ICandidate, double>(original.Value.clone(), original.Key);
                        if (best.Item2 < compare.Item2)
                        {
                            best = compare;
                        }
                    }
                }

                return best.Item1;
                //throw new NotImplementedException();
            }

            public void generationDebug()
            {
                //throw new NotImplementedException();
                if (debug) Profiler.start("killOff");
                killOff();
                if (debug) Profiler.stop("killOff");
                if (debug) Profiler.start("mate");
                mate();
                if (debug) Profiler.stop("mate");
                if (debug) Profiler.start("mutate");
                mutate();
                if (debug) Profiler.stop("mutate");
                if (debug) Profiler.start("evaluate");
                evaluate();
                if (debug) Profiler.stop("evaluate");
            }

            public void generation()
            {
                killOff();
                mate();
                mutate();
                evaluate();
            }

            public void mutate()
            {
                //throw new NotImplementedException();
                population.ForEach((x) => random.NextBool(mutationRate).Do(() => x.mutate()));
            }

            public void evaluate()
            {
                //throw new NotImplementedException();
                genCount++;

                generationLeaderBoard = new SortedDictionary<double, ICandidate>();
                //List<Tuple<ICandidate, double>> canidateFitness = new List<Tuple<ICandidate, double>>();
                //Profiler.start("Fitness");
                //population.ForEach((x) => canidateFitness.Add(new Tuple<ICandidate, double>(x, x.getFitness())));
                //Profiler.stop("Fitness");
                //Profiler.start("Ordering");
                //canidateFitness = canidateFitness.OrderByDescending((x) => x.Item2).ToList();
                //generationLeaderBoard = population.Select((x) => new Tuple<ICandidate, double>(x, x.getFitness())).OrderByDescending((x)=>x.Item2).ToList();
                //population.ForEach((p) => generationLeaderBoard.addSorted(new Tuple<ICandidate, double>(p, p.getFitness())));
                ICandidate[] aPop = population.ToArray();
                for (int i = 0; i < aPop.Length; i++)
                {
                    ICandidate candidate = aPop[i];
                    generationLeaderBoard.Add(candidate.getFitness(), candidate);
                }
                //Profiler.stop("Ordering");
                //generationLeaderBoard = canidateFitness;

                //Profiler.start("Console IO");
                if (genLogEvery > 0 && genCount % genLogEvery == 0)
                {
                    KeyValuePair<double, ICandidate> topCanidateFitness = generationLeaderBoard.Last();
                    Console.WriteLine("[Genus]: Generation {0} ({1} Canidates) with the top candidate {2} with a score of {3}", genCount, population.Count, topCanidateFitness.Value.getDescription(), topCanidateFitness.Key);
                }
                //Profiler.stop("Console IO");

                //Profiler.start("File IO");
                if (fileLogEvery > 0 && genCount % fileLogEvery == 0)
                {
                    if (fileStream == null)
                    {
                        fileStream = new FileStream(logFile, FileMode.OpenOrCreate);
                    }
                    KeyValuePair<double, ICandidate> topCanidateFitness = generationLeaderBoard.Last();
                    byte[] textdata = System.Text.Encoding.ASCII.GetBytes(topCanidateFitness.Key + " " + topCanidateFitness.Value.getGenome() + "\n");
                    fileStream.Write(textdata, 0, textdata.Length);
                }
                //Profiler.stop("File IO");
            }

            public void killOff()
            {
                //throw new NotImplementedException();
                //Profiler.start("ExtraK");
                if (beginingPop + (beginingPop * killOver) < population.Count)
                {
                    int take = (int)Math.Floor((double)beginingPop + ((double)beginingPop * killOver));
                    population = generationLeaderBoard.Take(take).Select((x)=>x.Value).ToList();//.GetRange(0, take).Select((x)=>x.Item1).ToList();
                }
                //Profiler.stop("ExtraK");

                int topnum = (int)Math.Floor((double)population.Count * topPercentage);

                //Profiler.start("Removing");
                for (int i = topnum; i < population.Count; i++)
                {
                    if (random.NextBool(lowerClassDeathRate))
                    {
                        population.RemoveAt(i);
                    }
                }
                //Profiler.stop("Removing");
            }

            public void mate()
            {
                int topnum = (int)Math.Floor((double)population.Count * topPercentage);
                if (topnum <= 1) throw new Exception("Not enough top canidates");
                List<ICandidate> mates = generationLeaderBoard.Take(topnum).Select((x) => x.Value).ToList();
                if (mates.Count % 2 != 0)
                {
                    if (mates.Count > 1)
                    {
                        //Don't include lone mate
                        mates.Remove(mates.Select((m) => new Tuple<ICandidate, double>(m, m.getFitness())).OrderByDescending((m) => m.Item2).Last().Item1);
                    }
                    else throw new Exception("Not enough mates!");
                }
                mates.Shuffle(random);
                for (int i = 0; i < mates.Count/2; i++)
                {
                    population.Add(mates[i].mate(mates[i + (mates.Count / 2)]));
                    //random.NextBool(.125).Do(() => population.Add(mates[i].mate(mates[i + (mates.Count / 2)])));
                }
                //throw new NotImplementedException();
            }
        }

        public static class Optimizer
        {
            public static Simulator getOptimizer(Info inObj, int popCount, Func<double[], double> func)
            {
                Simulator ret = new Simulator(popCount);
                //ret.population = new List<ICandidate>(popCount);
                for (int i = 0; i < popCount; i++)
                {
                    valueCanidate canidate = new valueCanidate(inObj);
                    canidate.toOptimze = func;
                    ret.population.Add(canidate);
                }
                return ret;
            }

            public static Simulator getOptimizer(double[] mins, double[] maxs, int num, double skewMating, int popCount, Func<double[], double> func)
            {
                Info infoObj = new Info();
                infoObj.minGuesses = mins;
                infoObj.maxGuesses = maxs;
                infoObj.numIn = num;
                infoObj.skewMating = skewMating;
                Simulator sim = new Simulator(popCount);
                //sim.population = new List<ICandidate>(popCount);
                for (int i = 0; i < popCount; i++)
                {
                    valueCanidate canidate = new valueCanidate(infoObj);
                    canidate.toOptimze = func;
                    sim.population.Add(canidate);
                }
                return sim;
            }

            public class Info
            {
                public int numIn = 1;

                public double[] maxGuesses = { 0 };
                public double[] minGuesses = { 0 };

                public double skewMating = 0.125;

                private Random rnd = new Random();

                public double[] getCanidate()
                {
                    double[] ret = new double[numIn];
                    for (int i = 0; i < numIn; i++)
                    {
                        ret[i] = rnd.NextDouble(minGuesses[i], maxGuesses[i]);
                    }
                    return ret;
                }

                public double[] mutate(double[] input)
                {
                    double[] ret = new double[numIn];
                    input.CopyTo(ret, 0);
                    for (int i = 0; i < numIn; i++)
                    {
                        ret[i] = rnd.NextDouble(minGuesses[i], maxGuesses[i]);
                    }
                    return ret;
                }

                public double[] mate(double[] firstMate, double[] secondMate)
                {
                    if (firstMate.Length != secondMate.Length) throw new Exception("Chromosome error!");
                    double[] child = new double[firstMate.Length > secondMate.Length ? firstMate.Length : secondMate.Length];
                    for (int i = 0; i < child.Length; i++)
                    {
                        child[i] = new double[] { firstMate[i], secondMate[i] }.Average() + rnd.NextDouble(-skewMating, skewMating);
                    }
                    return child;
                }
            }

            public class valueCanidate : ICandidate
            {
                public double[] myVal;
                public Func<double[], double> toOptimze;
                public Info obj;

                public valueCanidate(double[] chromosome, Info pobj)
                {
                    //Usually used when cloning or mating
                    myVal = chromosome;
                    obj = pobj;
                }

                public valueCanidate(Info pobj)
                {
                    //used to create a new canidate
                    obj = pobj;
                    myVal = obj.getCanidate();
                }

                public double getFitness()
                {
                    return toOptimze(myVal);
                }

                public ICandidate clone()
                {
                    valueCanidate cloned = new valueCanidate(obj);
                    cloned.toOptimze = toOptimze;
                    cloned.myVal = new double[myVal.Length];
                    myVal.CopyTo(cloned.myVal, 0);
                    return cloned;
                }

                public string getDescription()
                {
                    return myVal.toString();
                }

                public ICandidate mate(ICandidate other)
                {
                    valueCanidate child = new valueCanidate(obj.mate(myVal, ((valueCanidate)other).myVal), obj);
                    child.toOptimze = toOptimze;
                    return child;
                }

                public void mutate()
                {
                    myVal = obj.mutate(myVal);
                }

                public string getGenome()
                {
                    return myVal.join(" ");
                }
            }
        }


        //Extensions
        public static double NextDouble(this Random random, double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public static bool NextBool(this Random random, double probability)
        {
            return random.NextDouble() < probability;
        }

        public static double Map(this double from, double inMin, double inMax, double outMin, double outMax)
        {
            return (from - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public static void Do(this bool b, Action action)
        {
            if (b) action();
        }

        public static string toString<T>(this T[] input)
        {
            string ret = "{";

            for (int i = 0; i < input.Length; i++)
            {
                ret += input[i].ToString() + ",";
            }
            ret = ret.Substring(0, ret.Length - 1);

            return ret + "}";
        }

        public static string join<T>(this T[] input, string seperator)
        {
            string ret = "";

            for (int i = 0; i < input.Length; i++)
            {
                ret += input[i].ToString() + seperator;
            }
            return ret.Substring(0, ret.Length - seperator.Length);
        }

        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
