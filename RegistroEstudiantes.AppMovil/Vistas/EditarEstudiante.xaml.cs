namespace RegistroEstudiantes.AppMovil.Vistas;

using System.Collections.ObjectModel;
using Firebase.Database;
using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;
public partial class EditarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-8408d-default-rtdb.firebaseio.com/");
    public List<Curso> Cursos{ get; set; }
    public ObservableCollection<string> ListaCursos { set; get; } = new ObservableCollection<string>();
    private Estudiante estudianteActual = new Estudiante();
    private string estudianteId;
    public EditarEstudiante(string idEstudiante)
	{
		InitializeComponent();
        BindingContext = this;
        estudianteId = idEstudiante;
        CargarListaCursos();
        CargarEstudiante(estudianteId);
	}


    private async void CargarListaCursos()
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            ListaCursos.Clear();
            foreach (var curso in cursos)
            {
                ListaCursos.Add(curso.Object.Nombre);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error:" + ex.Message, "OK");
        }

    }

    private async void CargarEstudiante(string idEstudiante)
    {
        var estudiante = await client.Child("Estudiantes").Child(idEstudiante).OnceSingleAsync<Estudiante>();

        if(estudiante != null)
        {
            EditPrimerNombreEntry.Text = estudiante.PrimerNombre;
            EditSegundoNombreEntry.Text = estudiante.SegundoNombre;
            EditPrimerApellidoEntry.Text = estudiante.PrimerApellido;
            EditSegundoApellidoEntry.Text = estudiante.SegundoApellido;
            EditCorreoEntry.Text = estudiante.CorreoElectronico;
            EditEdadEntry.Text = estudiante.Edad.ToString();
            EditCursoPicker.SelectedItem = estudiante.Curso?.Nombre;

        }
    }



    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text)||
                    string.IsNullOrWhiteSpace(EditSegundoNombreEntry.Text) ||
                    string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
                    string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                    string.IsNullOrWhiteSpace(EditCorreoEntry.Text) ||
                    string.IsNullOrWhiteSpace(EditEdadEntry.Text) ||
                    EditCursoPicker.SelectedItem == null
                    )
            {
                await DisplayAlert("Error", "Todos los Campos son obligatorios", "OK");
                return;
            }

            if (!EditCorreoEntry.Text.Contains("@"))
            {
                await DisplayAlert("Error", "El correo electrónico no es válido", "OK");
                return;
            }

            if(!int.TryParse(EditEdadEntry.Text, out var edad))
            {
                await DisplayAlert("Error", "La edad ingresada no es un número válido", "OK");
                return;
            }

            if(edad <= 0)
            {
                await DisplayAlert("Error", "La edad debe ser mayor o igual a cero", "OK");
                return;
            }
            estudianteActual.Id = estudianteId;
            estudianteActual.PrimerNombre = EditPrimerNombreEntry.Text.Trim();
            estudianteActual.SegundoNombre = EditSegundoNombreEntry.Text.Trim();
            estudianteActual.PrimerApellido = EditPrimerApellidoEntry.Text.Trim();
            estudianteActual.SegundoApellido = EditSegundoApellidoEntry.Text.Trim();
            estudianteActual.CorreoElectronico = EditCorreoEntry.Text.Trim();
            estudianteActual.Edad = edad;
            estudianteActual.Curso = new Curso { Nombre = EditCursoPicker.SelectedItem.ToString() };
            estudianteActual.Estado = editEstadoSwitch.IsToggled;
            await client.Child("Estudiantes").Child(estudianteActual.Id).PatchAsync(estudianteActual);
            await DisplayAlert("Éxito", "El estudiante se ha modificado de manera correcta", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}