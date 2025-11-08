using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Work_winui_.User
{
    public sealed partial class SideBar : UserControl
    {
        public event EventHandler<string> NavigationItemInvoked;

        public SideBar()
        {
            this.InitializeComponent();
            Nav.SelectionChanged += Nav_SelectionChanged;
        }

        private void Nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem && selectedItem.Tag != null)
            {
                NavigationItemInvoked?.Invoke(this, selectedItem.Tag.ToString());
            }
        }
    }
}