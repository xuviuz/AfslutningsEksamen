using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSClient
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient _Client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            comboBoxOperation.SelectedIndex = 0;
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            textBoxResult.Clear();
            Run(this);
        }

        static async void Run(Form1 form1)
        {
            string url = form1.textBoxURL.Text;
            FunctionDefinitions fd;
            switch (form1.comboBoxOperation.SelectedItem.ToString())
            {
                case "create":
                    fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxFunction.Text);
                    break;
                case "run":
                    fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxParams.Text);
                    break;
                case "delete":
                    fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text,form1.textBoxParams.Text);
                    break;
                case "update":
                    fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxFunction.Text);
                    break;
                default:
                    fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxFunction.Text);
                    break;
            }
           
            Customer customer = new Customer(
                "clinetname",
                "functions." + form1.comboBoxOperation.SelectedItem.ToString(),
                "unique message id defined by the caller",
                Newtonsoft.Json.JsonConvert.SerializeObject(fd),
                "00000000-0000-0000-0000-000000000000",
                "00000000-0000-0000-0000-000000000000",
                "All");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(customer);

            try
            {
                var response = await Request(HttpMethod.Post, url, json, new Dictionary<string, string>());

                var responseText = await response.Content.ReadAsStringAsync();
                responseText = responseText.Replace("\"", string.Empty);

                var responseBytearray = Convert.FromBase64String(responseText);

                var rmw = Serializer<ReturnMessageWrapper>.GetData(responseBytearray);

                var myData = Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rmw.Data));
                form1.textBoxResult.Text = myData.ToString();
            }
            catch (Exception ex)
            {
                form1.textBoxResult.Text = ex.ToString();
            }
        }

        static async Task<HttpResponseMessage> Request(HttpMethod pMethod, string pUrl, string pJsonContent, Dictionary<string, string> pHeaders)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = pMethod;
            httpRequestMessage.RequestUri = new Uri(pUrl);
            foreach (var head in pHeaders)
            {
                httpRequestMessage.Headers.Add(head.Key, head.Value);
            }
            switch (pMethod.Method)
            {
                case "POST":
                    HttpContent httpContent = new StringContent(pJsonContent, Encoding.UTF8, "application/json");
                    httpRequestMessage.Content = httpContent;
                    break;

            }
            return await _Client.SendAsync(httpRequestMessage);
        }

        private void comboBoxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxParams.Enabled = comboBoxOperation.SelectedItem.ToString() == "run" ? true : false;
            textBoxFunction.Enabled = comboBoxOperation.SelectedItem.ToString() == "create" || comboBoxOperation.SelectedItem.ToString() == "update" ? true : false;
        }
    }
}
