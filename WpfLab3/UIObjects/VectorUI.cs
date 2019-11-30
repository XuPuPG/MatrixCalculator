using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WpfLab3.Events;

namespace WpfLab3.UIObjects
{
    public class VectorUI : IEditableObject, INotifyPropertyChanged
    {
        public Vector<double> v;

        public VectorUI(Vector<double> v_) {
            v = v_;
        }

        public double this[int i]
        {
            get { return v[i]; }
            set
            {
                v[i] = value;
                RaisePropertyChanged("any"); // вызывает событие изменения PropertyChanged
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event ItemEndEditEventHandler ItemEndEdit;


        public void BeginEdit()
        {
            Console.WriteLine("VectorUI: begin edit");
        }

        public void CancelEdit()
        {
            Console.WriteLine("VectorUI: cancel edit");
        }

        public void EndEdit()
        {
            if (ItemEndEdit != null)
            {
                ItemEndEdit(this);
            }
            Console.WriteLine("VectorUI: endEdit");
        }
  

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                //вызов события. Вызывается обработчик согласно сигнатуре делегата - отправитель и тип
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
