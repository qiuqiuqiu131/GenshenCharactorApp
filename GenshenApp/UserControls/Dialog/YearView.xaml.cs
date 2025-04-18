﻿using GenshenApp.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenshenApp.UserControls
{
    /// <summary>
    /// YearView.xaml 的交互逻辑
    /// </summary>
    public partial class YearView : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        public YearView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<DialogHide>().Publish();
        }
    }
}
