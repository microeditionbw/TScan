using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xNet;

namespace TScan
{
    public partial class MainForm : Form
    {
        VkAPI _ApiRequest;
        private string _Token;  //Токен, использующийся при запросах
        private string _UserId;  //ID пользователя
        private Dictionary<string, string> _Response;  //Ответ на запросы

        public MainForm()
        {
            InitializeComponent();
        }

        private void Button_GetToken_Click(object sender, EventArgs e)
        {
            AuthorizationForm GetToken = new AuthorizationForm();
            GetToken.ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void GetToken_Click(object sender, EventArgs e)
        {
            AuthorizationForm GetToken = new AuthorizationForm();
            GetToken.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                StreamReader ControlInf = new StreamReader("UserInf.txt");
                _Token = ControlInf.ReadLine();
                _UserId = ControlInf.ReadLine();
                ControlInf.Close();
                if (_Token != null)
                {
                    _ApiRequest = new VkAPI(_Token);
                    string[] Params = { "city", "country", "photo_max" };
                    _Response = _ApiRequest.GetInformation(_UserId, Params);
                    if (_Response != null)
                    {
                        User_ID.Text = _UserId;
                        User_Photo.ImageLocation = _Response["photo_max"];
                        User_Name.Text = _Response["first_name"];
                        User_Surname.Text = _Response["last_name"];
                        User_Country.Text = _ApiRequest.GetCountryById(_Response["country"]);
                        User_City.Text = _ApiRequest.GetCityById(_Response["city"]);
                        GetToken.Visible = false;
                    }
                }
            }
            catch { }

            //Загрузка списка стран
            comboBox1.Items.Clear();

            dynamic results = _ApiRequest.GetCountries();
            foreach (var i in results.response.items)
            {
            
                if (i.title.ToString() == "Россия")
                {
                    ru = it;
                    Country.Text = i.id.ToString();
                }
                it++;
                comboBox1.Items.Add(i.title.ToString());
            }
            comboBox1.SelectedIndex = ru;
            //загрузка списка стран конец

            if (textBox1.Text != "")
            {
                //Регион
                string id = _ApiRequest.GetCountryIdByName(_ApiRequest.GetCountries(), comboBox1.Text);
                Country.Text = id;
                dynamic Cities = _ApiRequest.GetCitiesById(id, textBox1.Text);
                comboBox2.Items.Clear();
                foreach (var i in Cities.response.items)
                {
                    comboBox2.Items.Add(i.region.ToString());
                }
                if (comboBox2.Items.Count > 2)
                {
                    comboBox2.SelectedIndex = 0;
                }
                //Определение региона
            }
        }
        int it = 0;
        int ru = 0;
  


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id = _ApiRequest.GetCountryIdByName(_ApiRequest.GetCountries(), comboBox1.Text);
            dynamic Cities = _ApiRequest.GetCitiesById(id, textBox1.Text);
            foreach (var i in Cities.response.items)
            {
                if (i.region.ToString() == comboBox2.Text)
                {
                    City.Text = i.id.ToString();
                }

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                if (textBox1.Text != "")
                {
                    //Регион
                    string id = _ApiRequest.GetCountryIdByName(_ApiRequest.GetCountries(), comboBox1.Text);
                    Country.Text = id;
                    dynamic Cities = _ApiRequest.GetCitiesById(id, textBox1.Text);
                    comboBox2.Items.Clear();
                    foreach (var i in Cities.response.items)
                    {
                        comboBox2.Items.Add(i.region.ToString());
                    }
                    if (comboBox2.Items.Count > 2)
                    {
                        comboBox2.SelectedIndex = 0;
                    }
                    //Определение региона
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string[] words = textBox6.Text.Split(new char[] { ',' });
                listBox1.Items.Clear();
                //Регион
                int offset = Convert.ToInt32(textBox2.Text) * Convert.ToInt32(textBox4.Text);
                dynamic People = _ApiRequest.GetPeopleFromSearch(Country.Text, City.Text, textBox2.Text, offset.ToString());
                this.Text = textBox1.Text + ". Зарегистрировано: " + People.response.count;
                progressBar1.Value = 0;
                progressBar1.Maximum = Convert.ToInt32(textBox3.Text);
                foreach (var i in People.response.items)
                {

                    listBox1.Items.Add("id" + i.id);
                    progressBar1.Increment(1);
                }
                if (listBox1.Items.Count > 2)
                {
                    listBox1.SelectedIndex = 0;
                }
               
                
            }
              
            //Определение региона
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                int a = Convert.ToInt32(textBox2.Text);
                if (a > 1000) { textBox2.Text = "999"; }
            }
         
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _UserId = listBox1.Text;
                string[] Params = { "city", "country", "photo_max" };
                _Response = _ApiRequest.GetInformation(_UserId, Params);
                if (_Response != null)
                {
                    User_ID.Text = _UserId;
                    pictureBox1.ImageLocation = _Response["photo_max"];
                    Uname.Text = _Response["first_name"];
                    Ulast.Text = _Response["last_name"];
                }

                string[] words = textBox6.Text.Split(new char[] { ',' });
                listBox2.Items.Clear();
                foreach (string word in words)
                {
                    dynamic PeopleWalls = _ApiRequest.GetPeopleWallSearch(listBox1.Text, word, textBox3.Text, 0);
                    if (PeopleWalls != null)
                    {
                       
                        foreach (var i in PeopleWalls.response.items)
                        {
                            if (i.text != "")
                            {
                                int timestamp = i.date;
                                DateTime date = new DateTime(1970, 1, 1).AddSeconds(timestamp);
                                listBox2.Items.Add(i.text);
                            }
                        }

                    }
                }
                
            }
            catch (Exception)
            {

            }
        }

        private void GetInformation_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader ControlInf = new StreamReader("UserInf.txt");
                _Token = ControlInf.ReadLine();
                ControlInf.Close();
                _ApiRequest = new VkAPI(_Token);
                _UserId = User_ID.Text;
                string[] Params = { "city", "country", "photo_max" };
                _Response = _ApiRequest.GetInformation(_UserId, Params);
                if (_Response != null)
                {
                    User_ID.Text = _UserId;
                    User_Photo.ImageLocation = _Response["photo_max"];
                    User_Name.Text = _Response["first_name"];
                    User_Surname.Text = _Response["last_name"];
                    User_Country.Text = _ApiRequest.GetCountryById(_Response["country"]);
                    User_City.Text = _ApiRequest.GetCityById(_Response["city"]);
                    GetToken.Visible = false;
                }
            }
            catch
            {

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
       // List<string> words = new List<string>();
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
   
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 0;
            toolTip1.InitialDelay = 0;
            toolTip1.ReshowDelay = 0;
            toolTip1.ShowAlways = true;
          
            toolTip1.SetToolTip(this.listBox2, listBox2.Items[listBox2.SelectedIndex].ToString());
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
              
        }

       

        private void textBox1_Leave(object sender, EventArgs e)
        {

            if (textBox1.Text != "")
            {
                //Регион
                string id = _ApiRequest.GetCountryIdByName(_ApiRequest.GetCountries(), comboBox1.Text);
                Country.Text = id;
                dynamic Cities = _ApiRequest.GetCitiesById(id, textBox1.Text);
                comboBox2.Items.Clear();
                int ai = 0;
                foreach (var i in Cities.response.items)
                {
                    ai++;
                    if (ai == 1) { City.Text = i.id.ToString(); }
                    comboBox2.Items.Add(i.region.ToString());
                }
                if (comboBox2.Items.Count > 2)
                {
                    comboBox2.SelectedIndex = 0;
                }
                //Определение региона
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HttpRequest GetCountries = new HttpRequest();
            string Result = GetCountries.Get("https://api.vk.com/api.php?oauth=1&v=5.5&method=database.getCountries&need_all=1&count=1000").ToString();
            MessageBox.Show(Result);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(Result);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://vk.com/"+listBox1.Text);
        }

        private void Tab2_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
          
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
         
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }
    }
}
