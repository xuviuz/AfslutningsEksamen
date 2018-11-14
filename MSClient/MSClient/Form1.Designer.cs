namespace MSClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFunction = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonPost = new System.Windows.Forms.Button();
            this.comboBoxOperation = new System.Windows.Forms.ComboBox();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxParams = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxURL
            // 
            this.textBoxURL.BackColor = System.Drawing.Color.LemonChiffon;
            this.textBoxURL.Location = new System.Drawing.Point(27, 27);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(546, 20);
            this.textBoxURL.TabIndex = 0;
            this.textBoxURL.Text = "https://localhost:44359/api/RPC";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(27, 69);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(546, 20);
            this.textBoxID.TabIndex = 1;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(27, 112);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(546, 20);
            this.textBoxName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(601, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "URL";
            // 
            // textBoxFunction
            // 
            this.textBoxFunction.Location = new System.Drawing.Point(27, 256);
            this.textBoxFunction.Multiline = true;
            this.textBoxFunction.Name = "textBoxFunction";
            this.textBoxFunction.Size = new System.Drawing.Size(397, 217);
            this.textBoxFunction.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(601, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(601, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Function";
            // 
            // buttonPost
            // 
            this.buttonPost.Location = new System.Drawing.Point(313, 489);
            this.buttonPost.Name = "buttonPost";
            this.buttonPost.Size = new System.Drawing.Size(111, 23);
            this.buttonPost.TabIndex = 8;
            this.buttonPost.Text = "POST";
            this.buttonPost.UseVisualStyleBackColor = true;
            this.buttonPost.Click += new System.EventHandler(this.buttonPost_Click);
            // 
            // comboBoxOperation
            // 
            this.comboBoxOperation.FormattingEnabled = true;
            this.comboBoxOperation.Items.AddRange(new object[] {
            "create",
            "delete",
            "update",
            "read",
            "run"});
            this.comboBoxOperation.Location = new System.Drawing.Point(27, 491);
            this.comboBoxOperation.Name = "comboBoxOperation";
            this.comboBoxOperation.Size = new System.Drawing.Size(121, 21);
            this.comboBoxOperation.TabIndex = 9;
            this.comboBoxOperation.SelectedIndexChanged += new System.EventHandler(this.comboBoxOperation_SelectedIndexChanged);
            // 
            // textBoxResult
            // 
            this.textBoxResult.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxResult.Location = new System.Drawing.Point(457, 256);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ReadOnly = true;
            this.textBoxResult.Size = new System.Drawing.Size(303, 217);
            this.textBoxResult.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(475, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Result";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(601, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Parameters";
            // 
            // textBoxParams
            // 
            this.textBoxParams.Enabled = false;
            this.textBoxParams.Location = new System.Drawing.Point(27, 160);
            this.textBoxParams.Name = "textBoxParams";
            this.textBoxParams.Size = new System.Drawing.Size(546, 20);
            this.textBoxParams.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 524);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxParams);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.comboBoxOperation);
            this.Controls.Add(this.buttonPost);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxFunction);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.textBoxURL);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFunction;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonPost;
        private System.Windows.Forms.ComboBox comboBoxOperation;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxParams;
    }
}

