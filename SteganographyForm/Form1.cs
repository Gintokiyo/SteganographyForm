namespace SteganographyForm
{
    public partial class Stegano : Form
    {
        // Our original image that we load will be here
        private Image? _loadedTrueImage = null;
        // Our decrypted image will be here
        private Image? _decryptedImage = null;
        private Bitmap? _loadedTrueBitmap = null;
        private Bitmap? _decryptedBitmap = null;

        // Our message length can't pass this number
        private double _textSize = 0;
        public Stegano()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Upload button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog= new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.png) | *.png;";

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxImage.Text = openFileDialog.FileName.ToString();
                pictureBoxImage.ImageLocation = textBoxImage.Text;

                _loadedTrueImage = Image.FromFile(textBoxImage.Text);
                _loadedTrueBitmap = new Bitmap(_loadedTrueImage);
                _decryptedImage = Image.FromFile(textBoxImage.Text);
                _decryptedBitmap = new Bitmap(_decryptedImage);
            }

            if(this._loadedTrueImage != null)
            {
                this._textSize = 
                    (8.0 * ((this._loadedTrueImage.Height * (this._loadedTrueImage.Width / 3) * 3) / 3 - 1)) / 1024;
            }
        }

        /// <summary>
        /// Encode Function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEncode_Click(object sender, EventArgs e)
        {
            if (textBoxImage.Text == "") MessageBox.Show("Please upload an image");

            if (textBoxMessage.Text == "") MessageBox.Show("Please write a Message");

            if (textBoxImage.Text != "" && textBoxMessage.Text != "")
            {
                var message = textBoxMessage.Text;
                var textLen = (double)System.Text.Encoding.ASCII.GetByteCount(message) / 1024;

                if (_textSize > textLen)
                {
                    Bitmap img = new Bitmap(textBoxImage.Text);

                    for (int width = 0; width < img.Width; width++)
                    {
                        for (int height = 0; height < img.Height; height++)
                        {
                            Color pixel = img.GetPixel(width, height);

                            if (width < 1 && height < textBoxMessage.TextLength)
                            {
                                Console.WriteLine("R= [" + width + "][" + height + "]=" + pixel.R);
                                Console.WriteLine("G= [" + width + "][" + height + "]=" + pixel.G);
                                Console.WriteLine("B= [" + width + "][" + height + "]=" + pixel.B);

                                var letter = Convert.ToChar(textBoxMessage.Text.Substring(height, 1));
                                var letterValue = Convert.ToInt32(letter);
                                Console.WriteLine("Letter: " + letter + " Value: " + letterValue);
                                img.SetPixel(width, height, Color.FromArgb(pixel.R, pixel.G, letterValue));
                            }

                            if (width == img.Width - 1 && height == img.Height - 1)
                            {
                                img.SetPixel(width, height, Color.FromArgb(pixel.R, pixel.G, textBoxMessage.TextLength));
                            }
                        }
                    }

                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Image Files (*.png) | *.png";
                    saveFile.InitialDirectory = "\\.";

                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        textBoxImage.Text = saveFile.FileName.ToString();
                        pictureBoxImage.ImageLocation = textBoxImage.Text;
                        img.Save(textBoxImage.Text);
                    }

                    MessageBox.Show("File saved succesfully!");
                }
                else
                {
                    MessageBox.Show("Cannot save more than " + textLen + " KB!");
                }


            }

        }

        /// <summary>
        /// Decode Function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDecode_Click(object sender, EventArgs e)
        {
            labelMessage.Text = "Decoded Message";

            if (textBoxImage.Text == "")
            {
                MessageBox.Show("Please pick an image to decode!");
                return;
            }

            var img = new Bitmap(textBoxImage.Text);
            string msg = "";
            Color lastPixel = img.GetPixel(img.Width - 1, img.Height - 1);
            var msgLength = lastPixel.B;

            for (int width = 0; width < img.Width; width++)
            {
                for (int height = 0; height < img.Height; height++)
                {
                    Color pixel = img.GetPixel(width, height);

                    if (width < 1 && height < msgLength)
                    {
                        var value = pixel.B;

                        string letter = System.Text.Encoding.ASCII.GetString(new byte[] { value });
                        Console.WriteLine("Letter: " + letter + " Value: " + value);
                        msg += letter;
                    }
                }
            }

            textBoxMessage.Text = msg;

            if (textBoxMessage.Text == "") MessageBox.Show("No data hidden in the message!");
        }
    }
}