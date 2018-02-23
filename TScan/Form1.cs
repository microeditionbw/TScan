using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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
          
            //Регион
            dynamic People = _ApiRequest.GetPeopleFromSearch(Country.Text, City.Text, textBox2.Text, textBox4.Text);
            listBox1.Items.Clear();
            this.Text = textBox1.Text + ". Зарегистрировано: " + People.response.count;
            foreach (var i in People.response.items)
            {
                listBox1.Items.Add("id"+i.id);
            }
            if (listBox1.Items.Count > 2)
            {
                listBox1.SelectedIndex = 0;
            }
            //Определение региона
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(textBox2.Text);
            if (a > 1000) { textBox2.Text = "999"; }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
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


            dynamic PeopleWall = _ApiRequest.GetPeopleWall(listBox1.Text, textBox3.Text);
            listBox2.Items.Clear();
            foreach (var i in PeopleWall.response.items)
            {
                if (i.text != "")
                {
                    int timestamp = i.date;
                    DateTime date = new DateTime(1970, 1, 1).AddSeconds(timestamp);

                    listBox2.Items.Add("[" + date+"] "+i.text);
                }
                
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

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
              
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                if (textBox5.Text != "")
                {
                    _UserId = textBox5.Text;
                    string[] Params = { "city", "country", "photo_max" };
                    _Response = _ApiRequest.GetInformation(_UserId, Params);
                    if (_Response != null)
                    {
                        User_ID.Text = _UserId;
                        pictureBox1.ImageLocation = _Response["photo_max"];
                        Uname.Text = _Response["first_name"];
                        Ulast.Text = _Response["last_name"];
                    }


                    dynamic PeopleWall = _ApiRequest.GetPeopleWall(textBox5.Text, textBox3.Text);
                    listBox2.Items.Clear();
                    foreach (var i in PeopleWall.response.items)
                    {
                        if (i.text != "")
                        {
                            listBox2.Items.Add("[" + i.date + "] " + i.text);
                        }

                    }
                }

            }
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
}
