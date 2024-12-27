using System.Collections.ObjectModel;
using Evaluacion_3.Modelos.Modelos;
using Firebase.Database;
using Firebase.Database.Query;

namespace Evaluacion_3.Vistas;

public partial class ListarEstudiantes : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");
	public ObservableCollection<Estudiante> Lista {  get; set; } = new ObservableCollection<Estudiante>();
    private List<Estudiante> ListaCompleta { get; set; } = new List<Estudiante>();
    private string IdEstudiante;
    public ListarEstudiantes()
	{
		InitializeComponent();
		BindingContext = this;
		GenerarLista();
	}

    private async void GenerarLista()
    {
        // Con esto obtengo los datos cargados en la BD al inicio de la app
        var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();
        foreach (var estudiante in estudiantes)
        {
            if (estudiante.Object.Estado)
            {
                estudiante.Object.Id = estudiante.Key;
                // Verifica si el estudiante ya está en la lista antes de agregarlo
                if (!Lista.Any(e => e.Id == estudiante.Key))
                {
                    Lista.Add(estudiante.Object);
                    ListaCompleta.Add(estudiante.Object);
                }
            }
        }

        // Con esto cargo los cambios que se realicen en el listado, carga de nuevos alumnos
        client.Child("Estudiantes").AsObservable<Estudiante>().Subscribe((estudiante) =>
        {
            if (estudiante != null && estudiante.Object.Estado && !Lista.Any(e => e.Id == estudiante.Key))
            {
                // Asignar el Key cuando se actualiza el estudiante
                estudiante.Object.Id = estudiante.Key;
                Lista.Add(estudiante.Object);
                ListaCompleta.Add(estudiante.Object);
            }
        });
    }


    private void FiltroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text.ToLower();

        if (filtro.Length > 0)
        {
            ListaCollection.ItemsSource = ListaCompleta
                .Where(x => x.Estado && x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            ListaCollection.ItemsSource = ListaCompleta.Where(x => x.Estado);
        }
    }

    private async void NuevoEstudianteBoton_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CrearEstudiante());
	}

    private async void editarButton_Clicked(object sender, EventArgs e)
    {

        var boton = sender as ImageButton;
        var estudiante = boton?.CommandParameter as Estudiante;

        if (estudiante != null && !string.IsNullOrEmpty(estudiante.Id))
        {
            await Navigation.PushAsync(new EditarEstudiante(estudiante.Id));
        }
        else
        {
            await DisplayAlert("Error", "No se pudo obtener la informacion del estudiante", "OK");
        }
    }

    private async void deshabilitarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var estudiante = boton?.CommandParameter as Estudiante;

        if (estudiante == null)
        {
            await DisplayAlert("Error", "No se pudo obtener la informacion del estudiante", "OK");
            return;
        }

        bool confirmacion = await DisplayAlert
            ("Confirmacion", $"Esta seguro que desea deshabilitar al estudiante {estudiante.NombreCompleto}", "Si", "No");

        if (confirmacion)
        {
            try
            {
                estudiante.Estado = false;
                await client.Child("Estudiantes").Child(estudiante.Id).PutAsync(estudiante);
                await DisplayAlert("Exito", $"Se ha deshabilitado correctamente al usuario {estudiante.NombreCompleto}", "OK");
                GenerarLista();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}