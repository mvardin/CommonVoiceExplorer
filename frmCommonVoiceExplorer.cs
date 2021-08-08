using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DubleKon.ir
{
    public partial class frmCommonVoiceExplorer : Form
    {
        List<Speech> list = new List<Speech>();
        public string RootPath { get; set; }
        public frmCommonVoiceExplorer()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                btnCheckAllFiles.Invoke((MethodInvoker)delegate
                {
                    FolderBrowserDialog folder = new FolderBrowserDialog();
                    if (folder.ShowDialog() == DialogResult.OK)
                    {
                        RootPath = folder.SelectedPath;
                        var lines = File.ReadAllLines(RootPath + "\\validated.tsv");

                        foreach (var line in lines)
                        {
                            var detail = line.Split(new string[] { "\t" }, StringSplitOptions.None);
                            Speech speech = new Speech()
                            {
                                ClientId = detail[0],
                                Path = detail[1],
                                Sentence = detail[2],
                                Age = detail[5],
                                Gender = detail[6],
                            };
                            list.Add(speech);
                        }

                        var groupedByOrderedList = list.GroupBy(a => a.ClientId).OrderBy(a => a.Count());

                        foreach (var item in groupedByOrderedList)
                        {
                            listBox1.Items.Insert(0, item.Count() + " - " + item.First().ClientId);
                        }
                    }
                });
            });
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                btnCheckAllFiles.Invoke((MethodInvoker)delegate
                {
                    var selected = listBox1.Items[listBox1.SelectedIndex].ToString().Split('-').Last().Trim();
                    var items = list.Where(a => a.ClientId == selected).OrderBy(a => Guid.NewGuid());

                    txtClientId.Text = items.First().ClientId;
                    lblAge.Text = items.First().Age;
                    lblGender.Text = items.First().Gender;

                    listBox2.Items.Clear();
                    foreach (var item in items)
                    {
                        listBox2.Items.Add(item.Sentence + " - " + item.Path);
                    }
                });
            });
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                btnCheckAllFiles.Invoke((MethodInvoker)delegate
                {
                    var speek = listBox2.Items[listBox2.SelectedIndex].ToString().Split('-').Last().Trim();
                    string path = RootPath + "\\clips\\" + speek;
                    WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer
                    {
                        URL = path
                    };
                    wplayer.controls.play();
                });
            });
        }
        private void btnLoadInfo_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                btnCheckAllFiles.Invoke((MethodInvoker)delegate
                {
                    var items = list.Where(a => a.ClientId == txtClientId.Text).OrderBy(a => Guid.NewGuid());

                    txtClientId.Text = items.First().ClientId;
                    lblAge.Text = items.First().Age;
                    lblGender.Text = items.First().Gender;

                    listBox2.Items.Clear();
                    foreach (var item in items)
                    {
                        listBox2.Items.Add(item.Sentence + " - " + item.Path);
                    }
                });
            });
        }
        private void btnCheckAllFiles_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                btnCheckAllFiles.Invoke((MethodInvoker)delegate
                {
                    var items = list.Where(a => a.ClientId == txtClientId.Text).ToList();
                    int exist = 0;
                    foreach (var item in items)
                    {
                        string path = RootPath + "\\clips\\" + item.Path;
                        if (File.Exists(path))
                            exist++;
                    }
                    MessageBox.Show("Exist: " + exist + "\r\nNot exist: " + (items.Count - exist));
                });
            });
        }
    }
    public class Speech
    {
        public string ClientId { get; set; }
        public string Path { get; set; }
        public string Sentence { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
    }
}
