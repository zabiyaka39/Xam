using Plugin.Settings;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class Transition : ContentPage
    {
        public List<Fields> fieldIssue { get; set; }//поля заявки
        public Transition()
        {
            InitializeComponent();
            
            Request request = new Request(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + $"/rest/api/2/issue/IT-5444/transitions?expand=transitions.fields");
            fieldIssue = request.GetFieldTransitions();
        }

    }
}
