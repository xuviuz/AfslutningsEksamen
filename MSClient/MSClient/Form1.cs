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
        /// <summary>
        /// HttpClient instance
        /// </summary>
        private static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the Form1
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            comboBoxOperation.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles buttons Post click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            // Checks if all necessary fields are filled up and initializes appropriate FunctionDefinitions object
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
                "functions." + form1.comboBoxOperation.SelectedItem.ToString().ToLower(), // routing key
                "unique message id defined by the caller",
                Newtonsoft.Json.JsonConvert.SerializeObject(fd), // serialized function definitions
                "00000000-0000-0000-0000-000000000000", // organisation id
                "00000000-0000-0000-0000-000000000000", // user context token
                "All"); // tracing
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            try
            {
                // Performs request and creates response which contains returned data
                HttpResponseMessage response = await Request(HttpMethod.Post, url, json, new Dictionary<string, string>());

                // Serializes the Http content to the string as an asynchronous operation
                string responseText = await response.Content.ReadAsStringAsync();
                responseText = responseText.Replace("\"", string.Empty);

                // Converts binary data from base-64 digits to eqvivalent 8-bit unsigned integer array
                byte[] responseBytearray = Convert.FromBase64String(responseText);
 
                // Coverts 8-bit unsigned integer array to ReturnMessageWrapper object
                ReturnMessageWrapper rmw = Serializer<ReturnMessageWrapper>.GetData(responseBytearray);

                // Converts data from ReturnMessageWrapper object to string and displays on screen
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
            // Creates a new instance of HttpRequestMessage, which contains message to be send
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
                    // Creates a new instance of the StringContent
                    HttpContent httpContent = new StringContent(pJsonContent, Encoding.UTF8, "application/json");
                    httpRequestMessage.Content = httpContent;
                    break;

            }
            // Sendes Http request as an asynchronous operation
            return await _client.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Handles ComboBoxs Operation Changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxParams.Enabled = comboBoxOperation.SelectedItem.ToString() == "RUN" ? true : false;
            textBoxFunction.Enabled = comboBoxOperation.SelectedItem.ToString() == "CREATE" || comboBoxOperation.SelectedItem.ToString() == "UPDATE" ? true : false;
        }
    }
}
