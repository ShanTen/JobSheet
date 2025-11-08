using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Work_winui_.User
{
    public sealed partial class Searchpage : UserControl
    {
        public Searchpage()
        {
            this.InitializeComponent();
        }

        private void AddCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            OrSeparator.Visibility = Visibility.Visible;
            SecondSearchRow.Visibility = Visibility.Visible;
            AddCriteriaButton.Visibility = Visibility.Collapsed;
        }
    }
}
