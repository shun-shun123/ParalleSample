using System;
using System.Threading;
using System.Threading.Tasks;

namespace A
{
    class Program
    {
        private static int N = 100;
        
        static void Main(string[] args)
        {
            var bootMessage = $@"
1: ParallelSampleCall()
2: ParallelLoopResult()
3: ParallelLoopState()";
            Console.WriteLine(bootMessage);
            var number = Console.ReadLine();
            if (int.TryParse(number, out var command))
            {
                switch (command)
                {
                    case 1:
                        ParallelSampleCall();
                        break;
                    case 2:
                        ParallelLoopResult();
                        break;
                    case 3:
                        ParallelLoopState();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Input value can't convert to integer.");
            }
        }

        private static void Method2(int i)
        {
            Console.WriteLine($"Method2: {i}");
        }
        
        #region SampleCode

        /// <summary>
        /// Parallelから並列処理を呼び出す方法サンプルコード
        /// </summary>
        private static void ParallelSampleCall()
        {
            // using a named method
            Parallel.For(0, N, Method2);

            // using an anonymous method
            Parallel.For(0, N, delegate(int i)
            {
                Console.WriteLine($"delegate: {i}");
            });

            // using a lambda expression
            Parallel.For(0, N, i =>
            {
                Console.WriteLine($"lambda: {i}");
            });
        }

        /// <summary>
        /// Parallelから実行された並列処理の結果を受け取る
        /// </summary>
        private static void ParallelLoopResult()
        {
            var result = Parallel.For(0, N, i =>
            {

            });
            // 全てのループ処理が完了しているのであれば、これはtrueになる。
            // 
            Console.WriteLine($"result.IsCompleted: {result.IsCompleted}");
            Console.WriteLine($"result.LowestBreakIteration: {result.LowestBreakIteration}");

            // IsCompletedがfalseでLowestBreakIterationがnon-null integral valueの場合はBreakにより終了されている
            // LowestBreakIterationはBreakが使われたループの最小のインデックス値
            result = Parallel.For(0, N, (i, state) =>
            {
                if (i == 10)
                {
                    state.Break();
                }
            });
            Console.WriteLine($"result.IsCompleted: {result.IsCompleted}");
            Console.WriteLine($"result.LowestBreakIteration: {result.LowestBreakIteration}");

            // IsCompletedがfalseでLowestBreakIterationがnullの時はStopによりループが終了されている
            result = Parallel.For(0, N, (i, state) =>
            {
                if (i == 10)
                {
                    state.Stop();
                }
            });
            Console.WriteLine($"result.IsCompleted: {result.IsCompleted}");
            Console.WriteLine($"result.LowestBreakIteration: {result.LowestBreakIteration}");
        }

        /// <summary>
        /// ParallelLoopStateについて
        /// 並列処理の各ループが他のループ処理と相互作用できるようにするためのクラス
        /// ユーザコードからインスタンス生成することはできず、Parallelクラスによって生成される。
        /// </summary>
        private static void ParallelLoopState()
        {
            var rnd = new Random();
            int breakIndex = rnd.Next(1, 11);
            
            // あるインデックスで並列処理を終了させる予定
            Console.WriteLine($"Will call Break at iteration {breakIndex}");

            var result = Parallel.For(1, 101, (i, state) =>
            {
                Console.WriteLine($"Beginning iteration {i}");
                int delay;
                lock (rnd)
                {
                    delay = rnd.Next(1, 1001);
                }
                Thread.Sleep(delay);

                // Breakが他のループ処理で呼ばれていても、そのほかのループ処理はすでに実行中かもしれないので、ここでチェックできる
                // ShouldExitCurrentIterationがtrueの場合は、他のイテレーション内でBreakが呼ばれたことを示す。
                if (state.ShouldExitCurrentIteration)
                {
                    if (state.LowestBreakIteration < i)
                    {
                        return;
                    }
                }

                if (i == breakIndex)
                {
                    Console.WriteLine($"Break in iteration {i}");
                    state.Break();
                }
                
                Console.WriteLine($"Completed iteration {i}");
            });

            if (result.LowestBreakIteration.HasValue)
            {
                Console.WriteLine($"Lowest Break Iteration: {result.LowestBreakIteration}");
            }
            else
            {
                Console.WriteLine($"No lowest break iteration");
            }
        }
        
        #endregion
    }
}
