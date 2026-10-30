using System.Configuration;
using System.Data;
using System.Windows;
using DotNetEnv;

namespace GameOn;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Env.Load();
    }
}

