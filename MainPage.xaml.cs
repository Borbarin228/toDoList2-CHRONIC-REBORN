using Microsoft.Maui.Controls;

namespace ToDoList
{
    public partial class MainPage : ContentPage 
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new TodoViewModel();
        }
    }

}
