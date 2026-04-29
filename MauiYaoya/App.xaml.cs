namespace MauiYaoya
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // 未処理例外をログに出す
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(
                    $"UnhandledException: {e.ExceptionObject}");
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "MauiYaoya" };
        }
    }
}
