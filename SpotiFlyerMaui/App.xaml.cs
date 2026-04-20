

namespace SpotiFlyerMaui
{
    public partial class App : Application
    {
        private static readonly string Tag = nameof(App);
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Console.WriteLine($"{Tag}: CreateWindow");
            //return new Window(new AppShell());
            return base.CreateWindow(activationState);
        }
    }
}
