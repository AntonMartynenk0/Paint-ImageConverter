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

namespace WpfApp1
{
    public partial class Scale : Window
    {
        public double w, h; //ширина и высота
        MainWindow mainWindow = new MainWindow();
        public Scale()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   
            //округление ширины и высоты
            textBox_Width.Text = Math.Round(w).ToString();
            textBox_Height.Text = Math.Round(h).ToString();
        }

        //запред на ввод не цифры
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        //запрет на ввод пробела
        private void scale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        //нажатие на кнопку ОК
        private void bnt_accept_Click(object sender, RoutedEventArgs e)
        {
            //получение нового значения для ширины и высоты перед закрытием окна
            w = Convert.ToDouble(textBox_Width.Text); 
            h = Convert.ToDouble(textBox_Height.Text);
            this.DialogResult = true;
        }        
    }
}