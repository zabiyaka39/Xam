//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::Xamarin.Forms.Xaml.XamlResourceIdAttribute("RTMobile.issues.AllIssuesView.xaml", "issues/AllIssuesView.xaml", typeof(global::RTMobile.issues.AllIssuesView))]

namespace RTMobile.issues {
    
    
    [global::Xamarin.Forms.Xaml.XamlFilePathAttribute("issues\\AllIssuesView.xaml")]
    public partial class AllIssuesView : global::Xamarin.Forms.ContentPage {
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.ListView issuesList;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.StackLayout noneIssue;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private void InitializeComponent() {
            global::Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(AllIssuesView));
            issuesList = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.ListView>(this, "issuesList");
            noneIssue = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.StackLayout>(this, "noneIssue");
        }
    }
}
