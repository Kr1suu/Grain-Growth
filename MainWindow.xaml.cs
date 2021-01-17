using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Windows.Shapes;
using System.Timers;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;

using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Linq;

namespace Rozrost_Ziarna
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ExecuteClick(object sender, RoutedEventArgs e)
        {
            Program program = new Program();
            Container.IsStructure = this.FindName("IsStructure") as CheckBox;
            TextBlock StructureWork = this.FindName("StructureWork") as TextBlock;
            TextBox SubstractSeed = this.FindName("SubstractSeed") as TextBox;
            Container.StructureSeedSel = Int32.Parse(SubstractSeed.Text);

            TextBox numberTextBoxY = this.FindName("NumberTextBoxY") as TextBox;
            TextBox numberTextBoxX = this.FindName("NumberTextBoxX") as TextBox;
            TextBox speed = this.FindName("Speed") as TextBox;
            TextBox Seedbox = this.FindName("Seed") as TextBox;
            TextBox Random = this.FindName("Random") as TextBox;
            TextBox InclusionCount = this.FindName("InclusionCount") as TextBox;
            TextBox InclusionSize = this.FindName("InclusionSize") as TextBox;
            Container.type = this.FindName("Type") as ComboBox;
            Container.IsInclusion = this.FindName("ISInclusion") as CheckBox;

            Container.random = Int32.Parse(Random.Text);
            Container.seed = Int32.Parse(Seedbox.Text);

            Container.IsWalls = this.FindName("IsWalls") as CheckBox;
            Container.IsAll = this.FindName("IsAll") as CheckBox;
            TextBox SeedWall = this.FindName("SeedWall") as TextBox;
            Container.SeedWall = Int32.Parse(SeedWall.Text);


            if (Container.SeedWall > Container.seed)
            {
                Container.SeedWall = Container.seed;
            }
            if (Container.SeedWall < 2)
            {
                Container.SeedWall = 2;
            }


            if ((bool)Container.IsStructure.IsChecked && Container.Import == true)
            {

            }
            else
            {
                Container.sizeY = Int32.Parse(numberTextBoxY.Text);
                Container.sizeX = Int32.Parse(numberTextBoxX.Text);
            }

            Container.seedid = 1;




            Container.cells = new Cell[Container.sizeX, Container.sizeY];

            Container.ms = Int32.Parse(speed.Text);


            for (int x = 0; x < Container.sizeX; x++)
            {
                for (int y = 0; y < Container.sizeY; y++)
                {
                    Container.cells[x, y] = new Cell(x, y, 0, false);
                }
            }

            Window GridWindow = new Window();
            GridWindow.Title = "Grid Sample";

            Container.myGrid = new Grid();
            CheckBox ShowGrid = this.FindName("ShowGrid") as CheckBox;

            if ((bool)ShowGrid.IsChecked)
            {
                Container.myGrid.ShowGridLines = true;
            }
            else
            {
                Container.myGrid.ShowGridLines = false;
            }


            ColumnDefinition[] columns = new ColumnDefinition[Container.sizeX];
            for (int x = 0; x < Container.sizeX; x++)
            {
                columns[x] = new ColumnDefinition();
                Container.myGrid.ColumnDefinitions.Add(columns[x]);
            }

            RowDefinition[] rows = new RowDefinition[Container.sizeY];
            for (int y = 0; y < Container.sizeY; y++)
            {
                rows[y] = new RowDefinition();
                Container.myGrid.RowDefinitions.Add(rows[y]);
            }

            GridWindow.Content = Container.myGrid;
            GridWindow.Show();

            if ((bool)Container.IsStructure.IsChecked && Container.Import == true)
            {
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.importCell[x, y].seed == Container.StructureSeedSel)
                        {
                            Container.cells[x, y].seed = 1;
                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[Container.StructureSeedSel]));
                            rec.Fill = brush;
                            Grid.SetRow(rec, y);
                            Grid.SetColumn(rec, x);
                            Container.myGrid.Children.Add(rec);
                        }
                    }
                }
            }


            if ((bool)Container.IsInclusion.IsChecked)
            {
                TextBox C = this.FindName("InclusionCount") as TextBox;
                TextBox S = this.FindName("InclusionSize") as TextBox;
                Container.InclusionCount = byte.Parse(C.Text);
                Container.InclusionSize = byte.Parse(S.Text);
                Program.Inclusion();
            }

            Program.StarterSet();
        }



        private void Import_Click(object sender, RoutedEventArgs e)
        {
            Container.IsStructure = this.FindName("IsStructure") as CheckBox;
            TextBlock StructureWork = this.FindName("StructureWork") as TextBlock;
            TextBox SubstractSeed = this.FindName("SubstractSeed") as TextBox;
            Container.StructureSeedSel = Int32.Parse(SubstractSeed.Text);
            int seedmax = 0;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();




            if (result == true)
            {
                StructureWork.Text = "Yes";
                Container.Import = true;
                string filename = dlg.FileName;

                string[] lines = System.IO.File.ReadAllLines(filename);

                Window GridWindow = new Window();
                GridWindow.Title = "Grid Sample";

                Grid myGrid = new Grid();
                if ((bool)ShowGrid.IsChecked)
                {
                    myGrid.ShowGridLines = true;
                }
                else
                {
                    myGrid.ShowGridLines = false;
                }

                string[] words = lines[0].Split(' ');
                int sizeX = Int32.Parse(words[0]);
                int sizeY = Int32.Parse(words[1]);


                ColumnDefinition[] columns = new ColumnDefinition[sizeX];

                for (int x = 0; x < sizeX; x++)
                {
                    columns[x] = new ColumnDefinition();
                    myGrid.ColumnDefinitions.Add(columns[x]);
                }

                RowDefinition[] rows = new RowDefinition[sizeY];
                for (int y = 0; y < sizeY; y++)
                {
                    rows[y] = new RowDefinition();
                    myGrid.RowDefinitions.Add(rows[y]);
                }

                Container.importCell = new Cell[sizeX, sizeY];

                int length = lines.Length;
                int[,] wordsLine = new int[length, 3];


                for (int i = 1; i < lines.Length; i++)
                {
                    string[] words1 = lines[i].Split(' ');
                    wordsLine[i - 1, 0] = Int32.Parse(words1[0]);
                    wordsLine[i - 1, 1] = Int32.Parse(words1[1]);
                    wordsLine[i - 1, 2] = Int32.Parse(words1[2]);
                    if (wordsLine[i - 1, 2] > seedmax)
                    {
                        seedmax = wordsLine[i - 1, 2];
                    }

                }
                Container.sizeY = sizeY;
                Container.sizeX = sizeX;

                if (Container.StructureSeedSel > seedmax)
                {
                    Container.StructureSeedSel = seedmax;
                }
                if (Container.StructureSeedSel < 2)
                {
                    Container.StructureSeedSel = 2;
                }


                for (int i = 0; i < sizeX * sizeY; i++)
                {
                    if (wordsLine[i, 2] == Container.StructureSeedSel)
                    {
                        Container.importCell[wordsLine[i, 0], wordsLine[i, 1]].seed = Container.StructureSeedSel;
                    }
                }

                for (int i = 0; i < sizeX * sizeY; i++)
                {
                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[wordsLine[i, 2]]));
                    rec.Fill = brush;
                    Grid.SetRow(rec, wordsLine[i, 1]);
                    Grid.SetColumn(rec, wordsLine[i, 0]);
                    myGrid.Children.Add(rec);
                }



                GridWindow.Content = myGrid;
                GridWindow.Show();


            }

        }
    }



    public class Program
    {

        public static void Inclusion()
        {
            int randY = 0;
            int randX = 0;
            for (int i = 0; i < Container.InclusionCount; i++)
            {
                randY = Container.rnd.Next(0, Container.sizeY - (Container.InclusionSize));
                randX = Container.rnd.Next(0, Container.sizeX - (Container.InclusionSize));
                for (int x = 0; x < Container.InclusionSize; x++)
                {
                    for (int y = 0; y < Container.InclusionSize; y++)
                    {
                        Container.cells[x + randX, y + randY] = new Cell(x + randX, y + randY, 1, false);
                        System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                        System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                        rec.Fill = brush;
                        Grid.SetRow(rec, y + randY);
                        Grid.SetColumn(rec, x + randX);
                        Container.myGrid.Children.Add(rec);
                    }
                }
            }

        }

        public static async Task StarterSet()
        {
            int randY;
            int randX;
            Cell[] created = new Cell[Container.seed];
            bool a = true;
            byte seedid = 2;

            for (int i = 0; i < Container.seed; i++)
            {
                await Task.Delay(1000);
                while (true)
                {
                    randY = Container.rnd.Next(0, Container.sizeY - 1);
                    randX = Container.rnd.Next(0, Container.sizeX - 1);

                    if (Container.cells[randX, randY].seed == 0)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                Container.cells[(uint)randX, (uint)randY] = new Cell(randX, randY, (short)(seedid), true);
                System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[seedid]));
                rec.Fill = brush;
                Grid.SetRow(rec, randY);
                Grid.SetColumn(rec, randX);
                Container.myGrid.Children.Add(rec);

                seedid++;
            }

            /* while (true)
             {
                 await Task.Delay(1000);
                 int randY = Container.rnd.Next(0, Container.sizeY - 1);
                 int randX = Container.rnd.Next(0, Container.sizeX - 1);
                 for (int k = 0; k < created.Length; k++)
                 {
                     if (created[k].seed == 0)
                     {
                         break;
                     }
                     if (randY == created[k].posY && randX == created[k].posX)
                     {
                         a = true;
                     }
                 }
                 if (a)
                 {
                     continue;
                 }
                 created[seedcount] = new Cell(randX, randY, (short)Container.seedid, true);
                 Container.cells[(uint)randX, (uint)randY] = new Cell(randX, randY, (short)(Container.seedid), true);
                 seedcount++;
                 counter++;

                 System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                 System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[Container.cells[randX, randY].seed]));
                 rec.Fill = brush;
                 Grid.SetRow(rec, randY);
                 Grid.SetColumn(rec, randX);
                 Container.myGrid.Children.Add(rec);

                 Container.seedid++;
                 if (counter == Container.seed)
                 {
                     break;
                 }

             }*/

            if (Container.type.Text.ToString() == "Von Newman")
            {
                Program.VNSpreed();
            }
            else if (Container.type.Text.ToString() == "Shape control")
            {
                Program.SCSpreed();
            }

        }

        public static async Task VNSpreed()
        {
            int blanks = 1;
            while (blanks != 0)
            {
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {

                        if (Container.cells[x, y].seed == 0)
                        {

                            int count = 0;
                            byte[] types = { 0, 0, 0, 0 };
                            byte[] cases = { 0, 0, 0, 0 };
                            byte a = 0;
                            if (y > 0)
                            {
                                if (Container.cells[x, y - 1].seed != 0 && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y - 1].spread == true)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x, y - 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x, y - 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (y < Container.sizeY - 1)
                            {
                                if (Container.cells[x, y + 1].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y + 1].spread == true)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x, y + 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x, y + 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (x > 0)
                            {
                                if (Container.cells[x - 1, y].seed != 0 && Container.cells[x - 1, y].seed != 1 && Container.cells[x - 1, y].spread == true)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x - 1, y].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x - 1, y].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (x < Container.sizeX - 1)
                            {
                                if (Container.cells[x + 1, y].seed != 0 && Container.cells[x + 1, y].seed != 1 && Container.cells[x + 1, y].spread == true)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x + 1, y].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x + 1, y].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                            }

                            byte sed = 0;
                            if (count == 1)
                            {
                                sed = types[0];
                            }
                            else if (count >= 2)
                            {
                                byte maxValue = cases.Max();
                                int maxIndex = cases.ToList().IndexOf(maxValue);
                                sed = types[maxIndex];
                            }
                            /*switch (count)
                            {
                                case 1:
                                    sed = types[0];
                                    break;
                                case 2:
                                    int rand1 = Container.rnd.Next(0, 1);
                                    sed = cases[rand1];
                                    break;
                                case 3:
                                    int rand2 = Container.rnd.Next(0, 2);
                                    sed = cases[rand2];
                                    break;
                                case 4:
                                    int rand3 = Container.rnd.Next(0, 3);
                                    sed = cases[rand3];
                                    break;
                            }*/
                            if (count > 0)
                            {

                                Container.cells[x, y] = new Cell(x, y, sed, false);
                                System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                rec.Fill = brush;
                                Grid.SetRow(rec, y);
                                Grid.SetColumn(rec, x);
                                Container.myGrid.Children.Add(rec);
                                await Task.Delay(Container.ms);
                            }

                        }

                    }
                }
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed != 0)
                        {
                            Container.cells[x, y].spread = true;
                        }

                    }
                }
                blanks = 0;
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed == 0)
                        {
                            blanks++;
                        }

                    }
                }
            }
            if ((bool)Container.IsWalls.IsChecked)
            {
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed == 1)
                        {
                            Container.cells[x, y].seed = 0;
                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[0]));
                            rec.Fill = brush;
                            Grid.SetRow(rec, y);
                            Grid.SetColumn(rec, x);
                            Container.myGrid.Children.Add(rec);
                            await Task.Delay(Container.ms);
                        }

                    }
                }
                if ((bool)Container.IsAll.IsChecked)
                {
                    for (int x = 0; x < Container.sizeX; x++)
                    {
                        for (int y = 0; y < Container.sizeY; y++)
                        {
                            byte seed;
                            if (y > 0)
                            {
                                if (Container.cells[x, y - 1].seed != Container.cells[x, y].seed && Container.cells[x, y - 1].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (y < Container.sizeY - 1)
                            {
                                if (Container.cells[x, y + 1].seed != Container.cells[x, y].seed && Container.cells[x, y + 1].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                            if (x > 0)
                            {
                                if (Container.cells[x - 1, y].seed != Container.cells[x, y].seed && Container.cells[x - 1, y].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (x < Container.sizeX - 1)
                            {
                                if (Container.cells[x + 1, y].seed != Container.cells[x, y].seed && Container.cells[x + 1, y].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                        }
                    }
                }
                else
                {
                    for (int x = 0; x < Container.sizeX; x++)
                    {
                        for (int y = 0; y < Container.sizeY; y++)
                        {
                            byte seed;
                            if (y > 0)
                            {
                                if (Container.cells[x, y - 1].seed != Container.cells[x, y].seed && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (y < Container.sizeY - 1)
                            {
                                if (Container.cells[x, y + 1].seed != Container.cells[x, y].seed && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                            if (x > 0)
                            {
                                if (Container.cells[x - 1, y].seed != Container.cells[x, y].seed && Container.cells[x - 1, y].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (x < Container.sizeX - 1)
                            {
                                if (Container.cells[x + 1, y].seed != Container.cells[x, y].seed && Container.cells[x + 1, y].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                        }
                    }
                }
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed != 1)
                        {
                            Container.cells[x, y].seed = 0;
                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[0]));
                            rec.Fill = brush;
                            Grid.SetRow(rec, y);
                            Grid.SetColumn(rec, x);
                            Container.myGrid.Children.Add(rec);
                            await Task.Delay(Container.ms);
                        }

                    }
                }
            }

            MessageBoxResult result = MessageBox.Show("Koniec. Czy exportować", "End", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Export();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("No to nie", "My App");
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        public static async Task SCSpreed()
        {
            int blanks = 1;
            while (blanks != 0)
            {
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {

                        if (Container.cells[x, y].seed == 0)
                        {


                            byte a = 0;
                            byte sed = 0;
                            bool check1 = false;
                            bool check2 = false;
                            bool check3 = false;
                            Random rand = new Random();

                            if (y > 0 && y < Container.sizeY - 1 && x > 0 && x < Container.sizeX - 1) //Rule #1
                            {
                                int count = 0;
                                byte[] types = { 0, 0, 0, 0, 0, 0, 0, 0 };
                                byte[] cases = { 0, 0, 0, 0, 0, 0, 0, 0 };
                                if (Container.cells[x, y - 1].seed != 0 && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y - 1].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x, y - 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x, y - 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                                if (Container.cells[x, y + 1].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y + 1].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x, y + 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x, y + 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                                if (Container.cells[x - 1, y].seed != 0 && Container.cells[x - 1, y].seed != 1 && Container.cells[x - 1, y].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x - 1, y].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x - 1, y].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                                if (Container.cells[x + 1, y].seed != 0 && Container.cells[x + 1, y].seed != 1 && Container.cells[x + 1, y].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x + 1, y].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x + 1, y].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                                if (Container.cells[x - 1, y - 1].seed != 0 && Container.cells[x - 1, y - 1].seed != 1 && Container.cells[x - 1, y - 1].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x - 1, y - 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x - 1, y - 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                                if (Container.cells[x - 1, y + 1].seed != 0 && Container.cells[x - 1, y + 1].seed != 1 && Container.cells[x - 1, y + 1].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x - 1, y + 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x - 1, y + 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                                if (Container.cells[x + 1, y + 1].seed != 0 && Container.cells[x + 1, y + 1].seed != 1 && Container.cells[x + 1, y + 1].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x + 1, y + 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x + 1, y + 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }
                                if (Container.cells[x + 1, y - 1].seed != 0 && Container.cells[x + 1, y - 1].seed != 1 && Container.cells[x + 1, y - 1].spread == true)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        if (types[i] == 0)
                                        {
                                            types[i] = (byte)Container.cells[x + 1, y - 1].seed;
                                            cases[i]++;
                                            count++;
                                            break;
                                        }
                                        else if (types[i] == (byte)Container.cells[x + 1, y - 1].seed)
                                        {
                                            cases[i]++;
                                            break;
                                        }
                                    }
                                }


                                for (int i = 0; i < 8; i++)
                                {
                                    if (cases[i] >= 5)
                                    {
                                        check1 = true;
                                        sed = types[i];
                                        break;
                                    }
                                }
                                if (check1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, sed, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                    continue;
                                }

                            }
                            if (!check1) //Rule 2
                            {
                                if (y > 0 && x > 0 && x < Container.sizeX - 1)
                                {
                                    if ((Container.cells[x, y - 1].seed != 0 && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y - 1].spread == true)
                                        && (Container.cells[x - 1, y].seed != 0 && Container.cells[x - 1, y].seed != 1 && Container.cells[x - 1, y].spread == true)
                                        && (Container.cells[x + 1, y].seed != 0 && Container.cells[x + 1, y].seed != 1 && Container.cells[x + 1, y].spread == true)) //1
                                    {
                                        if (Container.cells[x, y - 1].seed == Container.cells[x - 1, y].seed && Container.cells[x, y - 1].seed == Container.cells[x + 1, y].seed)
                                        {
                                            check2 = true;
                                            sed = (byte)Container.cells[x, y - 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }
                                }

                                if (y < Container.sizeY - 1 && x > 0 && x < Container.sizeX - 1)
                                {
                                    if ((Container.cells[x, y + 1].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y + 1].spread == true)
                                        && (Container.cells[x - 1, y].seed != 0 && Container.cells[x - 1, y].seed != 1 && Container.cells[x - 1, y].spread == true)
                                        && (Container.cells[x + 1, y].seed != 0 && Container.cells[x + 1, y].seed != 1 && Container.cells[x + 1, y].spread == true)) //2
                                    {
                                        if (Container.cells[x, y + 1].seed == Container.cells[x - 1, y].seed && Container.cells[x, y + 1].seed == Container.cells[x + 1, y].seed)
                                        {
                                            check2 = true;
                                            sed = (byte)Container.cells[x, y + 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }
                                }

                                if (y > 0 && y < Container.sizeY - 1 && x > 0)
                                {
                                    if ((Container.cells[x, y + 1].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y + 1].spread == true)
                                        && (Container.cells[x - 1, y].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x - 1, y].spread == true)
                                        && (Container.cells[x, y - 1].seed != 0 && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y - 1].spread == true)) //3
                                    {
                                        if (Container.cells[x, y + 1].seed == Container.cells[x - 1, y].seed && Container.cells[x, y + 1].seed == Container.cells[x, y - 1].seed)
                                        {
                                            check2 = true;
                                            sed = (byte)Container.cells[x, y + 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }
                                }

                                if (y > 0 && y < Container.sizeY - 1 && x < Container.sizeX - 1)
                                {
                                    if ((Container.cells[x, y + 1].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y + 1].spread == true)
                                        && (Container.cells[x + 1, y].seed != 0 && Container.cells[x + 1, y].seed != 1 && Container.cells[x + 1, y].spread == true)
                                        && (Container.cells[x, y - 1].seed != 0 && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y - 1].spread == true)) //4
                                    {
                                        if (Container.cells[x, y + 1].seed == Container.cells[x + 1, y].seed && Container.cells[x, y + 1].seed == Container.cells[x, y - 1].seed)
                                        {
                                            check2 = true;
                                            sed = (byte)Container.cells[x, y + 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (!check2) //Rule 3
                            {
                                if (y > 0 && y < Container.sizeY - 1 && x > 0 && x < Container.sizeX - 1)
                                {

                                    if ((Container.cells[x - 1, y - 1].seed != 0 && Container.cells[x - 1, y - 1].seed != 1 && Container.cells[x - 1, y - 1].spread == true)
                                    && (Container.cells[x + 1, y - 1].seed != 0 && Container.cells[x + 1, y - 1].seed != 1 && Container.cells[x + 1, y - 1].spread == true)
                                    && (Container.cells[x - 1, y + 1].seed != 0 && Container.cells[x - 1, y + 1].seed != 1 && Container.cells[x - 1, y + 1].spread == true)) //1
                                    {
                                        if (Container.cells[x - 1, y - 1].seed == Container.cells[x + 1, y - 1].seed && Container.cells[x - 1, y - 1].seed == Container.cells[x - 1, y + 1].seed)
                                        {
                                            check3 = true;
                                            sed = (byte)Container.cells[x - 1, y - 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }


                                    else if ((Container.cells[x - 1, y - 1].seed != 0 && Container.cells[x - 1, y - 1].seed != 1 && Container.cells[x - 1, y - 1].spread == true)
                                        && (Container.cells[x + 1, y - 1].seed != 0 && Container.cells[x + 1, y - 1].seed != 1 && Container.cells[x + 1, y - 1].spread == true)
                                        && (Container.cells[x + 1, y + 1].seed != 0 && Container.cells[x + 1, y + 1].seed != 1 && Container.cells[x + 1, y + 1].spread == true)) //2
                                    {
                                        if (Container.cells[x - 1, y - 1].seed == Container.cells[x + 1, y - 1].seed && Container.cells[x - 1, y - 1].seed == Container.cells[x + 1, y + 1].seed)
                                        {
                                            check3 = true;
                                            sed = (byte)Container.cells[x - 1, y - 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }

                                    else if ((Container.cells[x - 1, y - 1].seed != 0 && Container.cells[x - 1, y - 1].seed != 1 && Container.cells[x - 1, y - 1].spread == true)
                                        && (Container.cells[x - 1, y + 1].seed != 0 && Container.cells[x - 1, y + 1].seed != 1 && Container.cells[x - 1, y + 1].spread == true)
                                        && (Container.cells[x + 1, y + 1].seed != 0 && Container.cells[x + 1, y + 1].seed != 1 && Container.cells[x + 1, y + 1].spread == true)) //3
                                    {
                                        if (Container.cells[x - 1, y - 1].seed == Container.cells[x - 1, y + 1].seed && Container.cells[x - 1, y - 1].seed == Container.cells[x + 1, y + 1].seed)
                                        {
                                            check3 = true;
                                            sed = (byte)Container.cells[x - 1, y - 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }
                                    else if ((Container.cells[x + 1, y - 1].seed != 0 && Container.cells[x + 1, y - 1].seed != 1 && Container.cells[x + 1, y - 1].spread == true)
                                        && (Container.cells[x - 1, y + 1].seed != 0 && Container.cells[x - 1, y + 1].seed != 1 && Container.cells[x - 1, y + 1].spread == true)
                                        && (Container.cells[x + 1, y + 1].seed != 0 && Container.cells[x + 1, y + 1].seed != 1 && Container.cells[x + 1, y + 1].spread == true)) //4
                                    {
                                        if (Container.cells[x - 1, y - 1].seed == Container.cells[x - 1, y + 1].seed && Container.cells[x - 1, y - 1].seed == Container.cells[x + 1, y + 1].seed)
                                        {
                                            check3 = true;
                                            sed = (byte)Container.cells[x + 1, y - 1].seed;
                                            Container.cells[x, y] = new Cell(x, y, sed, false);
                                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                            rec.Fill = brush;
                                            Grid.SetRow(rec, y);
                                            Grid.SetColumn(rec, x);
                                            Container.myGrid.Children.Add(rec);
                                            await Task.Delay(Container.ms);
                                            continue;
                                        }
                                    }

                                }
                            }
                            if (!check3) //Rule4
                            {

                                if (Container.random > rand.Next(0, 100))
                                {
                                    continue;

                                }
                                else
                                {
                                    int count = 0;
                                    byte[] types = { 0, 0, 0, 0, 0, 0, 0, 0 };
                                    byte[] cases = { 0, 0, 0, 0, 0, 0, 0, 0 };
                                    if (y > 0)
                                    {
                                        if (Container.cells[x, y - 1].seed != 0 && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y - 1].spread == true)
                                        {
                                            for (int i = 0; i < 8; i++)
                                            {
                                                if (types[i] == 0)
                                                {
                                                    types[i] = (byte)Container.cells[x, y - 1].seed;
                                                    cases[i]++;
                                                    count++;
                                                    break;
                                                }
                                                else if (types[i] == (byte)Container.cells[x, y - 1].seed)
                                                {
                                                    cases[i]++;
                                                    break;
                                                }
                                            }
                                        }
                                        if (x > 0)
                                        {
                                            if (Container.cells[x - 1, y - 1].seed != 0 && Container.cells[x - 1, y - 1].seed != 1 && Container.cells[x - 1, y - 1].spread == true)
                                            {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                    if (types[i] == 0)
                                                    {
                                                        types[i] = (byte)Container.cells[x - 1, y - 1].seed;
                                                        cases[i]++;
                                                        count++;
                                                        break;
                                                    }
                                                    else if (types[i] == (byte)Container.cells[x - 1, y - 1].seed)
                                                    {
                                                        cases[i]++;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (x < Container.sizeX - 1)
                                        {
                                            if (Container.cells[x + 1, y - 1].seed != 0 && Container.cells[x + 1, y - 1].seed != 1 && Container.cells[x + 1, y - 1].spread == true)
                                            {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                    if (types[i] == 0)
                                                    {
                                                        types[i] = (byte)Container.cells[x + 1, y - 1].seed;
                                                        cases[i]++;
                                                        count++;
                                                        break;
                                                    }
                                                    else if (types[i] == (byte)Container.cells[x + 1, y - 1].seed)
                                                    {
                                                        cases[i]++;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (y < Container.sizeY - 1)
                                    {
                                        if (Container.cells[x, y + 1].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y + 1].spread == true)
                                        {
                                            for (int i = 0; i < 8; i++)
                                            {
                                                if (types[i] == 0)
                                                {
                                                    types[i] = (byte)Container.cells[x, y + 1].seed;
                                                    cases[i]++;
                                                    count++;
                                                    break;
                                                }
                                                else if (types[i] == (byte)Container.cells[x, y + 1].seed)
                                                {
                                                    cases[i]++;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (x > 0)
                                    {
                                        if (Container.cells[x - 1, y].seed != 0 && Container.cells[x - 1, y].seed != 1 && Container.cells[x - 1, y].spread == true)
                                        {
                                            for (int i = 0; i < 8; i++)
                                            {
                                                if (types[i] == 0)
                                                {
                                                    types[i] = (byte)Container.cells[x - 1, y].seed;
                                                    cases[i]++;
                                                    count++;
                                                    break;
                                                }
                                                else if (types[i] == (byte)Container.cells[x - 1, y].seed)
                                                {
                                                    cases[i]++;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (y < Container.sizeY - 1)
                                    {
                                        if (Container.cells[x, y + 1].seed != 0 && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y + 1].spread == true)
                                        {
                                            for (int i = 0; i < 8; i++)
                                            {
                                                if (types[i] == 0)
                                                {
                                                    types[i] = (byte)Container.cells[x, y + 1].seed;
                                                    cases[i]++;
                                                    count++;
                                                    break;
                                                }
                                                else if (types[i] == (byte)Container.cells[x, y + 1].seed)
                                                {
                                                    cases[i]++;
                                                    break;
                                                }
                                            }
                                        }

                                        if (x > 0)
                                        {
                                            if (Container.cells[x - 1, y + 1].seed != 0 && Container.cells[x - 1, y + 1].seed != 1 && Container.cells[x - 1, y + 1].spread == true)
                                            {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                    if (types[i] == 0)
                                                    {
                                                        types[i] = (byte)Container.cells[x - 1, y + 1].seed;
                                                        cases[i]++;
                                                        count++;
                                                        break;
                                                    }
                                                    else if (types[i] == (byte)Container.cells[x - 1, y + 1].seed)
                                                    {
                                                        cases[i]++;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (x < Container.sizeX - 1)
                                        {
                                            if (Container.cells[x + 1, y + 1].seed != 0 && Container.cells[x + 1, y + 1].seed != 1 && Container.cells[x + 1, y + 1].spread == true)
                                            {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                    if (types[i] == 0)
                                                    {
                                                        types[i] = (byte)Container.cells[x + 1, y + 1].seed;
                                                        cases[i]++;
                                                        count++;
                                                        break;
                                                    }
                                                    else if (types[i] == (byte)Container.cells[x + 1, y + 1].seed)
                                                    {
                                                        cases[i]++;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    if (count == 1)
                                    {
                                        sed = types[0];
                                    }
                                    else if (count >= 2)
                                    {
                                        byte maxValue = cases.Max();
                                        int maxIndex = cases.ToList().IndexOf(maxValue);
                                        sed = types[maxIndex];
                                    }

                                    if (count > 0)
                                    {

                                        Container.cells[x, y] = new Cell(x, y, sed, false);
                                        System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                        System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[sed]));
                                        rec.Fill = brush;
                                        Grid.SetRow(rec, y);
                                        Grid.SetColumn(rec, x);
                                        Container.myGrid.Children.Add(rec);
                                        await Task.Delay(Container.ms);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed != 0)
                        {
                            Container.cells[x, y].spread = true;
                        }

                    }
                }
                blanks = 0;
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed == 0)
                        {
                            blanks++;
                        }

                    }
                }
            }
            if ((bool)Container.IsWalls.IsChecked)
            {
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed == 1)
                        {
                            Container.cells[x, y].seed = 0;
                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[0]));
                            rec.Fill = brush;
                            Grid.SetRow(rec, y);
                            Grid.SetColumn(rec, x);
                            Container.myGrid.Children.Add(rec);
                            await Task.Delay(Container.ms);
                        }

                    }
                }
                if ((bool)Container.IsAll.IsChecked)
                {
                    for (int x = 0; x < Container.sizeX; x++)
                    {
                        for (int y = 0; y < Container.sizeY; y++)
                        {
                            byte seed;
                            if (y > 0)
                            {
                                if (Container.cells[x, y - 1].seed != Container.cells[x, y].seed && Container.cells[x, y - 1].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (y < Container.sizeY - 1)
                            {
                                if (Container.cells[x, y + 1].seed != Container.cells[x, y].seed && Container.cells[x, y + 1].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                            if (x > 0)
                            {
                                if (Container.cells[x - 1, y].seed != Container.cells[x, y].seed && Container.cells[x - 1, y].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (x < Container.sizeX - 1)
                            {
                                if (Container.cells[x + 1, y].seed != Container.cells[x, y].seed && Container.cells[x + 1, y].seed != 1)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                        }
                    }
                }
                else
                {
                    for (int x = 0; x < Container.sizeX; x++)
                    {
                        for (int y = 0; y < Container.sizeY; y++)
                        {
                            byte seed;
                            if (y > 0)
                            {
                                if (Container.cells[x, y - 1].seed != Container.cells[x, y].seed && Container.cells[x, y - 1].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (y < Container.sizeY - 1)
                            {
                                if (Container.cells[x, y + 1].seed != Container.cells[x, y].seed && Container.cells[x, y + 1].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                            if (x > 0)
                            {
                                if (Container.cells[x - 1, y].seed != Container.cells[x, y].seed && Container.cells[x - 1, y].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }
                            if (x < Container.sizeX - 1)
                            {
                                if (Container.cells[x + 1, y].seed != Container.cells[x, y].seed && Container.cells[x + 1, y].seed != 1 && Container.cells[x, y].seed == Container.SeedWall)
                                {
                                    Container.cells[x, y] = new Cell(x, y, 1, false);
                                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[1]));
                                    rec.Fill = brush;
                                    Grid.SetRow(rec, y);
                                    Grid.SetColumn(rec, x);
                                    Container.myGrid.Children.Add(rec);
                                    await Task.Delay(Container.ms);
                                }
                            }

                        }
                    }
                }
                for (int x = 0; x < Container.sizeX; x++)
                {
                    for (int y = 0; y < Container.sizeY; y++)
                    {
                        if (Container.cells[x, y].seed != 1)
                        {
                            Container.cells[x, y].seed = 0;
                            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
                            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(Program.ToMediaColor(Container.seedColors[0]));
                            rec.Fill = brush;
                            Grid.SetRow(rec, y);
                            Grid.SetColumn(rec, x);
                            Container.myGrid.Children.Add(rec);
                            await Task.Delay(Container.ms);
                        }

                    }
                }
            }

            MessageBoxResult result = MessageBox.Show("Koniec. Czy exportować", "End", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Export();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("No to nie", "My App");
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        public static void Export()
        {
            string[] lines = new string[(Container.sizeX * Container.sizeY) + 1];
            lines[0] = Container.sizeX + " " + Container.sizeY;
            int index = 1;
            for (int x = 0; x < Container.sizeX; x++)
            {
                for (int y = 0; y < Container.sizeY; y++)
                {
                    lines[index] = Container.cells[x, y].posX + " " + Container.cells[x, y].posY + " " + Container.cells[x, y].seed;
                    index++;
                }
            }
            string docPath;

            Bitmap btm = ImageCreator();


            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                docPath = dialog.FileName;
                using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "WriteLines.txt")))
                {
                    foreach (string line in lines)
                        outputFile.WriteLine(line);
                }
                btm.Save(docPath + "/image.bmp", ImageFormat.Bmp);
            }
        }

        public static MColor ToMediaColor(DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Bitmap ImageCreator()
        {

            Bitmap myBitmap = new Bitmap(Container.sizeX, Container.sizeY);

            for (int Xcount = 0; Xcount < myBitmap.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height; Ycount++)
                {
                    myBitmap.SetPixel(Xcount, Ycount, Container.seedColors[Container.cells[Xcount, Ycount].seed]);
                }
            }


            return myBitmap;
        }
    }




    struct Container
    {
        public static Color[] seedColors = { Color.FromArgb(255, 255, 255), Color.FromArgb(0, 0, 0), Color.FromArgb(32, 178, 170), Color.FromArgb(86, 200, 100), Color.FromArgb(45, 104, 194), Color.FromArgb(168, 66, 184), Color.FromArgb(184, 66, 88), Color.FromArgb(209, 188, 31), Color.FromArgb(43, 232, 14), Color.FromArgb(231, 148, 13), Color.FromArgb(184, 231, 13), Color.FromArgb(61, 220, 54), Color.FromArgb(50, 168, 82), Color.FromArgb(72, 50, 168), Color.FromArgb(212, 36, 197), Color.FromArgb(212, 98, 36), Color.FromArgb(138, 26, 26) };
        public static int seed; //= Int32.Parse(Seedbox.Text);
        public static int random;
        public static int sizeY; //= Int32.Parse(numberTextBoxY.Text);
        public static int sizeX; //= Int32.Parse(numberTextBoxX.Text);
        public static Cell[,] cells;// = new Cell[sizeX, sizeY];
        public static Grid myGrid;
        public static Random rnd = new Random();
        public static int seedid = 2;
        public static Timer timer = new Timer();
        public static int ms;
        public static ComboBox type;
        public static CheckBox IsInclusion;
        public static CheckBox IsStructure;
        public static byte InclusionCount;
        public static byte InclusionSize;
        public static int StructureSeedSel;
        public static Cell[,] importCell;
        public static bool Import = false;

        public static CheckBox IsWalls;
        public static CheckBox IsAll;
        public static int SeedWall;

    }

    struct Cell
    {
        public int posX;
        public int posY;
        public int seed;
        public bool spread;

        public Cell(int posX, int posY, short seed, bool spread)
        {
            this.posX = posX;
            this.posY = posY;
            this.seed = seed;
            this.spread = spread;
        }
    }
}



