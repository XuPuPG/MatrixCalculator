using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfLab3.UIObjects;
using static WpfLab3.Events;

namespace WpfLab3
{
    public class DataProvider
    {
        //источник
        public List<Matrix<double>> matrixDataObjects = new List<Matrix<double>>();
        public MatrixUIObjects matrixUIObjects = new MatrixUIObjects();

        Matrix<double> va = Matrix<double>.Build.Random(100, 5);

        public DataProvider()
        {
            //TODO 
            matrixDataObjects.Add(Matrix<double>.Build.Random(3, 5));
            matrixDataObjects.Add(Matrix<double>.Build.Random(6, 5));
            matrixDataObjects.Add(va);
        }

        

        public MatrixUIObjects getMatrixUIObjects()
        {
            for(int i=0; i<matrixDataObjects.Count; i++ )
            {
                var a = new MatrixUI(matrixDataObjects[i]);
                a.id = i.ToString();
                a.name = i.ToString() + " matrix";
                matrixUIObjects.Add(a);
            }

            //Добавляем обработчик для события изменения коллекции
            //добавляем обработчик для события изменения объекта в коллекции
            matrixUIObjects.ItemEndEdit += new ItemEndEditEventHandler(matrixItemEndEdit);
            matrixUIObjects.CollectionChanged += new NotifyCollectionChangedEventHandler(matrixesCollectionChanged);

            return matrixUIObjects;
        }



        //TODO: написать реализацию удаления удаленных объектов из источника
        void matrixesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("collection change");
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    MatrixUI matrixObject = item as MatrixUI;
                    Console.WriteLine("old item:" + matrixObject.id);
                    // use the data access layer to delete the wrapped data object
                    matrixDataObjects.Remove(matrixObject.getMatrix());
                    Console.WriteLine("old item:"+item);
                }
            }
        }

        //у пришедшего UI объекта забираем данные и обновляем в источнике
        void matrixItemEndEdit(IEditableObject sender)
        {
            MatrixUI matrixUI = sender as MatrixUI;
            Console.WriteLine("matrixItemEndEdit");
            if (matrixUI != null)
            {
                if (int.Parse(matrixUI.id) >= matrixDataObjects.Count) {
                    matrixDataObjects.Add(Matrix<double>.Build.Dense(1, 1));
                }


                int i = int.Parse(matrixUI.id); //TODO: переделать title в тип int , toString в матрице
                
                matrixDataObjects[i] = Matrix<double>.Build.DenseOfRowVectors(matrixUI.getUIData().Select(x => x.v));
            }
            else Console.WriteLine("nullll");


        }
    }
}
