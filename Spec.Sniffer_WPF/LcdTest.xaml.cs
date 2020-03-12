using System.Windows.Input;
using System.Windows.Media;

namespace Spec.Sniffer_WPF
{

    public partial class LcdTest
    {
        public LcdTest()
        {
            InitializeComponent();
            BackgroundGrid.DataContext = this;

            BackgroundGridColors.Color = Color.FromRgb(255, 255, 255);
        }

        private void BackgroundChange()
        {
            if (BackgroundGridColors.Color == Color.FromRgb(255, 255, 255))
                BackgroundGridColors.Color = Color.FromRgb(255, 0, 0);
            else if (BackgroundGridColors.Color == Color.FromRgb(255, 0, 0))
                BackgroundGridColors.Color = Color.FromRgb(0, 255, 0);
            else if (BackgroundGridColors.Color == Color.FromRgb(0, 255, 0))
                BackgroundGridColors.Color = Color.FromRgb(0, 0, 255);
            else if (BackgroundGridColors.Color == Color.FromRgb(0, 0, 255))
                BackgroundGridColors.Color = Color.FromRgb(0, 0, 0);
            else
                BackgroundGridColors.Color = Color.FromRgb(255, 255, 255);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    BackgroundChange();
                    break;

                case Key.Escape:
                    Close();
                    break;
            }
        }
    }
}