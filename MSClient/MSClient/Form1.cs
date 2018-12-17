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
        private static readonly HttpClient _client = new HttpClient();

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

        /// <summary>
        /// Makes ready data to send and handles response
        /// </summary>
        /// <param name="form1">Form1 instance</param>
        private async void Run(Form1 form1)
        {
            string url = form1.textBoxURL.Text;
            FunctionDefinitions fd;
            switch (form1.comboBoxOperation.SelectedItem.ToString())
            {
                case "CREATE":
                    if (string.IsNullOrEmpty(form1.textBoxID.Text) || string.IsNullOrEmpty(form1.textBoxName.Text) || string.IsNullOrEmpty(form1.textBoxFunction.Text))
                    {
                        MessageBox.Show("Missing input data!");
                        return;
                    }
                    else
                    {
                        fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxFunction.Text);
                    }
                    break;
                case "RUN":
                    if (string.IsNullOrEmpty(form1.textBoxName.Text))
                    {
                        MessageBox.Show("Missing input data!");
                        return;
                    }
                    else
                    {
                        fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxParams.Text);
                    }
                    break;
                case "DELETE":
                    if (string.IsNullOrEmpty(form1.textBoxName.Text))
                    {
                        MessageBox.Show("Missing input data!");
                        return;
                    }
                    else
                    {
                        fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxParams.Text);
                    }
                    break;
                case "UPDATE":
                    if (string.IsNullOrEmpty(form1.textBoxName.Text) || string.IsNullOrEmpty(form1.textBoxFunction.Text))
                    {
                        MessageBox.Show("Missing input data!");
                        return;
                    }
                    else
                    {
                        fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxFunction.Text);
                    }
                    break;
                case "READALL":
                    fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxParams.Text);
                    break;
                default:
                    if (string.IsNullOrEmpty(form1.textBoxName.Text))
                    {
                        MessageBox.Show("Missing input data!");
                        return;
                    }
                    else
                    {
                        fd = new FunctionDefinitions(form1.textBoxID.Text, form1.textBoxName.Text, form1.textBoxFunction.Text);
                    }
                    break;
            }

            User user = new User(
                "clinetname",
                "functions." + form1.comboBoxOperation.SelectedItem.ToString().ToLower(),
                "unique message id defined by the caller",
                Newtonsoft.Json.JsonConvert.SerializeObject(fd),
                "00000000-0000-0000-0000-000000000000",
                "00000000-0000-0000-0000-000000000000",
                "All");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            try
            {
                var response = await Request(HttpMethod.Post, url, json, new Dictionary<string, string>());

                var responseText = await response.Content.ReadAsStringAsync();
                responseText = responseText.Replace("\"", string.Empty);

                var responseBytearray = Convert.FromBase64String(responseText);

                var rmw = Serializer<ReturnMessageWrapper>.GetData(responseBytearray);

                form1.textBoxResult.Text = Serializer<string>.GetData(rmw.Data);
            }
            catch (Exception ex)
            {
                form1.textBoxResult.Text = ex.ToString();
            }
        }

        /// <summary>
        /// Performs sending data to web API
        /// </summary>
        /// <param name="pMethod">HttpMethod</param>
        /// <param name="pUrl">url</param>
        /// <param name="pJsonContent">json content</param>
        /// <param name="pHeaders">headers</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> Request(HttpMethod pMethod, string pUrl, string pJsonContent, Dictionary<string, string> pHeaders)
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
            return await _client.SendAsync(httpRequestMessage);
        }

        private void comboBoxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxParams.Enabled = comboBoxOperation.SelectedItem.ToString() == "RUN" ? true : false;
            textBoxFunction.Enabled = comboBoxOperation.SelectedItem.ToString() == "CREATE" || comboBoxOperation.SelectedItem.ToString() == "UPDATE" ? true : false;
        }
    }
}
