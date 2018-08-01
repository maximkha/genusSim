using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace genusSim
{
    public struct Statistic
    {
        public Stopwatch timer;
        public int calls;
        public long totalTime;
    }

    public static class Profiler
    {
        public static Dictionary<string, Statistic> calls = new Dictionary<string, Statistic>();

        public static void start([CallerMemberName] string memberName = "")
        {
            if (!calls.ContainsKey(memberName))
            {
                Statistic stat = new Statistic();
                stat.timer = new Stopwatch();
                calls.Add(memberName, stat);
            }

            Statistic st = calls[memberName];
            st.timer.Start();
            calls[memberName] = st;
        }

        public static void stop([CallerMemberName] string memberName = "")
        {
            if (!calls.ContainsKey(memberName))
            {
                throw new Exception("No function registered " + memberName);
            }

            Statistic st = calls[memberName];

            st.timer.Stop();
            st.totalTime += calls[memberName].timer.ElapsedMilliseconds;
            st.timer.Reset();
            st.calls++;

            calls[memberName] = st;
        }

        public static void log()
        {
            if (calls.Count == 0) Console.WriteLine("No functions were tracked.");
            string[] keys = calls.Keys.ToArray();
            //Array.Sort(keys, (x, y) => string.Compare(x, y));
            Array.Sort(keys, (x, y) => ((double)calls[y].totalTime / (double)calls[y].calls).CompareTo((double)calls[x].totalTime / (double)calls[x].calls));
            int mKeyLen = keys.Max((x) => x.Length);// + 1;
            for (int i = 0; i < keys.Length; i++)
            {
                Statistic st = calls[keys[i]];
                Console.WriteLine("{0} | ASpd: {1} MS/CL, TtlT: {2} MS, Cls: {3}", keys[i] + new string(' ', mKeyLen - keys[i].Length), String.Format("{0:0.##}", ((double)st.totalTime / (double)st.calls)), st.totalTime, st.calls);
            }
        }
    }
}
