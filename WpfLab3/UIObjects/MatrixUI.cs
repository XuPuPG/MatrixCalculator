using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static WpfLab3.Events;

namespace WpfLab3.UIObjects
{

    public class MatrixUI : IEditableObject, INotifyPropertyChanged
    {
        /*
        Доступ к UI матрице и списку векторов должен предоставляться только в режиме read only
        */
        private Matrix<double> matrix;
        private List<VectorUI> list_vectorUI = new List<VectorUI>();

        public List<VectorUI> getUIData() {
            return list_vectorUI;
        }
        public Matrix<double> getMatrix() {
            return matrix;
        }
        private bool isChoosed_;
        public string id { get; set; }

        public string name { get;  set; }
        public bool isChoosed {
            get { return isChoosed_; }
            set {
                isChoosed_ = value;
                RaisePropertyChanged("isChoosed");

            }
        }

        public void setName(string name_)
        {
            this.name= name_;
            RaisePropertyChanged("name");
        }

        // TODO: матрица и список строк матрицы не синхронизированны.
        public MatrixUI(Matrix<double> mt)
        {
            setMatrix(mt);
        }

        public void setMatrix(List<VectorUI> lvec)
        {
            foreach(var i  in lvec)
            {
                i.ItemEndEdit += new ItemEndEditEventHandler(ItemEndEditHandler);
            }
            list_vectorUI = lvec;
            EndEdit();
        }

        public void setMatrix(Matrix<double> mt)
        {
            matrix = mt;

            foreach (var vector in matrix.EnumerateRows())
            {
                VectorUI vec = new VectorUI(vector);
                vec.ItemEndEdit += new ItemEndEditEventHandler(ItemEndEditHandler);
                list_vectorUI.Add(vec);
            }
            EndEdit();
        }

        void ItemEndEditHandler(IEditableObject sender)
        {
            if (ItemEndEdit != null)
            {
                ItemEndEdit(this);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event ItemEndEditEventHandler ItemEndEdit; 

        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            if (ItemEndEdit != null)
            {
                ItemEndEdit(this);
            }
            Console.WriteLine("Matrix: end edit");
        }

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                Console.WriteLine(property);
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
