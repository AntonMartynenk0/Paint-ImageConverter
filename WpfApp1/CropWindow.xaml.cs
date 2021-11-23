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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace WpfApp1
{
    public partial class CropWindow : Window
    {
        MainWindow mainWindow = new MainWindow();
        public double w, h; //исходные ширина и высота
        public double newW, newH; //новые значения для размера
        public CroppedBitmap cb;
        
        public CropWindow()
        {
            InitializeComponent();
        }
        
        //нажатие на кнопку ОК
        private void bnt_accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image_after.Source = image_before.Source;
            label_before.Content = w + " X " + h;
            label_after.Content = w + " X " + h;
        }

        //применение для обрезки
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //считаем новые значение для ширины и высоты
            newW = w - Convert.ToDouble(cropLeft.Text) - Convert.ToDouble(cropRight.Text);
            newH = h - Convert.ToDouble(cropTop.Text) - Convert.ToDouble(cropBottom.Text);
            
            if (newH >= 0 && newW >= 0 && newH <= h && newW <= w)
            {
                //считает координаты прямоугольника
                int X = Convert.ToInt32(cropLeft.Text);
                int Y = Convert.ToInt32(cropTop.Text);
                Int32Rect rect = new Int32Rect(X, Y, (int)newW, (int)newH); //создаем приямоугольник
                cb = new CroppedBitmap((BitmapSource)image_before.Source, rect); //создаем CroppedBitmap на основании имебщегося изображения и прямоуголинка
                image_after.Source = cb;
                label_after.Content = newW + " X " + newH; //изменяем текст label на новый размер изображения
            }
        }

        //запрет на ввод пробела
        private void crop_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        //запред на ввод не цифры
        private void crop_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}