using DataProcessLibrary.Data;
using SimplexPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DataProcessLibrary.Analys
{
    public delegate void NextProgress();

    public static class SimplexStage
    {
        public static event NextProgress OnProgress;

        public static double StartRisk= 0.7;
        public static double FinishRisk = 1;
        public static SingleDecidionGraphic FindDecidions(AnalysData analysData)
        {
            CreateMainSystemOfLimitation();
            return CountForAllRisks(analysData);
        }

        private static SystemOfLimitations SystemOfLimitations;

        private static void CreateMainSystemOfLimitation()
        {
            
            SystemOfLimitations = DataScript.CreateSystemTemplate();
        }

        private static Dictionary<double, Dictionary<DataScript, double>> results;

        private static Mutex Mutex = new Mutex();

        public static Dictionary<DataScript, List<Point>> pointsToTest = new Dictionary<DataScript, List<Point>>();

        private static SingleDecidionScript DecideInThread(double risk, AnalysData AnalysData, List<DataScript> dataScripts)
        {
            SystemOfLimitations localSystemOfLimitations = SystemOfLimitations.Copy();
            SingleDecidionScript best = null;
            double max = double.NegativeInfinity;
            List<KeyValuePair<DataScript, double>> localres = new List<KeyValuePair<DataScript, double>>();

            for (int i = 0; i < dataScripts.Count(); i++)
            {
                if (NeedToStop)
                    return null;
                    var t = DateTime.Now;
                    dataScripts[i].UpdateSystem(ref localSystemOfLimitations, AnalysData, risk);
                    IntegerSimplex simplex = new IntegerSimplex(localSystemOfLimitations);
                    var decidion = simplex.FindResult();
                    if(!(OnProgress is null))
                        OnProgress();
                MaxIsReady++;
                    if (!(decidion is null))
                    {
                        
                        var t2 = DateTime.Now;
                        SingleDecidionScript tmp = new SingleDecidionScript(dataScripts[i], decidion, AnalysData);
                        localres.Add(new KeyValuePair<DataScript, double>(dataScripts[i], tmp.decidion.MaxResult));
                        if (tmp.decidion.MaxResult > max)
                        {
                            max = tmp.decidion.MaxResult;
                            best = tmp;
                        }
                        var dif2 = DateTime.Now - t2;
                    }
                    else
                    {
                    localres.Add(new KeyValuePair<DataScript, double>(dataScripts[i], double.NegativeInfinity));
                }
                    

            }
            Mutex.WaitOne();
            foreach(var l in localres)
            {
                results[risk][l.Key] = l.Value;
            }
            Mutex.ReleaseMutex();
            return best;
        }
        
        public static SingleDecidionScript Decide(double risk, AnalysData AnalysData)
        {
            if(results is null)
            {
                results = new Dictionary<double, Dictionary<DataScript, double>>();
            }
            results[risk] = new Dictionary<DataScript, double>();
            int N = AnalysData.dataScripts.Count();
            if (N == 0)
                return null;

            int cntOfProcessors =Math.Min(N,Environment.ProcessorCount*2);
            //int cntOfProcessors = 1;
            int len = N / cntOfProcessors;
            len += len * cntOfProcessors == N ? 0 : 1;
            List<Task<SingleDecidionScript>> tasks = new List<Task<SingleDecidionScript>>();
            for(int i = 0; i < cntOfProcessors; i++)
            {
                int ind = i;
                List<DataScript> dataScripts = new List<DataScript>();
                for(int j=len*ind;j< Math.Min(len * (ind + 1), N); j++)
                {
                    dataScripts.Add(AnalysData.dataScripts[j]);
                }
                Task<SingleDecidionScript> task = Task<SingleDecidionScript>.Run(() =>
                   {
                       try
                       {
                           return DecideInThread(risk, AnalysData, dataScripts);
                       }
                       catch(Exception e)
                       {
                           throw e;
                       }
                   }
                );
                tasks.Add(task);
            }
            SingleDecidionScript best = null;
            double max = double.NegativeInfinity;
            for(int i = 0; i < cntOfProcessors; i++)
            {
                try
                {
                    tasks[i].Wait();
                }
                catch(Exception e)
                {
                    throw e;
                }
                SingleDecidionScript tmpDec = tasks[i].Result;
                if(!(tmpDec is null) && tmpDec.decidion.MaxResult > max)
                {
                    best = tmpDec;
                    max = tmpDec.decidion.MaxResult;
                }

            }

            risksIsReady++;
            return best;
        }


        private static int risksIsReady = 0;
        private static int MaxIsReady = 0;
        private static void ClearNulls(List<DataScript> dataScripts, double max, double risk)
        {
            int amountofrisks = (int)(FinishRisk * 100) - (int)(StartRisk * 100) + 1 - risksIsReady;
            int maximum = amountofrisks * dataScripts.Count()+MaxIsReady;
            if (!(OnSetMaximum is null))
                OnSetMaximum(maximum);
            if (!results.ContainsKey(risk))
                return;
            double lastcnt = dataScripts.Count();
            dataScripts.RemoveAll((c) => (results[risk].Any(d=>d.Key.combination_id==c.combination_id)? (results[risk].Where(d=>d.Key.combination_id==c.combination_id).First().Value<max):true));
            if (lastcnt != dataScripts.Count())
            {

            }

            
        }

        private static void PrintPoints(AnalysData analysData, double risk)
        {
            foreach (var d in analysData.dataScripts)
            {
                if (!pointsToTest.ContainsKey(d))
                {
                    pointsToTest[d] = new List<Point>();
                }
                pointsToTest[d].Add(new Point(risk, results[risk][d]));
            }
        }


        private static void FindDecidionForRisk(ref AnalysData AnalysData, double start, double finish, ref Dictionary<double,SingleDecidionScript> result )
        {
            double step = 0.01;
            SingleDecidionScript decSqript;
            double startMax = 0;
            if (!result.ContainsKey(start/step))
            {
                GeneratorDataScriptsStage.GenerateDateScripts(ref AnalysData, start);
                
                ClearNulls(AnalysData.dataScripts, -1, StartRisk - step);
                decSqript = Decide((double)start, AnalysData);
                if (!(decSqript is null))
                {
                    startMax = decSqript.decidion.MaxResult;
                    decSqript.Init(StartRisk);
                    result[Math.Round(StartRisk / step,2)] = decSqript;
                }
            }
 


            GeneratorDataScriptsStage.GenerateDateScripts(ref AnalysData, finish);
            ClearNulls(AnalysData.dataScripts, -1, StartRisk - step);
            decSqript = Decide((double)finish, AnalysData);
            double finishMax = 0;
            if (!(decSqript is null))
            {
                finishMax = decSqript.decidion.MaxResult;
                decSqript.Init(finish);
                result[Math.Round(finish / step,2)] = decSqript;
            }

            double rightMax = finishMax;
            double riskLeft = start + step;
            double riskRight = finish - step;
            while (riskLeft <= riskRight)
            {
                GeneratorDataScriptsStage.GenerateDateScripts(ref AnalysData, riskLeft);
                ClearNulls(AnalysData.dataScripts, rightMax, riskLeft - step);
                decSqript = Decide((double)riskLeft, AnalysData);
                if (!(decSqript is null))
                {
                    decSqript.Init(riskLeft);
                    result[Math.Round(riskLeft / step,2)] = decSqript;
                }

                riskLeft = Math.Round(riskLeft + step, 2);
            }

        }

        public static SingleDecidionScript FlexibilityAnalys(AnalysData analysData, double Risk)
        {
            results = new Dictionary<double, Dictionary<DataScript, double>>();
            CreateMainSystemOfLimitation();
            SingleDecidionScript decSqript;
            GeneratorDataScriptsStage.GenerateDateScripts(ref analysData, Risk);
            decSqript = Decide((double)Risk, analysData);
            if (!(decSqript is null))
            {
                decSqript.Init(Risk);
            }
            return decSqript;
        }

        public  delegate void SetMaximum(int maximum);
        public static event SetMaximum OnSetMaximum;

        public delegate void SetStart();
        public static event SetStart OnStart;

        public static bool NeedToStop = false;
        private static SingleDecidionGraphic CountForAllRisks(AnalysData AnalysData)
        {
            NeedToStop = false;
            //pointsToTest = new Dictionary<DataScript, List<Point>>();
            var result = new Dictionary<double, SingleDecidionScript>();
            results = new Dictionary<double, Dictionary<DataScript, double>>();
            //double step = 0.01;
            double riskLeft = StartRisk;
            double riskMiddle1 =Math.Round( StartRisk + (FinishRisk-StartRisk)/3,2);
            double riskMiddle2 =Math.Round( StartRisk + (FinishRisk - StartRisk) / 3 *2,2);
            double riskRight = FinishRisk;


            risksIsReady = 0;
            MaxIsReady = 0;
            if(!(OnStart is null))
                OnStart();
            try
            {
                FindDecidionForRisk(ref AnalysData, StartRisk, riskMiddle1, ref result);
                FindDecidionForRisk(ref AnalysData, riskMiddle1, riskMiddle2, ref result);
                FindDecidionForRisk(ref AnalysData, riskMiddle2, FinishRisk, ref result);

            }
            catch(Exception e)
            {
                throw e;
            }
            /*
             *
             *while (riskLeft <= riskRight)
            {
                if (riskLeft < riskRight)
                {
                    middleStep = (leftMax - rightMax) / ((riskRight - riskLeft) / step);
                }
                GeneratorDataScriptsStage.GenerateDateScripts(ref AnalysData, riskRight);
                ClearNulls(AnalysData.dataScripts, rightMax, riskLeft-step);
                var decSqript = Decide((double)riskRight, AnalysData);

                if (!(decSqript is null))
                {
                    rightMax = decSqript.decidion.MaxResult;
                    decSqript.Init(riskRight);
                    result[riskRight * 100] = decSqripx;
                }
                riskRight -= step;
                riskRight = Math.Round(riskRight,4);
                
                //PrintPoints(AnalysData, riskRight);

                if (riskLeft <= riskRight)
                {
                    if (riskLeft < riskRight)
                    {
                        middleStep = (leftMax - rightMax) / ((riskRight - riskLeft) / step);
                    }
                    GeneratorDataScriptsStage.GenerateDateScripts(ref AnalysData, riskLeft);

                    ClearNulls(AnalysData.dataScripts, rightMax, riskLeft - step);
                    var decSqriptLeft = Decide((double)riskLeft, AnalysData);
                    if (!(decSqriptLeft is null))
                    {
                        leftMax2 = leftMax;
                        leftMax = decSqriptLeft.decidion.MaxResult;
                        decSqriptLeft.Init( riskLeft);
                        result[riskLeft * 100] = decSqriptLeft;
                    }
                    riskLeft += step;
                    riskLeft = Math.Round(riskLeft, 4);
                    //PrintPoints(AnalysData, riskLeft);
                }
            }
            riskMiddle = Math.Round((riskMiddle+step) / step) * step;
            while (riskMiddle <= FinishRisk)
            {
                GeneratorDataScriptsStage.GenerateDateScripts(ref AnalysData, riskMiddle);
                ClearNulls(AnalysData.dataScripts, -1, riskRight);
                var decSqript = Decide((double)riskMiddle, AnalysData);

                if (!(decSqript is null))
                {
                    rightMax = decSqript.decidion.MaxResult;
                    decSqript.Init(riskMiddle);
                    result[riskMiddle * 100] = decSqripx;
                }
                riskMiddle += step;
            }*/

            return new SingleDecidionGraphic(result);
        }

        public static SingleDecidionScript FindDecidionForRisk(double risk,AnalysData AnalysData)
        {
            results = new Dictionary<double, Dictionary<DataScript, double>>();
            CreateMainSystemOfLimitation();
            return Decide(risk, AnalysData);
        }
    }
}
