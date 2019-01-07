using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Test_Client_1.Models
{
    public class ToastModel 
    {
        public double ExtraSize { get; private set; }
        public string Text { get; set; }
        public SolidColorBrush BackgroundColor { get; private set; }
        public SolidColorBrush FontColor { get; private set; }

        public ToastModel() { }
        public ToastModel(string text, Color fontColor, Color backgroundColor, double extraSize = 0)
        {
            ExtraSize = extraSize;

            Text = text;
            BackgroundColor = new SolidColorBrush(backgroundColor);
            FontColor = new SolidColorBrush(fontColor);
        }
    }
}
