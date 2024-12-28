using System.Collections.ObjectModel;
using Evaluacion_3.Modelos.Modelos;
using Firebase.Database;
using Firebase.Database.Query;

namespace Evaluacion_3.Vistas;

public partial class EditarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-49dad-default-rtdb.europe-west1.firebasedatabase.app/");
    public ObservableCollection<Nivel> ListarNil { get; set; } = new ObservableCollection<Nivel>();
    public ObservableCollection<Curso> ListarCursos { get; set; } = new ObservableCollection<Curso>();
    private readonly string estudianteID;

    public EditarEstudiante(string idEstudiante)
    {
        InitializeComponent();
        BindingContext = this;
        estudianteID = idEstudiante;
        ListarNivel();
        CargarEstudiante(estudianteID);
    }
    private async void ListarNivel()
    {
        try
        {
            var niveles = await client.Child("Nivel").OnceAsync<Nivel>();
            ListarNil.Clear();
            foreach (var nivel in niveles)
            {
                ListarNil.Add(nivel.Object);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar niveles: {ex.Message}", "OK");
        }
    }

    private async void CargarCursosPorNivel(Nivel nivelSeleccionado)
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();

            var cursosFiltrados = cursos.Where(c => c.Object.Nivel == nivelSeleccionado.Nombre).ToList();

            if (cursosFiltrados.Any())
            {
                ListarCursos.Clear();
                foreach (var curso in cursosFiltrados)
                {
                    ListarCursos.Add(curso.Object);
                }
                cursoPicker.IsEnabled = true;
            }
            else
            {
                cursoPicker.IsEnabled = false;
                await DisplayAlert("Información", "No hay cursos disponibles para este nivel.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar cursos: {ex.Message}", "OK");
        }
    }



    private async void CargarEstudiante(string idEstudiante)
    {
        try
        {
            var estudiante = await client.Child("Estudiantes").Child(idEstudiante).OnceSingleAsync<Estudiante>();

            if (estudiante != null)
            {
                EditPrimerNombreEntry.Text = estudiante.PrimerNombre;
                EditSegundoNombreEntry.Text = estudiante.SegundoNombre;
                EditPrimerApellidoEntry.Text = estudiante.PrimerApellido;
                EditSegundoApellidoEntry.Text = estudiante.SegundoApellido;
                EditCorreoEntry.Text = estudiante.CorreoElectronico;

                var nivelSeleccionado = ListarNil.FirstOrDefault(n => n.Nombre == estudiante.Nivel);
                if (nivelSeleccionado != null)
                {
                    nivelPicker.SelectedItem = nivelSeleccionado;
                    CargarCursosPorNivel(nivelSeleccionado);
                }

                var cursoSeleccionado = ListarCursos.FirstOrDefault(c => c.Nombre == estudiante.Curso);
                if (cursoSeleccionado != null)
                {
                    cursoPicker.SelectedItem = cursoSeleccionado;
                }

                estadoSwitch.IsToggled = estudiante.Estado;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar el estudiante: {ex.Message}", "OK");
        }
    }

    private async void NivelPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (nivelPicker.SelectedItem is Nivel nivelSeleccionado)
        {
            CargarCursosPorNivel(nivelSeleccionado);  
        }
    }

    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditCorreoEntry.Text) ||
                nivelPicker.SelectedItem == null ||
                cursoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            if (!IsValidEmail(EditCorreoEntry.Text))
            {
                await DisplayAlert("Error", "El correo electrónico no es válido", "OK");
                return;
            }

            Estudiante estudianteActualizado = new Estudiante
            {
                PrimerNombre = EditPrimerNombreEntry.Text.Trim(),
                SegundoNombre = EditSegundoNombreEntry.Text.Trim(),
                PrimerApellido = EditPrimerApellidoEntry.Text.Trim(),
                SegundoApellido = EditSegundoApellidoEntry.Text.Trim(),
                CorreoElectronico = EditCorreoEntry.Text.Trim(),
                Nivel = ((Nivel)nivelPicker.SelectedItem).Nombre,  
                Curso = ((Curso)cursoPicker.SelectedItem).Nombre,  
                Estado = estadoSwitch.IsToggled
            };

            await client.Child("Estudiantes").Child(estudianteID).PutAsync(estudianteActualizado);

            await DisplayAlert("Éxito", "El estudiante se ha actualizado correctamente", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al actualizar el estudiante: {ex.Message}", "OK");
        }
    }

    private bool IsValidEmail(string email)
    {
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
    }
}
