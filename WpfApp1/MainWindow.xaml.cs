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
using System.IO;
using System.Windows.Ink;
using System.Windows.Markup;
using System.Windows.Media.Effects;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        int rotateAngle = 0;    //угол поворота изображения

        public MainWindow() //инициализация компонентов
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)    //загрузка главного окна
        {
            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.RenderingBias = RenderingBias.Quality;
            shadowEffect.ShadowDepth = 15;
            shadowEffect.BlurRadius = 30;
            shadowEffect.Color = Colors.Black;
            shadowEffect.Opacity = 0.2;
            viewBox.Effect = shadowEffect;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)    //сохранение в графичесий файл
        {
            try
            {
                //скролим вверх и влево для коректного сохранения изображения
                scroll_viewer.ScrollToLeftEnd();
                scroll_viewer.ScrollToTop();
                System.Windows.Forms.SaveFileDialog saveFile = new System.Windows.Forms.SaveFileDialog();

                saveFile.Filter = "BMP(*.bmp)|*.bmp|GIF(*.gif)|*.gif|JPEG(*.jpeg, *.jpg)|*.jpeg; *.jpg|PNG(*.png)|*.png|" +
                    "TIFF(*.tiff)|*.tiff|WMP(*.wmp)|*.wmp"; //устанавливаем фильтр форматов
                saveFile.FilterIndex = 3;   //индекс фильтра который будет отображаться по умолчанию
                saveFile.FileName = "Новый рисунок";    //имя файла по умолчанию
                saveFile.InitialDirectory = @"C:\Users\%User%\Desktop"; //расположение по уполчанию
                if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    BitmapEncoder encoder = null;
                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);  //создаем растровое изобажение из визуального контента
                    renderBitmap.Render(inkCanvas);
                    string fileExtension = System.IO.Path.GetExtension(saveFile.FileName);  //получаем выбраный формат
                    switch (fileExtension.ToLower())    //определяем класс для кодировки
                    {
                        case ".bmp":
                            encoder = new BmpBitmapEncoder();
                            break;
                        case ".jpg":
                            encoder = new JpegBitmapEncoder();
                            break;
                        case ".jpeg":
                            encoder = new JpegBitmapEncoder();
                            break;
                        case ".png":
                            encoder = new PngBitmapEncoder();
                            break;
                        case ".gif":
                            encoder = new GifBitmapEncoder();
                            break;
                        case ".tiff":
                            encoder = new TiffBitmapEncoder();
                            break;
                        case ".wmp":
                            encoder = new WmpBitmapEncoder();
                            break;
                    }
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));   //задаем кадр изображения
                    using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))  //создаем поток
                    {
                        encoder.Save(fs);   //сохраняем
                    }
                }
            }
            catch (Exception) { }
        }

        private void openBtn_Click(object sender, RoutedEventArgs e)    //открытие из графического файла
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.InitialDirectory = @"C:\Users\%User%\Desktop"; //расположение по уполчанию
            openFile.Filter = "Все файлы изображений|*.bmp; *.jpeg; *jpg; *.png; *.gif; *.tiff; *.wmp;|BMP(*.bmp)|*.bmp|" +
                "GIF(*.gif)|*.gif|JPEG(*.jpeg, *.jpg)|*.jpeg; *.jpg|PNG(*.png)|*.png|TIFF(*.tiff)|*.tiff|WMP(*.wmp)|*.wmp"; //устанавливаем фильтр форматов

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //скролим вверх и влево для коректного отображения изображения после открытия
                scroll_viewer.ScrollToLeftEnd();
                scroll_viewer.ScrollToTop();
                image.Source = new BitmapImage(new Uri(openFile.FileName)); //задаем Source для Image
                //подгоняем размеры области рисования под размеры изображения
                inkCanvas.Height = image.Source.Height;
                inkCanvas.Width = image.Source.Width;
            }
        }

        private void btn_scale_Click(object sender, RoutedEventArgs e)  //масштабирование изображения
        {
            if (image.Source != null)   //проверка на наличие изображения
            {
                Scale scaleWindow = new Scale();    //создание экземпляра 2 окна
                //передача значений размера во 2 окно
                scaleWindow.w = image.Source.Width;
                scaleWindow.h = image.Source.Height;
                //скролим вверх и влево для коректного отображения изображения после открытия
                scroll_viewer.ScrollToLeftEnd();
                scroll_viewer.ScrollToTop();
                scaleWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen; //появление окна 2 по центру экрана
                try
                {
                    if (scaleWindow.ShowDialog() == true)
                    {
                        //получаем BitmapFrame из вызваного метода
                        BitmapFrame bf = CreateResizedImage(image.Source, Convert.ToInt32(scaleWindow.textBox_Width.Text), Convert.ToInt32(scaleWindow.textBox_Height.Text));
                        image.Source = bf;  //задаем соурс для изображения
                        //подгоняем размеры области рисования под размеры изображения
                        inkCanvas.Width = image.Width;
                        inkCanvas.Height = image.Height;
                    }
                }
                catch (Exception) { };
            }
        }

        public static BitmapFrame CreateResizedImage(ImageSource source, int width, int height)    //метод получения BitmapFrame
        {
            var rect = new Rect(0, 0, width, height);   //создаем прямоугольник
            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);   //масшабирование изображения с высоким качеством
            group.Children.Add(new ImageDrawing(source, rect)); //создаем объект Drawing
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawDrawing(group);  //рисуем объект Drawing
            }
            var resizedImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default); //создаем RenderTargetBitmap
            resizedImage.Render(drawingVisual); //отображаем Visual
            return BitmapFrame.Create(resizedImage);    //возвращаем фрейм
        }

        private void btn_crop_Click(object sender, RoutedEventArgs e)   //обрезка изображения
        {
            if (image.Source != null)   //проверка на наличие изображения
            {
                //изменяем размеры изображения до целочисленного представления, что бы не было ошибки выхода за границы интервала при попытке обрезать
                BitmapFrame bf = CreateResizedImage(image.Source, (int)image.ActualWidth, (int)image.ActualHeight);
                image.Source = bf;
                CropWindow cropWindow = new CropWindow();
                cropWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen; //положение 3 окна при открытии
                cropWindow.image_before.Source = image.Source;  //передаем изображение в 3 форму
                //передаем значения размера округляя до целого
                cropWindow.w = Math.Round(image.ActualWidth);
                cropWindow.h = Math.Round(image.ActualHeight);
                if (cropWindow.ShowDialog() == true)
                {
                    image.Source = cropWindow.cb;   //показываем обрезаное изображение
                    //подгоняем размеры области рисования под размеры обрезаного изображения
                    inkCanvas.Height = image.Source.Height;
                    inkCanvas.Width = image.Source.Width;
                }
            }
        }

        private void btn_rotateLeft_Click(object sender, RoutedEventArgs e) //поворот изображения налево
        {
            try
            {
                rotateAngle -= 90;  //угол поворота -90
                image.LayoutTransform = new RotateTransform(rotateAngle);   //задаем поворот для изображения на -90
                if (image.Source.Width != image.Source.Height)  //проверка на соотношение сторон, если не квадрат, то немяем стороны области для рисования местами
                {
                    double tmp; //вспомогательная переменная
                    tmp = inkCanvas.Width;
                    inkCanvas.Width = inkCanvas.Height;
                    inkCanvas.Height = tmp;
                }
            }
            catch (Exception) { }
        }

        private void btn_rotateRight_Click(object sender, RoutedEventArgs e)    //поворот изображения направо
        {
            try
            {
                rotateAngle += 90;  //угол поворота +90
                image.LayoutTransform = new RotateTransform(rotateAngle);   //задаем поворот для изображения на +90
                if (image.Source.Width != image.Source.Height)  //проверка на соотношение сторон, если не квадрат, то немяем стороны области для рисования местами
                {
                    double tmp; //вспомогательная переменная
                    tmp = inkCanvas.Width;
                    inkCanvas.Width = inkCanvas.Height;
                    inkCanvas.Height = tmp;
                }
            }
            catch (Exception) { }
        }

        private void btn_pen_Click(object sender, RoutedEventArgs e)    //выбор режима редактирования
        {
            RadioButton radiobutton = sender as RadioButton;
            string radioBPressed = radiobutton.Content.ToString();  //значение Content от нажатой кнопки
            switch (radioBPressed)
            {
                case "Ручка":
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;   //режима редактирования - рисование
                    break;
                case "Ластик":
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;  //режима редактирования - ластик
                    break;
                case "Выбрать":
                    inkCanvas.EditingMode = InkCanvasEditingMode.Select;    //режима редактирования - выбор
                    break;
            }
        }

        private void btn_thickness_Click(object sender, RoutedEventArgs e)  //выбор толщины пера
        {
            RadioButton radiobutton = sender as RadioButton;
            string radioBPressed = radiobutton.Content.ToString();  //значение Content от нажатой кнопки
            switch (radioBPressed)
            {
                case "2":   //толщина пера - 2
                    strokeAttr.Width = 2;
                    strokeAttr.Height = 2;
                    break;
                case "4":   //толщина пера - 4
                    strokeAttr.Width = 4;
                    strokeAttr.Height = 4;
                    break;
                case "8":   //толщина пера - 8
                    strokeAttr.Width = 8;
                    strokeAttr.Height = 8;
                    break;
                case "16":  //толщина пера - 16
                    strokeAttr.Width = 16;
                    strokeAttr.Height = 16;
                    break;
            }
        }
       
        private void btn_pickColor_Click(object sender, RoutedEventArgs e)  //вызов диалога выбора цвета
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.FullOpen = true; //по умолчанию открываем дополнительную область для выбор цвета в диалоге
            cd.Color = System.Drawing.Color.FromArgb(strokeAttr.Color.A, strokeAttr.Color.R, strokeAttr.Color.G, strokeAttr.Color.B); //выбраный цвет по умолчанию = предыдущий цвет
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SolidColorBrush colorBrush = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));   //получение цвета из диалога
                rect_color.Fill = colorBrush;   //зарисовываем прямоугольник полученым цветом для отображения текущего цвета
                strokeAttr.Color = Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);  //присвоение перу рисования нового цвета
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e) //очистка графических областей
        {
            inkCanvas.Strokes.Clear();  //удаление коллекции Strokes для inkCanvas
            image.Source = null;    //удаление изображения
            inkCanvas.Width = 1765; //первоначальный размер inkCanvas
            inkCanvas.Height = 830;
        }

        private void viewBox_MouseWheel(object sender, MouseWheelEventArgs e)   //зум
        {
            UpdateViewBox((e.Delta > 0) ? 20 : -20);    //вызов метода при каждой прогрутки колёсика
        }

        private void UpdateViewBox(int newValue)    //увеличение/уменьшение размера viewBox(зум)
        {
            if (viewBox.Width >= 0 && viewBox.Height >= 0)  //размер viewBox больше нуля?
            {
                //изменение размера viewBox
                viewBox.Width += newValue;
                viewBox.Height += newValue;
            }
        }
    }
}