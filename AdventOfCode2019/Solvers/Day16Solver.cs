using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solvers
{
    class Day16Solver : AbstractSolver
    {
        public override string Part1()
        {
            List<int> signal;
            var basePattern = new List<int> { 0, 1, 0, -1 };

            using (var input = File.OpenText(InputFile))
            {
                signal = input.ReadLine().ToCharArray().Select(c => c - 48).ToList();
            }

            for (var phase = 0; phase < 100; phase++)
            {
                var processedSignal = new List<int>();
                for (var i = 0; i < signal.Count; i++)
                {
                    var pattern = new List<int>();
                    var bpEnum = basePattern.GetEnumerator();
                    bpEnum.MoveNext();
                    while (pattern.Count <= signal.Count) // we need one extra because the first element is removed shortly
                    {
                        for (var j = 0; j < i + 1; j++)
                        {
                            pattern.Add(bpEnum.Current);
                            if (pattern.Count > signal.Count) break;
                        }
                        if (!bpEnum.MoveNext())
                        {
                            bpEnum = basePattern.GetEnumerator();
                            bpEnum.MoveNext();
                        }
                    }
                    pattern.RemoveAt(0);

                    processedSignal.Insert(i, 0);
                    for (var n = 0; n < signal.Count; n++)
                    {
                        processedSignal[i] += (signal[n] * pattern[n]);
                    }
                    processedSignal[i] %= 10;
                    if (processedSignal[i] < 0) processedSignal[i] *= -1;
                }
                signal = processedSignal;
            }

            var sb = new StringBuilder();
            for (var i = 0; i < 8; i++) sb.Append(signal[i]);
            return sb.ToString();
        }

        public override string Part2()
        {
            throw new NotImplementedException();
            
            List<int> signal = new List<int>();
            var basePattern = new List<int> { 0, 1, 0, -1 };

            using (var input = File.OpenText(InputFile))
            {
                var baseSignal = input.ReadLine().ToCharArray().Select(c => c - 48).ToList();
                for (var i = 0; i < 10000; i++) baseSignal.ForEach(s => signal.Add(s));
            }

            var offsetSb = new StringBuilder();
            for (var i = 0; i < 7; i++) offsetSb.Append(signal[i]);
            var resultOffset = Convert.ToInt32(offsetSb.ToString());



            for (var phase = 0; phase < 100; phase++)
            {
                var processedSignal = new List<int>();
                for (var i = 0; i < signal.Count; i++)
                {
                    var pattern = new List<int>();
                    var bpEnum = basePattern.GetEnumerator();
                    bpEnum.MoveNext();
                    while (pattern.Count <= signal.Count) // we need one extra because the first element is removed shortly
                    {
                        for (var j = 0; j < i + 1; j++)
                        {
                            pattern.Add(bpEnum.Current);
                            if (pattern.Count > signal.Count) break;
                        }
                        if (!bpEnum.MoveNext())
                        {
                            bpEnum = basePattern.GetEnumerator();
                            bpEnum.MoveNext();
                        }
                    }
                    pattern.RemoveAt(0);

                    processedSignal.Insert(i, 0);
                    for (var n = 0; n < signal.Count; n++)
                    {
                        processedSignal[i] += (signal[n] * pattern[n]);
                    }
                    processedSignal[i] %= 10;
                    if (processedSignal[i] < 0) processedSignal[i] *= -1;
                }
                signal = processedSignal;
            }

            var sb = new StringBuilder();
            for (var i = resultOffset; i < resultOffset+8; i++) sb.Append(signal[i]);
            return sb.ToString();
        }
    }
}
