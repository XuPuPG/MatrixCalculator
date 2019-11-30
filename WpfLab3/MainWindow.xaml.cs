using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WpfLab3.UIObjects;
using System.ComponentModel;

namespace WpfLab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataProvider dp = new DataProvider();
        HierholzerAlgorithm hierholzer = new HierholzerAlgorithm();

        public MainWindow()
        {
            InitializeComponent();
            list_matrixes.ItemsSource = dp.getMatrixUIObjects();
        }


        void resetColumns(int size)
        {
            matrix_grid.Columns.Clear();
            for (int i = 0; i < size; i++)
            {
                var col = new DataGridTextColumn();
                col.Header = i;
                col.Binding = new Binding(string.Format("[{0}]", i));
                matrix_grid.Columns.Add(col);
            }
        }

        void r_resetColumns(int size)
        {
            result_grid.Columns.Clear();
            for (int i = 0; i < size; i++)
            {
                var col = new DataGridTextColumn();
                col.Header = i;
                col.Binding = new Binding(string.Format("[{0}]", i));
                result_grid.Columns.Add(col);
            }
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            var m = new MatrixUI(
                    Matrix<double>.Build.Random(
                        int.Parse(row_inp.Text),
                        int.Parse(col_inp.Text))
                        );
            m.id = dp.matrixUIObjects.Count.ToString();
            m.name = (m.id+ " matrix");
            dp.matrixUIObjects.Add(m);
            dp.matrixDataObjects.Add(m.getMatrix());
            
            
            var a = list_matrixes.SelectedItem as MatrixUI;
            if (a != null)
            {
                Console.WriteLine(a.id);
                Console.WriteLine(dp.matrixDataObjects[int.Parse(a.id)].ToMatrixString());
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var m = list_matrixes.SelectedItem as MatrixUI;
            if (m != null) {

                list_matrixes.SelectedIndex -= 1;
                dp.matrixUIObjects.Remove(m);
            }
        }

        private void Matrixes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = list_matrixes.SelectedItem as MatrixUI;

            if (list_matrixes.SelectedIndex == -1)
            {
                matrix_grid.ItemsSource = null;
                resetColumns(0);
            }

            if (sender != null && a!=null)
            {
                resetColumns(a.getMatrix().ColumnCount);
                matrix_grid.ItemsSource = a.getUIData();
            }
        }

        private void Matrix_grid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            /*
            var cellInfo = matrix_grid.SelectedCells[0];
            var content = cellInfo.Column.GetCellContent(cellInfo.Item);
            Console.WriteLine((content as TextBlock).Text.ToString());
        */    
    }

        private void Change_matrix_title_Click(object sender, RoutedEventArgs e)
        {
            var a = list_matrixes.SelectedItem as MatrixUI;


            if (a != null)
            {
                var idx = list_matrixes.SelectedIndex;
                a.setName (matrix_title.Text.ToString());
            }
        }


        private void Show_only_checked_mx_Checked(object sender, RoutedEventArgs e)
        {
            list_matrixes.Items.Filter = (o) => { return (o as MatrixUI).isChoosed;};
        }

        private void Show_only_checked_mx_Unchecked(object sender, RoutedEventArgs e)
        {
            list_matrixes.Items.Filter = null;
        }


        private List<MatrixUI> getCheckedMatrixes()
        {
            List<MatrixUI> ms = new List<MatrixUI>();
            foreach(MatrixUI matrix in dp.matrixUIObjects)
            {
                if (matrix.isChoosed) ms.Add(matrix);
            }
            return ms;
        }

        private List<KeyValuePair<string, string>> get_mult_sqns()
        {
            var ms = getCheckedMatrixes();
            if (ms.Count < 2) { return new List<KeyValuePair<string, string>>();  };
            List<KeyValuePair<string , string>> m_sizes = new List<KeyValuePair<string, string>>();

            foreach(var m in ms)
            {
                m_sizes.Add(new KeyValuePair<string, string>(
                    m.getMatrix().RowCount.ToString(), 
                    m.getMatrix().ColumnCount.ToString()));
            }
            hierholzer.setEdges(m_sizes);

            if(ms.Count == 2)
            {
                InputDialog inputDialog = new InputDialog("Укажите порядок умножения явно через запятую", this,",");
                if (inputDialog.ShowDialog() == true)
                {
                    string s = inputDialog.Answer;
                    string n, m = "";
                    int found = s.IndexOf(",");
                    n = s.Substring(0, found).Trim();
                    m = s.Substring(found + 1).Trim();
                    hierholzer.setStartEdge(n, m);
                }
            }

            return hierholzer.find_path();
        }

      

        private void Save_result_Click(object sender, RoutedEventArgs e)
        {
            var m = list_matrixes.SelectedItem as MatrixUI;
            if (m != null)
            {
                Matrix<double> mr = Matrix<double>.Build.DenseOfRows((result_grid.ItemsSource as List<VectorUI>).Select(x => x.v));
                MatrixUI mui = new MatrixUI(mr);
                mui.id = m.id;
                mui.setName(m.name);
                mui.isChoosed = true;
                dp.matrixDataObjects[int.Parse(m.id)] = mr; // model
                int i = dp.matrixUIObjects.IndexOf(m);                //model view
                dp.matrixUIObjects[i] = mui;
                resetColumns(mr.ColumnCount); //view
                matrix_grid.ItemsSource = mui.getUIData();
                //обновить представление 
                
            }
        }

        //summ       
        private bool isCumulative()
        {
            var ms = getCheckedMatrixes();
            if (ms.Count < 2) { return false; }

            int rows = ms[0].getMatrix().RowCount;
            int col = ms[0].getMatrix().ColumnCount;
            Console.WriteLine("ROWS:"+rows);
            Console.WriteLine("COLUMNS:" + col);

            foreach (var m in ms)
            {
                if (rows != m.getMatrix().RowCount)
                {
                    Console.WriteLine("!1 "+m.getMatrix().RowCount);
                    return false;
                }
                if (col != m.getMatrix().ColumnCount)
                {

                    Console.WriteLine("!2 "+m.getMatrix().ColumnCount);
                    return false;
                }
            }
            return true;
        }

        void printMatrix(Matrix<double> m)
        {
            Console.WriteLine("START");
            foreach (var i in m.EnumerateRows())
            {
                Console.WriteLine(i.ToVectorString());
            }
            Console.WriteLine("FINISH");
        }

        private void Result_Click(object sender, RoutedEventArgs e)
        {

            var op = operations.SelectedItem as Label;
            if (op != null)
            {

                if (op.Name.Equals("summ"))
                {
                    if (isCumulative())
                    {
                        var ms = getCheckedMatrixes();
                        Matrix<double> rm = Matrix<double>.
                            Build.
                            Dense(ms[0].getMatrix().RowCount, ms[0].getMatrix().ColumnCount);
                        
                        foreach(var m in ms)
                        {
                            rm += dp.matrixDataObjects[int.Parse(m.id)];
                            //rm += m.getMatrix();
                        }

                        printMatrix(rm);

                        MatrixUI rm_ui = new MatrixUI(rm);
                        r_resetColumns(rm.ColumnCount);
                        result_grid.ItemsSource = rm_ui.getUIData();

                    }
                    else
                    {
                        MessageBox.Show("Невозможно сложить матрицы: матрицы не выбраны или размер не одинаковый.");
                    }
                }
                if (op.Name.Equals("multiply"))
                {
                    var ms = getCheckedMatrixes();
                    if (ms.Count >=2) { //если отмеченных матриц больше или равно двум
                    var resultPath = get_mult_sqns();
                    HashSet<int> used_matrix_id = new HashSet<int>();

                        Matrix<double> rm = Matrix<double>.Build.Dense(1, 1);
                        MatrixUI rm_ui = new MatrixUI(rm);

                        if (resultPath.Count >= 2)
                        {
                            /*
                            1) Взять элемент из пути
                            2) Найти матрицу с такими размерностями
                            3) Добавить ее в set - если получилось - умножить на копию
                            если не получилось - искать 
                            */
                            rm = Matrix<double>.Build.Dense(
                            int.Parse(resultPath[0].Key), int.Parse(resultPath[0].Value));


                            foreach (var p in resultPath) {
                            foreach (var m in ms)
                            {
                                int r = int.Parse(p.Key);
                                int c = int.Parse(p.Value);

                                if (m.getMatrix().RowCount == r && m.getMatrix().ColumnCount == c)
                                {
                                    if (used_matrix_id.Add(int.Parse(m.id)))
                                    {
                                            if (rm.ColumnCount == r) {
                                                rm *= dp.matrixDataObjects[int.Parse(m.id)];
                                            }else
                                            {
                                                rm = dp.matrixDataObjects[int.Parse(m.id)];
                                            }
                                    }
                                }
                            }
                        }

                    }

                    if (resultPath.Count < 2)
                    {
                        MessageBox.Show("Невозможно переменожить матрицы: " +
                            "матрицы не выбраны или размерности не подходят");
                    };
                        rm_ui = new MatrixUI(rm);

                        r_resetColumns(rm_ui.getMatrix().ColumnCount);
                        result_grid.ItemsSource = rm_ui.getUIData();

                }
                }

                if (op.Name.Equals("transpose"))
                {
                    var matrix = list_matrixes.SelectedItem as MatrixUI;
                    if (matrix != null)
                    {
                        Matrix<double> m = Matrix<double>.Build.DenseOfRowVectors(
                            matrix.getUIData().ToArray().Select(x => x.v)
                            ).Transpose();
                        MatrixUI m_ui = new MatrixUI(m);

                        r_resetColumns(m.ColumnCount);
                        result_grid.ItemsSource = m_ui.getUIData();
                    }
                    
                }
                
            }
        }
    }
}
