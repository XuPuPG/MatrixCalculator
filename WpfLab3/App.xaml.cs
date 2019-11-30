using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfLab3.UIObjects;

namespace WpfLab3
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {

        void printMatrix(Matrix<double> m)
        {
            Console.WriteLine("START");
            foreach(var i in m.EnumerateRows())
            {
                Console.WriteLine(i.ToVectorString());
            }
            Console.WriteLine("FINISH");
        }

        Matrix<double> m1 = Matrix<double>.Build.Random(1, 2);

        App()
        {
            

            MatrixUI mui = new MatrixUI(m1);

            //вывести содержимое матрицы-источника и UI матрицы
            //изменить содержимое UI матрицы, вектор 0 0,0
            //1

            
            
            /*
            Vector<double> v1 = Vector<double>.Build.Random(3, 5);
            VectorUI v2 = new VectorUI(v1);

            Console.WriteLine(v1.ToVectorString());
            Console.WriteLine("------------------------");
            Console.WriteLine(v2.v.ToVectorString());

            v2[0] = 1.0;
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@2");

            Console.WriteLine(v1.ToVectorString());
            Console.WriteLine("----------------------------");
            Console.WriteLine(v2.v.ToVectorString());
            */
        }

    }
}
