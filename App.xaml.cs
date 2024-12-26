namespace ToDoList
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Destroying += Window_Destroying;

            return window;
        }

        private void Window_Destroying(object sender, EventArgs e)
        {
            if (MainPage.BindingContext is TodoViewModel viewModel)
            {
                viewModel.SaveTodos();
            }
        }
    }
}