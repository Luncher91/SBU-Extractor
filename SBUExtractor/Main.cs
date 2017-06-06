using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SBUExtractor.DataExtraction;

namespace SBUExtractor
{
    public partial class Main : Form
    {
        // You have to add every kinds of data to this Array!
        public static readonly Extractable[] Extracts = new Extractable[] {
                                                                                new VCalendar()
                                                                                ,new VCard()
                                                                                //,new Files()
                                                                                , new Jpeg()
                                                                                //, new JpegFinder(),
                                                                                , new FilesNew()
                                                                                , new SMMExtraction()
                                                                                , new SPBExtraction()
                                                                          };

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // disable Extracting and COunting until no valid SBU file was choosen
            this.chooseDataGroup.Enabled = false;
            this.extractDataGroup.Enabled = false;
            // write all available modules to the DataGridView
            LoadDataModules();
        }

        // writes all available modules to the DataGridView
        private void LoadDataModules()
        {
            for(int i = 0; i < Extracts.Length; i++)
            {
                int position = this.dataGridView1.Rows.Add(new object[] { false, Extracts[i].GetLabel(), 0 });
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            string filterString = GetCombinedFilter(Extracts);

            // choose a SBU-File
            OpenFileDialog fileD = new OpenFileDialog
                                       {
                                           CheckFileExists = true,
                                           Multiselect = false,
                                           Filter = filterString
                                       };

            // Show the Dialog
            DialogResult res = fileD.ShowDialog(this);

            if(res == DialogResult.OK)
            {
                if(this.isValidSBU(fileD.FileName))
                {
                    // enable the next steps (Counting and Extracting)
                    this.chooseDataGroup.Enabled = true;
                    extractDataGroup.Enabled = true;
                    // write the choosen file to the TextBox
                    this.fileTextBox.Text = fileD.FileName;

                    for(int i = 0; i < Extracts.Length; i++)
                    {
                        if (Extracts[i].FileMatches(fileD.FileName))
                        {
                            dataGridView1.Rows[i].ReadOnly = false;
                            dataGridView1.Rows[i].Visible = true;
                        }
                        else
                        {
                            dataGridView1.Rows[i].ReadOnly = true;
                            dataGridView1.Rows[i].Visible = false;
                            dataGridView1.Rows[i].Cells[0].Value = false;
                        }
                        
                    }
                }
                else
                {
                    // disable the next steps (Counting and Extracting)
                    this.chooseDataGroup.Enabled = false;
                    extractDataGroup.Enabled = false;
                    // write the choosen file to the TextBox
                    this.fileTextBox.Text = fileD.FileName;
                    // Show an error
                    MessageBox.Show("This file is not a valid Kies backup file!");
                }
            }
        }

        private string GetCombinedFilter(Extractable[] extracts)
        {
            List<string> filterList = new List<string>();

            foreach(Extractable ext in extracts)
            {
                foreach(var fileType in ext.GetFilematches())
                {
                    if(!filterList.Contains(fileType.ToString()))
                        filterList.Add(fileType.ToString());
                }
            }

            return string.Join("|", filterList.ToArray());
        }

        // TODO: Check if the SBU-File is a valid SBU-File
        private bool isValidSBU(string path)
        {
            return true;
        }

        // runs for all selected kinds of data the CountData Method
        private void countButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Extracts.Length; i++)
                if((bool)this.dataGridView1.Rows[i].Cells[0].Value)
                    Extracts[i].CountData(this.fileTextBox.Text, this, this.dataGridView1.Rows[i].Cells[2]);
        }

        // starts the extraction of selected data
        private void extractDataButton_Click(object sender, EventArgs e)
        {
            // ask user where to place the extracted data
            FolderBrowserDialog fb = new FolderBrowserDialog
                                         {Description = "Open Folder for extracted data...", ShowNewFolderButton = true};

            DialogResult res = fb.ShowDialog(this);

            // Path was choosen
            if(res != DialogResult.OK)
                return;

            // so run ExtractData for all selected data
            for (int i = 0; i < Extracts.Length; i++)
                if ((bool)this.dataGridView1.Rows[i].Cells[0].Value)
                    Extracts[i].ExtractData(this.fileTextBox.Text, fb.SelectedPath + "\\" + Extracts[i].GetLabel(), this);
        }

        // same as extractDataButton but its not important if the data was selected or not
        private void extractAllButton_Click(object sender, EventArgs e)
        {
            // ask user where to place the extracted data
            FolderBrowserDialog fb = new FolderBrowserDialog { Description = "Open Folder for extracted data...", ShowNewFolderButton = true };

            DialogResult res = fb.ShowDialog(this);

            // Path was choosen
            if (res != DialogResult.OK)
                return;

            // so run ExtractData for all data
            for (int i = 0; i < Extracts.Length; i++)
                if(dataGridView1.Rows[i].Visible)
                    Extracts[i].ExtractData(this.fileTextBox.Text, fb.SelectedPath + "\\" + Extracts[i].GetLabel(), this);
        }
    }
}
