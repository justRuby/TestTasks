using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test_Client_1.CustomControls
{
    using Test_Client_1.Models;

    public partial class AnimatedToastBlock : UserControl
    {
        public AnimatedToastBlock()
        {
            InitializeComponent();
        }

        public AnimatedToastBlock(ToastModel model)
        {
            InitializeComponent();

            BackgroundRectangle.Fill = model.BackgroundColor;
            MessageTextBlock.Text = model.Text;
            MessageTextBlock.Foreground = model.FontColor;

            BackgroundRectangle.Width = MessageTextBlock.Width + model.ExtraSize;
        }

        public void StartAnimation()
        {
            var story = this.FindResource("StoryboardToast") as Storyboard;
            BeginStoryboard(story);
        }

        ~AnimatedToastBlock()
        {
            //_queueToast = null;
        }
    }
}
