using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Logging;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            ActualizarCursos();
            ActualizarEstudiantes();
            return builder.Build();
        }

        public static async Task ActualizarCursos()
        {
            FirebaseClient client = new FirebaseClient("https://registroestudiantes-8408d-default-rtdb.firebaseio.com/");
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            
            if(cursos.Count == 0)
            {
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "1ro Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "2do Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "3ro Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "4to Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "5to Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "6to Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "7mo Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "8vo Básico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "1ro Medio" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "2do Medio" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "3ro Medio" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "4to Medio" });
            }
            else
            {
                foreach (var curso in cursos)
                {
                    if(curso.Object.Estado == null)
                    {
                        var cursoActualizado = curso.Object;
                        cursoActualizado.Estado = true;

                        await client.Child("Cursos").Child(curso.Key).PutAsync(cursoActualizado);
                    }
                }
            }
        }
        public static async Task ActualizarEstudiantes()
        {
            FirebaseClient client = new FirebaseClient("https://registroestudiantes-8408d-default-rtdb.firebaseio.com/");
            var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();

            foreach (var estudiante in estudiantes)
            {
                if(estudiante.Object.Estado == null)
                {
                    var estudianteActualizado = estudiante.Object;
                    estudianteActualizado.Estado = true;

                    await client.Child("Estudiantes").Child(estudiante.Key).PutAsync(estudianteActualizado);
                }
            }
        }
    }
}
