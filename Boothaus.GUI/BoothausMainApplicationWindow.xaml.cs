using Boothaus.Domain;
using Boothaus.GUI;
using Boothaus.GUI.ViewModels;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using Domain.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Boothaus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BoothausMainApplicationWindow : ThemedWindow
{ 
    public BoothausMainApplicationWindow()
    {
        InitializeComponent(); 
    } 
}
