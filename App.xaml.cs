using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KalevaAalto.TxtToEpub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            
        }

        private async void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            await KalevaAalto.Main.ProcessInitAsync();
        }
    }
}
