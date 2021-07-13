using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    public partial class MainWindow : Window
    {
        //таймер для змейки
        DispatcherTimer timer = new DispatcherTimer();

        //таймер для генерации еды
        DispatcherTimer foodGenerate1 = new DispatcherTimer();
        DispatcherTimer foodGenerate2 = new DispatcherTimer();

        DispatcherTimer writeSnake = new DispatcherTimer();

        //10 скоростей
        TimeSpan[] speeds = new TimeSpan[10];

        //игровое поле
        Rectangle[,] field = new Rectangle[39, 39];

        bool keyEnabled = true;
        Random random = new Random();
        int count = 0;
        Scores scores = new Scores();

        //координаты
        class Position
        {
            int x;
            int y;

            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int X
            {
                get{ return x; }
                set{ x = value; }
            }

            public int Y
            {
                get{ return y; }
                set{ y = value; }
            }
        }
        LinkedList<Position> snake = new LinkedList<Position>();

        //цвета для кнопок
        SolidColorBrush colorEnter = new SolidColorBrush(Color.FromRgb(255, 213, 113));
        SolidColorBrush colorLeave = new SolidColorBrush(Color.FromRgb(81, 152, 114));

        //цвета для змейки
        SolidColorBrush colorHead = new SolidColorBrush(Color.FromRgb(255, 211, 29));
        SolidColorBrush colorBody = new SolidColorBrush(Color.FromRgb(238, 238, 238));

        //цвет еды
        SolidColorBrush colorFood = new SolidColorBrush(Color.FromRgb(168, 223, 101));

        //направление движения змейки
        Key activeKey = new Key();

        public MainWindow()
        {
            //заставка
            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();
            Thread.Sleep(1000);
            splashScreen.Close();

            InitializeComponent();

            scores = scores.Load();

            //создание игрового поля
            for (int x = 0; x < 39; x++)
                for (int y = 0; y < 39; y++)
                {
                    field[x, y] = new Rectangle();
                    field[x, y] = CreateRectangle(x * 10 + 2, y * 10 + 2, null);
                }

            timer.Tag = false;

            for (int i = 0; i < 10; i++)
            {
                speeds[i] = new TimeSpan(0, 0, 0, 0, (10 - i - 1) * 30 + 40);
            }

            //начальные настройки режима скорости
            rctSpeedModeStatic.Tag = true;
            rctSpeedModeStatic.Fill = colorEnter;
            rctSpeedModeDynamic.Tag = false;

            //изменение цвета кнопок при наведении
            foreach (var rectangle in gridControlls.Children.OfType<Rectangle>().Where(rct => rct.Tag == null))
            {
                rectangle.MouseEnter += (s, a) =>
                {
                    rectangle.Fill = colorEnter;
                };
                rectangle.MouseLeave += (s, a) =>
                {
                    rectangle.Fill = colorLeave;
                };
            }

            //изменение цвета кнопок режимов скоростей при наведении
            foreach (var rectangle in gridControlls.Children.OfType<Rectangle>().Where(rct => rct.Tag != null))
            {
                rectangle.MouseEnter += (s, a) =>
                {
                    rectangle.Fill = colorEnter;
                };
                rectangle.MouseLeave += (s, a) =>
                {
                    if ((bool)rectangle.Tag == false)
                        rectangle.Fill = colorLeave;
                };
            }

            Position next = new Position(0, 0);
            Position food = new Position(0, 0);
            //запуск змейки
            timer.Tick += (s, a) =>
            {
                next.X = snake.First().X;
                next.Y = snake.First().Y;
                //перемещение змейки
                switch (activeKey)
                {
                    case Key.Up:
                        if (next.Y > 0) next.Y--;
                        else next.Y = 38;
                        break;
                    case Key.Down:
                        if (next.Y < 38) next.Y++;
                        else next.Y = 0;
                        break;
                    case Key.Left:
                        if (next.X > 0) next.X--;
                        else next.X = 38;
                        break;
                    case Key.Right:
                        if (next.X < 38) next.X++;
                        else next.X = 0;
                        break;
                }

                if (field[next.X, next.Y].Fill == colorBody)
                {
                    Thread.Sleep(2500);
                    GameOver();
                    writeSnake.Start();
                }
                else
                {
                    field[snake.First().X, snake.First().Y].Fill = colorBody;
                    snake.AddFirst(new Position(next.X, next.Y));
                    if (field[next.X, next.Y].Fill != colorFood)
                    {
                        field[snake.Last().X, snake.Last().Y].Fill = null;
                        snake.RemoveLast();
                    }
                    else
                    {
                        lblScore.Content = int.Parse(lblScore.Content.ToString()) + 1;

                        do
                        {
                            food.X = random.Next(0, 39);
                            food.Y = random.Next(0, 39);
                        } while (field[food.X, food.Y].Fill != null);
                        field[food.X, food.Y].Fill = colorFood;

                        if ((bool)rctSpeedModeDynamic.Tag == true)
                        {
                            switch(int.Parse(lblScore.Content.ToString()))
                            {
                                case 1:
                                    timer.Interval = speeds[1];
                                    break;
                                case 2:
                                    timer.Interval = speeds[2];
                                    break;
                                case 5:
                                    timer.Interval = speeds[3];
                                    break;
                                case 8:
                                    timer.Interval = speeds[4];
                                    break;
                                case 12:
                                    timer.Interval = speeds[5];
                                    break;
                                case 20:
                                    timer.Interval = speeds[6];
                                    break;
                                case 30:
                                    timer.Interval = speeds[7];
                                    break;
                                case 40:
                                    timer.Interval = speeds[8];
                                    break;
                                case 50:
                                    timer.Interval = speeds[9];
                                    break;
                            }
                        }
                    }
                    field[next.X, next.Y].Fill = colorHead;
                }
                keyEnabled = true;
            };

            //¯\_(ツ)_/¯
            writeSnake.Interval = speeds[0];
            writeSnake.Tick += (s, a) =>
            {
                switch (count)
                {
                    // S
                    case 0:
                        field[9, 16].Fill = colorHead;
                        break;
                    case 1:
                        field[8, 16].Fill = colorHead;
                        field[9, 16].Fill = colorBody;
                        break;
                    case 2:
                        field[7, 16].Fill = colorHead;
                        field[8, 16].Fill = colorBody;
                        break;
                    case 3:
                        field[6, 16].Fill = colorHead;
                        field[7, 16].Fill = colorBody;
                        break;
                    case 4:
                        field[5, 16].Fill = colorHead;
                        field[6, 16].Fill = colorBody;
                        break;
                    case 5:
                        field[5, 17].Fill = colorHead;
                        field[5, 16].Fill = colorBody;
                        break;
                    case 6:
                        field[5, 18].Fill = colorHead;
                        field[5, 17].Fill = colorBody;
                        break;
                    case 7:
                        field[5, 19].Fill = colorHead;
                        field[5, 18].Fill = colorBody;
                        break;
                    case 8:
                        field[6, 19].Fill = colorHead;
                        field[5, 19].Fill = colorBody;
                        break;
                    case 9:
                        field[7, 19].Fill = colorHead;
                        field[6, 19].Fill = colorBody;
                        break;
                    case 10:
                        field[8, 19].Fill = colorHead;
                        field[7, 19].Fill = colorBody;
                        break;
                    case 11:
                        field[9, 19].Fill = colorHead;
                        field[8, 19].Fill = colorBody;
                        break;
                    case 12:
                        field[9, 20].Fill = colorHead;
                        field[9, 19].Fill = colorBody;
                        break;
                    case 13:
                        field[9, 21].Fill = colorHead;
                        field[9, 20].Fill = colorBody;
                        break;
                    case 14:
                        field[9, 22].Fill = colorHead;
                        field[9, 21].Fill = colorBody;
                        break;
                    case 15:
                        field[8, 22].Fill = colorHead;
                        field[9, 22].Fill = colorBody;
                        break;
                    case 16:
                        field[7, 22].Fill = colorHead;
                        field[8, 22].Fill = colorBody;
                        break;
                    case 17:
                        field[6, 22].Fill = colorHead;
                        field[7, 22].Fill = colorBody;
                        break;
                    case 18:
                        field[5, 22].Fill = colorHead;
                        field[6, 22].Fill = colorBody;
                        break;

                    // N
                    case 19:
                        field[11, 16].Fill = colorHead;
                        field[5, 22].Fill = colorBody;
                        break;
                    case 20:
                        field[11, 17].Fill = colorHead;
                        field[11, 16].Fill = colorBody;
                        break;
                    case 21:
                        field[11, 18].Fill = colorHead;
                        field[11, 17].Fill = colorBody;
                        break;
                    case 22:
                        field[11, 19].Fill = colorHead;
                        field[11, 18].Fill = colorBody;
                        break;
                    case 23:
                        field[11, 20].Fill = colorHead;
                        field[11, 19].Fill = colorBody;
                        break;
                    case 24:
                        field[11, 21].Fill = colorHead;
                        field[11, 20].Fill = colorBody;
                        break;
                    case 25:
                        field[11, 22].Fill = colorHead;
                        field[11, 21].Fill = colorBody;
                        break;
                    case 26:
                        field[15, 22].Fill = colorHead;
                        field[11, 22].Fill = colorBody;
                        break;
                    case 27:
                        field[15, 21].Fill = colorHead;
                        field[15, 22].Fill = colorBody;
                        break;
                    case 28:
                        field[15, 20].Fill = colorHead;
                        field[15, 21].Fill = colorBody;
                        break;
                    case 29:
                        field[15, 19].Fill = colorHead;
                        field[15, 20].Fill = colorBody;
                        break;
                    case 30:
                        field[15, 18].Fill = colorHead;
                        field[15, 19].Fill = colorBody;
                        break;
                    case 31:
                        field[15, 17].Fill = colorHead;
                        field[15, 18].Fill = colorBody;
                        break;
                    case 32:
                        field[15, 16].Fill = colorHead;
                        field[15, 17].Fill = colorBody;
                        break;
                    case 33:
                        field[12, 18].Fill = colorHead;
                        field[15, 16].Fill = colorBody;
                        break;
                    case 34:
                        field[13, 19].Fill = colorHead;
                        field[12, 18].Fill = colorBody;
                        break;
                    case 35:
                        field[14, 20].Fill = colorHead;
                        field[13, 19].Fill = colorBody;
                        break;

                    // A
                    case 36:
                        field[17, 22].Fill = colorHead;
                        field[14, 20].Fill = colorBody;
                        break;
                    case 37:
                        field[17, 21].Fill = colorHead;
                        field[17, 22].Fill = colorBody;
                        break;
                    case 38:
                        field[17, 20].Fill = colorHead;
                        field[17, 21].Fill = colorBody;
                        break;
                    case 39:
                        field[17, 19].Fill = colorHead;
                        field[17, 20].Fill = colorBody;
                        break;
                    case 40:
                        field[18, 19].Fill = colorHead;
                        field[17, 19].Fill = colorBody;
                        break;
                    case 41:
                        field[19, 19].Fill = colorHead;
                        field[18, 19].Fill = colorBody;
                        break;
                    case 42:
                        field[20, 19].Fill = colorHead;
                        field[19, 19].Fill = colorBody;
                        break;
                    case 43:
                        field[21, 19].Fill = colorHead;
                        field[20, 19].Fill = colorBody;
                        break;
                    case 44:
                        field[21, 20].Fill = colorHead;
                        field[21, 19].Fill = colorBody;
                        break;
                    case 45:
                        field[21, 21].Fill = colorHead;
                        field[21, 20].Fill = colorBody;
                        break;
                    case 46:
                        field[21, 22].Fill = colorHead;
                        field[21, 21].Fill = colorBody;
                        break;
                    case 47:
                        field[17, 18].Fill = colorHead;
                        field[21, 22].Fill = colorBody;
                        break;
                    case 48:
                        field[17, 17].Fill = colorHead;
                        field[17, 18].Fill = colorBody;
                        break;
                    case 49:
                        field[18, 16].Fill = colorHead;
                        field[17, 17].Fill = colorBody;
                        break;
                    case 50:
                        field[19, 16].Fill = colorHead;
                        field[18, 16].Fill = colorBody;
                        break;
                    case 51:
                        field[20, 16].Fill = colorHead;
                        field[19, 16].Fill = colorBody;
                        break;
                    case 52:
                        field[21, 17].Fill = colorHead;
                        field[20, 16].Fill = colorBody;
                        break;
                    case 53:
                        field[21, 18].Fill = colorHead;
                        field[21, 17].Fill = colorBody;
                        break;

                    // K
                    case 54:
                        field[23, 16].Fill = colorHead;
                        field[21, 18].Fill = colorBody;
                        break;
                    case 55:
                        field[23, 17].Fill = colorHead;
                        field[23, 16].Fill = colorBody;
                        break;
                    case 56:
                        field[23, 18].Fill = colorHead;
                        field[23, 17].Fill = colorBody;
                        break;
                    case 57:
                        field[23, 19].Fill = colorHead;
                        field[23, 18].Fill = colorBody;
                        break;
                    case 58:
                        field[23, 20].Fill = colorHead;
                        field[23, 19].Fill = colorBody;
                        break;
                    case 59:
                        field[23, 21].Fill = colorHead;
                        field[23, 20].Fill = colorBody;
                        break;
                    case 60:
                        field[23, 22].Fill = colorHead;
                        field[23, 21].Fill = colorBody;
                        break;
                    case 61:
                        field[27, 16].Fill = colorHead;
                        field[23, 22].Fill = colorBody;
                        break;
                    case 62:
                        field[26, 17].Fill = colorHead;
                        field[27, 16].Fill = colorBody;
                        break;
                    case 63:
                        field[25, 18].Fill = colorHead;
                        field[26, 17].Fill = colorBody;
                        break;
                    case 64:
                        field[24, 19].Fill = colorHead;
                        field[25, 18].Fill = colorBody;
                        break;
                    case 65:
                        field[25, 20].Fill = colorHead;
                        field[24, 19].Fill = colorBody;
                        break;
                    case 66:
                        field[26, 21].Fill = colorHead;
                        field[25, 20].Fill = colorBody;
                        break;
                    case 67:
                        field[27, 22].Fill = colorHead;
                        field[26, 21].Fill = colorBody;
                        break;

                    // E
                    case 68:
                        field[33, 19].Fill = colorHead;
                        field[27, 22].Fill = colorBody;
                        break;
                    case 69:
                        field[32, 19].Fill = colorHead;
                        field[33, 19].Fill = colorBody;
                        break;
                    case 70:
                        field[31, 19].Fill = colorHead;
                        field[32, 19].Fill = colorBody;
                        break;
                    case 71:
                        field[30, 19].Fill = colorHead;
                        field[31, 19].Fill = colorBody;
                        break;
                    case 72:
                        field[29, 19].Fill = colorHead;
                        field[30, 19].Fill = colorBody;
                        break;
                    case 73:
                        field[29, 18].Fill = colorHead;
                        field[29, 20].Fill = colorHead;

                        field[29, 19].Fill = colorBody;
                        break;
                    case 74:
                        field[29, 17].Fill = colorHead;
                        field[29, 18].Fill = colorBody;

                        field[29, 21].Fill = colorHead;
                        field[29, 20].Fill = colorBody;
                        break;
                    case 75:
                        field[29, 16].Fill = colorHead;
                        field[29, 17].Fill = colorBody;

                        field[29, 22].Fill = colorHead;
                        field[29, 21].Fill = colorBody;
                        break;
                    case 76:
                        field[30, 16].Fill = colorHead;
                        field[29, 16].Fill = colorBody;

                        field[30, 22].Fill = colorHead;
                        field[29, 22].Fill = colorBody;
                        break;
                    case 77:
                        field[31, 16].Fill = colorHead;
                        field[30, 16].Fill = colorBody;

                        field[31, 22].Fill = colorHead;
                        field[30, 22].Fill = colorBody;
                        break;
                    case 78:
                        field[32, 16].Fill = colorHead;
                        field[31, 16].Fill = colorBody;

                        field[32, 22].Fill = colorHead;
                        field[31, 22].Fill = colorBody;
                        break;
                    case 79:
                        field[33, 16].Fill = colorHead;
                        field[32, 16].Fill = colorBody;

                        field[33, 22].Fill = colorHead;
                        field[32, 22].Fill = colorBody;
                        break;
                    case 80:
                        field[33, 16].Fill = colorBody;
                        field[33, 22].Fill = colorBody;
                        break;
                    case 100:
                        writeSnake.Stop();
                        GameOver();
                        writeSnake.Start();
                        break;
                }
                count++;
            };

            writeSnake.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //запуск игры
            if ((e.Key == Key.Enter || e.Key == Key.Space) && (bool)timer.Tag == false)
            {
                if ((bool)rctSpeedModeStatic.Tag == true)
                    timer.Interval = speeds[int.Parse(lblSpeed.Content.ToString()) - 1];
                else
                    timer.Interval = speeds[0];

                writeSnake.Stop();
                GameOver();
                snake.AddFirst(new Position(19, 19));
                field[snake.First().X, snake.First().Y].Fill = colorHead;
                timer.Tag = true;
                field[random.Next(0, 39), random.Next(0, 39)].Fill = colorFood;
                field[random.Next(0, 39), random.Next(0, 39)].Fill = colorFood;
                field[random.Next(0, 39), random.Next(0, 39)].Fill = colorFood;
                timer.Start();
            }

            //управление змейкой
            if (((e.Key == Key.Up && activeKey != Key.Down) ||
                (e.Key == Key.Down && activeKey != Key.Up) ||
                (e.Key == Key.Left && activeKey != Key.Right) ||
                (e.Key == Key.Right && activeKey != Key.Left))
                && keyEnabled == true)
            {
                activeKey = e.Key;
                keyEnabled = false;
            }

            //завершить игру
            if(e.Key == Key.Escape)
            {
                GameOver();
                writeSnake.Start();
            }
        }

        //создание прямоугольника
        // (201; 201) - середина
        private Rectangle CreateRectangle(int x, int y, SolidColorBrush color)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Fill = color;
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            rectangle.Margin = new Thickness(x, y, 0, 0);
            rectangle.Height = 9;
            rectangle.Width = 9;
            gridField.Children.Add(rectangle);

            rectangle.MouseLeftButtonDown += (s, a) =>
            {
                rectangle.Fill = colorBody;
            };
            rectangle.MouseRightButtonDown += (s, a) =>
            {
                rectangle.Fill = null;
            };

            return rectangle;
        }

        private void GameOver()
        {
            timer.Stop();
            foodGenerate1.Stop();
            foodGenerate2.Stop();
            timer.Tag = false;
            foreach (Rectangle rct in gridField.Children)
            {
                rct.Fill = null;
            }
            snake.Clear();
            if(int.Parse(lblBestScore.Content.ToString()) < int.Parse(lblScore.Content.ToString()))
            {
                lblBestScore.Content = int.Parse(lblScore.Content.ToString());
                if((bool)rctSpeedModeStatic.Tag == true)
                    scores.staticSpeed[int.Parse(lblSpeed.Content.ToString()) - 1] = int.Parse(lblScore.Content.ToString());
                else
                    scores.dynamicSpeed = int.Parse(lblScore.Content.ToString());
            }  
            lblScore.Content = 0;
            activeKey = Key.A;
            count = 0;
        }

        //уменьшение скорости
        private void rctSpeedMinus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (int.Parse(lblSpeed.Content.ToString()) != 1)
            {
                lblSpeed.Content = int.Parse(lblSpeed.Content.ToString()) - 1;
                lblBestScore.Content = scores.staticSpeed[int.Parse(lblSpeed.Content.ToString()) - 1];
                writeSnake.Interval = speeds[int.Parse(lblSpeed.Content.ToString()) - 1];
            }
        }

        //увеличение скорости
        private void rctSpeedPlus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (int.Parse(lblSpeed.Content.ToString()) != 10)
            {
                lblSpeed.Content = int.Parse(lblSpeed.Content.ToString()) + 1;
                lblBestScore.Content = scores.staticSpeed[int.Parse(lblSpeed.Content.ToString()) - 1];
                writeSnake.Interval = speeds[int.Parse(lblSpeed.Content.ToString()) - 1];
            }   
        }

        //режим без изменения скорости
        private void rctSpeedModeStatic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rctSpeedModeStatic.Tag = true;
            rctSpeedModeDynamic.Tag = false;
            rctSpeedModeStatic.Fill = colorEnter;
            rctSpeedModeDynamic.Fill = colorLeave;

            lblBestScore.Content = scores.staticSpeed[int.Parse(lblSpeed.Content.ToString()) - 1];
        }

        //режим с возрастанием скорости
        private void rctSpeedModeDynamic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rctSpeedModeDynamic.Tag = true;
            rctSpeedModeStatic.Tag = false;
            rctSpeedModeDynamic.Fill = colorEnter;
            rctSpeedModeStatic.Fill = colorLeave;

            lblBestScore.Content = scores.dynamicSpeed;
        }

        //свернуть окно
        private void rctMinimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        //закрыть окно
        private void rctClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            scores.Save(scores);
            Close();
        }

        //перетаскивание окна
        private void gridControlls_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }
    }
}
