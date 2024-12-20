using System.Collections.ObjectModel;
using Evaluacion_3.Modelos.Modelos;
using Firebase.Database;

namespace Evaluacion_3.Vistas;

public partial class ListarEstudiantes : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");
	public ObservableCollection<Estudiante> Lista {  get; set; } = new ObservableCollection<Estudiante>();
	public ListarEstudiantes()
	{
		InitializeComponent();
		BindingContext = this;
		GenerarLista();
	}

    private void GenerarLista()
    {
		client.Child("Estudiante").AsObservable<Estudiante>().Subscribe((Estudiante) =>
		{
			if (Estudiante.Object != null)
			{
				Lista.Add(Estudiante.Object);
			}
		});
    }

    private void FiltroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar?.Text?.ToLower() ?? string.Empty;

        if (filtro.Length > 0)
        {
            ListaCollection.ItemsSource = Lista
                .Where(x => !string.IsNullOrEmpty(x.NombreCompleto) && x.NombreCompleto.ToLower().Contains(filtro))
                .ToList(); 
        }
        else
        {
            ListaCollection.ItemsSource = Lista;
        }
    }

    private async void NuevoEstudianteBoton_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CrearEstudiante());
	}
}