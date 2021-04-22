using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HuffmanProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string text;
        ScrollViewer MainSw;
        Dictionary<char, double> freqDictionary;
        Dictionary<char, string> codeDictionary;
        string path;
        string newpath;
        public MainWindow()
        {
            InitializeComponent();
            MainSw = SwMain;
            ((TextBlock)MainSw.Content).Text = "кубабеба";
        }
        Dictionary<char, double> GetFrequency(string text)
        {
            if (text == null) return new Dictionary<char, double>();
            Dictionary<char, double> res = new Dictionary<char, double>();
            foreach (char sym in text)
            {
                if (res.ContainsKey(sym)) res[sym]++;
                else res.Add(sym, 1);
            }
            int l = text.Length;
            for (int i = 0; i < res.Count; i++)
            {
                res[res.ElementAt(i).Key] /= l;
            }

            res = res.OrderBy((p) => p.Value).ToDictionary(p => p.Key, p => p.Value);

            return res;
        }

        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Header)
            {
                case "Файл": GetFile();
                    break;
                case "Анализировать": Analyze();
                    break;
                case "Закодировать": Encode();
                    break;
                case "Раскодировать": Decode();
                    break;
            }
        }

        private void Decode()
        {
            if (string.IsNullOrEmpty(path)) return;

            using (StreamReader sr = new StreamReader(newpath))
            {
                string decres = CubaBeba.Decode(codeDictionary, sr.ReadToEnd());

                if (decres.Length > 10000)
                {
                    TbDecyph.Text = decres.Substring(0, 10000) + "\n file too long to be fully................................................."; 
                }
                else
                {
                    TbDecyph.Text = decres;
                }
            }
            //TbDecyph.Text = CubaBeba.Decode(codeDictionary, TbCyph.Text).Substring(0, 10000);
        }

        private void Encode()
        {
            if (string.IsNullOrEmpty(path)) return;
            string encoderes = CubaBeba.Encode(codeDictionary, text);

            int ind = path.IndexOf(".txt",StringComparison.CurrentCultureIgnoreCase);
            newpath = path.Insert(ind, "(Encoded)");
            using (StreamWriter sw = new StreamWriter(newpath))
            {
                sw.Write(encoderes);
            }
            if (encoderes.Length > 10000)
            {
                TbCyph.Text = encoderes.Substring(0, 10000) + "\n file too long to be fully................................................."; 
            }
            else
            {
                TbCyph.Text = encoderes;
            }
        }

        private void Analyze()
        {
            LbEncode.Items.Clear();
            LbFreq.Items.Clear();
            TbCyph.Text = "";
            TbDecyph.Text = "";
            freqDictionary = GetFrequency(text);
            foreach (var p in freqDictionary)
            {
                LbFreq.Items.Add($"{p.Key}\t - {p.Value}");
            }
            codeDictionary = HuffmanCoding.CreateSymboleCodeDictionary(freqDictionary);
            codeDictionary = codeDictionary.OrderBy((p) => p.Value.Length).ToDictionary(p => p.Key, p => p.Value);
            foreach (var p in codeDictionary)
            {
                LbEncode.Items.Add($"{p.Key}\t - {p.Value}");
            }
        }

        private void GetFile()
        {
            LbEncode.Items.Clear();
            LbFreq.Items.Clear();
            TbCyph.Text = "";
            TbDecyph.Text = "";
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                path = ofd.FileName;
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    text = sr.ReadToEnd();
                    if (text.Length > 10000)
                    {
                        TbMain.Text = text.Substring(0, 10000) + "\n file too long to be fully.................................................";
                    }
                    else
                    {
                        TbMain.Text = text;
                    }
                    //foreach (var p in GetFrequency(text))
                    //{
                    //    LbFreq.Items.Add($"{p.Key} - {p.Value}");
                    //}
                }
            }
        }

        private void TbCyph_Drop(object sender, DragEventArgs e)
        {
            LbEncode.Items.Clear();
            LbFreq.Items.Clear();
            TbCyph.Text = "";
            TbDecyph.Text = "";
            //Type t = e.Data.GetType();
            object txt = e.Data.GetData(DataFormats.FileDrop);
            string[] vs = txt as string[];
            path = vs[0];
            using (StreamReader sr = new StreamReader(vs[0]))
            {
                text = sr.ReadToEnd();
                if (text.Length > 10000)
                {
                    TbMain.Text = text.Substring(0, 10000) + "\n file too long to be fully................................................."; 
                }
                else
                {
                    TbMain.Text = text;
                }
                //foreach (var p in GetFrequency(text))
                //{
                //    LbFreq.Items.Add($"{p.Key} - {p.Value}");
                //}
            }
        }

        private void SidePanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var el = MainGrid.Children.Cast<UIElement>().First(b => Grid.GetRow(b) == 1 && Grid.GetColumn(b) == 2);
            if (el.Equals(MainSw))
            {
                MainGrid.Children.Remove(el);
                if (sender is ListBox)
                {
                    ListBox d = sender as ListBox;
                    ListBox lb = new ListBox();
                    foreach (var item in d.Items)
                    {
                        lb.Items.Add(item);
                    }
                    lb.Background = d.Background;
                    lb.Margin = d.Margin;
                    Grid.SetRowSpan(lb, 2);
                    Grid.SetColumn(lb, 2);
                    Grid.SetRow(lb, 1);
                    MainGrid.Children.Add(lb);
                }
                else if (sender is TextBlock)
                {
                    ScrollViewer sw = new ScrollViewer();
                    TextBlock d = sender as TextBlock;
                    TextBlock lb = new TextBlock();
                    lb.Text = d.Text;
                    lb.Background = d.Background;
                    lb.Margin = d.Margin;
                    lb.TextWrapping = TextWrapping.Wrap;
                    sw.Content = lb;
                    Grid.SetRowSpan(sw, 2);
                    Grid.SetColumn(sw, 2);
                    Grid.SetRow(sw, 1);
                    MainGrid.Children.Add(sw);
                }
            }
            else
            {
                Grid.SetColumn(MainSw, 2);
                Grid.SetRow(MainSw, 1);
                MainGrid.Children.Remove(el);
                MainGrid.Children.Add(MainSw);
            }
        }
    }
}