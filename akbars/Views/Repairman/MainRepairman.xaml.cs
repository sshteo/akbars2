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
using System.Windows.Shapes;

namespace akbars.Views.Repairman
{
    /// <summary>
    /// Логика взаимодействия для MainRepairman.xaml
    /// </summary>
    public partial class MainRepairman : Window
    {
        public MainRepairman(string fio)
        {
            InitializeComponent();
            HelloText.Text = "Привет, исполнитель " + fio;
        }

       
    }
}
