using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexPlan
{
    public class Simplex
    {
        private double[,] Table;
        private List<int> basis;
        private bool[] is_basis;
        private int mainColIndexInTable;
        private int mainRowIndexInTable;
        private int rowsCount;
        private int columnsCount;

        private List<int> variables;

        /*
         sourse:
         для системы 
         5x+9y<=45
         3x+3y<=19
         2x+y<=10
         F(x)=5x+6y

            source:
            будет выглядеть как
            5 9 45
            3 3 19
            2 1 10
            
            F:
            5 6 0
             


       table:           0  1  2  3  4   5 - allkoef
        basiselems      x0 x1 x2 x3 x4 free
      0 x2              5  9  1  0  0   45
      1 x3              3  3  0  1  0   19
      2 x4              2  1  0  0  1   10
      3 F              -5 -6  0  0  0    0
             
             */

        private bool Compatible = true;
        public Simplex(SystemOfLimitations systemOfLimitations)
        {
            Init(systemOfLimitations.ToTable(), systemOfLimitations.GetMainFunction(),systemOfLimitations.Variables);
        }

        private void Init(double[,] source, double[] F, List<int> variables)
        {
            this.variables = variables;
            int mainElemsCount = source.GetLength(1) - 1; //основные элементы x,y -1 так как в таблице лежат еще и свободные члены
            int helpElemsCount = source.GetLength(0);
            rowsCount = helpElemsCount + 1; //+1 так как еще строка для функции целевой
            columnsCount = mainElemsCount + helpElemsCount + 1; // плюс столбец для свободных членов

            //инициализация Table
            Table = new double[rowsCount, columnsCount];
            //заполняем ячейки связанные с коэффициентами перед основными элементами и свободные члены
            for (int i = 0; i < source.GetLength(0); i++)
            {
                Table[i, columnsCount - 1] = source[i, source.GetLength(1) - 1];
                for (int j = 0; j < source.GetLength(1) - 1; j++)
                    Table[i, j] = source[i, j];
            }
            //заполняем последнюю строку с данными о функции
            Table[rowsCount - 1, columnsCount - 1] = F[F.Length - 1];
            for (int j = 0; j < F.Length; j++)
                Table[rowsCount - 1, j] = -F[j];
            //заполняем правую часть таблицы 1 и 0
            for (int i = 0; i < rowsCount; i++)
                for (int j = mainElemsCount; j < columnsCount - 1; j++)
                    Table[i, j] = j == i + mainElemsCount ? 1 : 0;
            //в базис кладем вспомогательные элементы, то есть их номера начиная 
            basis = new List<int>(helpElemsCount);
            is_basis = new bool[helpElemsCount + mainElemsCount];
            for (int i = 0; i < helpElemsCount; i++)
            {
                basis.Add(i + mainElemsCount);
                is_basis[i+mainElemsCount] = true;
            }

            Compatible = PrepareNegativeVariables();

        }
/*
        private void PrepareLine(int lineNum)
        {

            int newbasis = 0;
            double free = Table[lineNum, columnsCount - 1];
            int oldbasis = basis[lineNum];
            
            for(int i = 0; i < columnsCount - 1;i++)
            {
                if (Table[lineNum, i] < 0)
                {
                    newbasis = i;
                    break;
                }
            }
            for(int i = 0; i < rowsCount;i++)
            {
                if (Table[i, newbasis] != 0)
                {
                    Table[i, oldbasis] = Table[i, newbasis];
                    Table[i, newbasis] = 0;
                    Table[i, columnsCount - 1] += free * Table[i, oldbasis];
                    Table[i, columnsCount - 1] = HelpUtils.SmartRound(Table[i, columnsCount - 1]);
                }
            }
            Table[lineNum, columnsCount - 1] = -free;
            Table[lineNum, newbasis] = 1;
            Table[lineNum, oldbasis] = -1;
            basis[lineNum] = newbasis;
        }


        private void PrepareLineStock(int lineNum)
        {

            int newbasis = 0;
            double free = Table[lineNum, columnsCount - 1];
            int oldbasis = basis[lineNum];
            int pair = 0;
            int i = 0;
            for (; i < columnsCount - 1; i++)
            {
                if (Table[lineNum, i] < 0)
                {
                    pair = i;
                    break;
                }
            }
            i++;
            for (; i < columnsCount - 1; i++)
            {
                if (Table[lineNum, i] < 0)
                {
                    newbasis = i;
                    break;
                }
            }
            i = 0;
            for (; i < rowsCount; i++)
            {
                if (Table[i, newbasis] != 0)
                {
                    double coef = Table[i, newbasis];
                    Table[i, oldbasis] += coef;
                    Table[i, pair] -= coef;
                    Table[i, newbasis] = 0;
                    Table[i, columnsCount - 1] += free * coef;
                    Table[i, columnsCount - 1] = HelpUtils.SmartRound(Table[i, columnsCount - 1]);
                }
            }
            Table[lineNum, columnsCount - 1] = -free;
            Table[lineNum, newbasis] = 1;
            Table[lineNum, pair] = 1;
            Table[lineNum, oldbasis] = -1;
            basis[lineNum] = newbasis;
        }

        private bool IsItPersonalLimit(int index)
        {
            int cnt = 0;
            for (int i = 0; i < columnsCount - 1; i++)
                if (Table[index, i] != 0)
                    cnt++;
            return cnt == 2;
        }
        */
        private void PrepareTable(int mainRow, int mainCol)
        {
            mainColIndexInTable = mainCol;
            mainRowIndexInTable = mainRow;
            RebuildTable();
        }

        private int FindNegativeRow()
        {
            for(int i = 0; i < rowsCount - 1; i++)
            {
                if (Table[i, columnsCount - 1] < 0)
                    return i;
            }
            return -1;
        }

        private int FindNegativeCol(int Row)
        {
            for (int i = 0; i < columnsCount - 1; i++)
            {
                if (Table[Row, i] < 0)
                    return i;
            }
            return -1;
        }

        private bool PrepareNegativeVariables()
        {
            int row = FindNegativeRow();
            while (row > -1)
            {
                int col = FindNegativeCol(row);
                if (col < 0)
                    return false;
                PrepareTable(row, col);
                row = FindNegativeRow();
            }
            return true;
        }


        private int Max_IsEnd()
        {
            for (int i = 0; i < columnsCount-1; i++)
            {
                if (Table[rowsCount - 1, i] < 0)
                    return i;
            }
            return -1;
        }

        private int Min_IsEnd()
        {
            for (int i = 0; i < columnsCount - 1; i++)
            {
                if (Table[rowsCount-1, i] > 0)
                    return i;
            }
            return -1;
        }

        private void Max_FindMainColumn(int indexFirst)
        {
            double min = Table[rowsCount - 1, indexFirst];
            mainColIndexInTable = indexFirst;
            for (int i = indexFirst + 1; i < columnsCount-1; i++)
            {
                double tmp = Table[rowsCount - 1,i ];
                if (tmp < min && !is_basis[i])
                    {
                        min = tmp;
                        mainColIndexInTable = i;
                    }

            }
        }

        private bool FindMainRow()
        {
            double min = double.PositiveInfinity;
            mainRowIndexInTable = 0;
            for (int i = 0; i < rowsCount - 1; i++)
            {
                double tmp;
                if (Table[i, mainColIndexInTable] != 0)
                {
                    if (Table[i, columnsCount - 1] >= 0 && Table[i, mainColIndexInTable] < 0)
                        tmp = double.PositiveInfinity;
                    else
                        tmp = Table[i, columnsCount - 1] / Table[i, mainColIndexInTable];
                }
                else
                    tmp = double.PositiveInfinity;
                if (tmp <= min)
                {
                    min = tmp;
                    mainRowIndexInTable = i;
                }
            }
            return min < double.PositiveInfinity;
        }

    
        /*private void Min_FindMainColumn(int indexFirst)
        {
            double max = Table[rowsCount - 1, indexFirst];
            mainColIndexInTable = indexFirst;
            for (int i = indexFirst + 1; i < columnsCount - 1; i++)
            {
                double tmp = Table[rowsCount - 1, i];
                int koef = i;
                if (tmp > max)
                    if (!basis_set.Contains(koef))
                    {
                        max = tmp;
                        mainColIndexInTable = i;
                    }

            }
        }
        */
        private void RebuildTable()
        {
            is_basis[basis[mainRowIndexInTable]]=false;
            basis[mainRowIndexInTable] = mainColIndexInTable;
            is_basis[mainColIndexInTable] = true;

            //ключевой элемент
            double keyElem = Table[mainRowIndexInTable, mainColIndexInTable];
            //изменяем ключевую строку
            for (int i = 0; i < columnsCount; i++)
            {
                Table[mainRowIndexInTable, i] = HelpUtils.SmartRound(Table[mainRowIndexInTable, i] / keyElem);
            }
            //Table[mainRowIndexInTable, mainColIndexInTable] = 1;
            for (int i = 0; i < rowsCount; i++)
            {
                if (i != mainRowIndexInTable)
                {
                    double rowkey = Table[i, mainColIndexInTable];
                    if(rowkey!=0)
                    for (int j = 0; j < columnsCount; j++)
                    {
                        Table[i, j] = HelpUtils.SmartRound(Table[i, j] - Table[mainRowIndexInTable, j] * rowkey);
                    }
                    //Table[i, mainColIndexInTable] = 0;//для точности обнулим
                }
            }
        }

        private Decidion GetResult()
        {
            if (!Max_CheckResult())
                return null;
            double res = Table[rowsCount - 1, columnsCount - 1];
            KeyValuePair<double, Dictionary<int,double>> answer = new KeyValuePair<double, Dictionary<int,double>>(res, new Dictionary<int, double>());
            for (int i = 0; i < rowsCount - 1; i++)
            {
                if (basis[i]<variables.Count)
                    answer.Value[variables[basis[i]]] = Table[i, columnsCount - 1];
            }
            return new Decidion(answer.Key,answer.Value);
        }

        private bool Max_CheckResult()
        {
            for (int i = 0; i < rowsCount - 1; i++)
                if (Table[i, columnsCount - 1] < 0)
                    return false;
            return true;
        }

        private  bool Min_CheckResult()
        {
            for( int i = 0; i < rowsCount-1; i++)
            {
                if (Table[i, columnsCount-1] < 0)
                    return false;
            }
            return true;
        }

        private Decidion Max_Cycle()
        {
            int indexFirst = Max_IsEnd();
            while (indexFirst != -1)
            {
                Max_FindMainColumn(indexFirst);
                if (!FindMainRow())
                    return null;
                RebuildTable();
                indexFirst = Max_IsEnd();
            }
            return GetResult();
        }

      /*  private Decidion Min_Cycle()
        {
            int indexFirst = Min_IsEnd();
            while (indexFirst != -1)
            {
                Min_FindMainColumn(indexFirst);
                if (!FindMainRow())
                    return null;
                RebuildTable();
                indexFirst = Min_IsEnd();
            }
            if (Min_CheckResult())
                return GetResult();
            else
                return null;
        }*/

        public Decidion FindDecidion( bool min=false)
        {
            if (!Compatible)
                return null;
            /*if (min)
                return Min_Cycle();*/
            return Max_Cycle();
        }

    }
}
