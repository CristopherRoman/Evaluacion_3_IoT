using Evaluacion_3.Vistas;

namespace Evaluacion_3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new CrearEstudiante());
        }
    }
}